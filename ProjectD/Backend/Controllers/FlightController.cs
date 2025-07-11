using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using System;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Net.Http.Headers;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Caching.Memory;
using ProjectD.Models;

namespace ProjectD
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class FlightController : ControllerBase
    {
        private readonly FlightDBContext _context;
        private readonly IMemoryCache _cache;

        public FlightController(FlightDBContext context, IMemoryCache cache)
        {
            _context = context;
            _cache = cache;
        }

        [Authorize(Roles = "User, Admin")]
        [HttpGet("{id}")]
        public async Task<ActionResult<Flight>> GetFlightById(int id)
        {
            string userId = User == null ? "anonymous" : "autorized";
            string cacheKey = $"user:{userId}:flights:{id}";
            if (userId == "anonymous")
            {
                return Unauthorized(new Error(401, Request.Path, "You must be logged in to access this resource."));
            }
            if (_cache.TryGetValue(cacheKey, out Flight? cachedResult))
            {
                return Ok(cachedResult);
            }
            if (_cache.TryGetValue(cacheKey, out Error cachedError))
            {
                return NotFound(cachedError);
            }
            Console.WriteLine("[DEBUG] Flight ID received: " + id);

            var flight = await _context.Flights.FirstOrDefaultAsync(f => f.Id == id);
            if (flight == null)
            {
                Console.WriteLine("[DEBUG] Flight not found.");
                Error error = new Error(404, Request?.Path ?? "/unknown", $"Flight with ID {id} not found.");
                _cache.Set(cacheKey, error, TimeSpan.FromSeconds(300));
                return NotFound(error);
            }

            Console.WriteLine("[DEBUG] Flight found: " + flight.Id);
            _cache.Set(cacheKey, flight, TimeSpan.FromSeconds(300));
            return Ok(flight);
        }

        [Authorize(Roles = "Admin")]
        [HttpGet("Flights with IDs and URL")]
        public async Task<ActionResult<Dictionary<int, string>>> GetFlightsWithID()
        {
            var URL = $"{Request.Scheme}://{Request.Host}/api/flight";
            string userId = User == null ? "anonymous" : "autorized";
            string cacheKey = $"user:{userId}:flights:Flights_with_IDs_and_URL";
            if (userId == "anonymous")
            {
                return Unauthorized(new Error(401, Request.Path, "You must be logged in to access this resource."));
            }
            if (_cache.TryGetValue(cacheKey, out  Dictionary<int, string>? cachedResult))
            {
                return Ok(cachedResult);
            }
            if (_cache.TryGetValue(cacheKey, out Error cachedError))
            {
                return NotFound(cachedError);
            }
            var flightsLinks = await _context.Flights
                .Select(f => new { f.Id })
                .ToDictionaryAsync(
                    f => f.Id,
                    f => $"{URL}/{f.Id}"
                );

            if (!flightsLinks.Any())
            {
                Error error = new Error(404, Request?.Path ?? "/unknown", "No flights found");
                _cache.Set(cacheKey, error, TimeSpan.FromSeconds(300));
                return NotFound();
            }
            _cache.Set(cacheKey, flightsLinks, TimeSpan.FromSeconds(300));
            return Ok(flightsLinks);
        }

        [Authorize(Roles = "User, Admin")]
        [HttpGet("filter")]
        [ResponseCache(Duration = 60)]
        public async Task<IActionResult> FilterFlights(
            [FromQuery] int? id,
            [FromQuery] string? date,
            [FromQuery] string? country,
            [FromQuery] int page = 1)
        {
            const int pageSize = 100;
            if (page < 1) page = 1;

            var URL = $"{Request.Scheme}://{Request.Host}/api/flight";
            string userId = User == null ? "anonymous" : "autorized";
            // cache values in case of nulls
            string? cv1 = id == null ? "?" : id.ToString();
            string? cv2 = date == null ? "?" : date;
            string? cv3 = country == null ? "?" : country;
            string cacheKey = $"user:{userId}:flights:filter:id:{cv1}:date:{cv2}:country:{cv3}:page:{page}";
            if (userId == "anonymous")
            {
                return Unauthorized(new Error(401, Request.Path, "You must be logged in to access this resource."));
            }
            if (_cache.TryGetValue(cacheKey, out PageManagerFlights cachedResult))
            {
                return Ok(cachedResult);
            }
            if (_cache.TryGetValue(cacheKey, out Error cachedError))
            {
                return NotFound(cachedError);
            }
            
            var query = _context.Flights.AsQueryable();

            if (id.HasValue)
            {
                query = query.Where(f => f.Id == id.Value);
            }

            if (!string.IsNullOrEmpty(date))
            {
                if (DateTime.TryParse(date, out DateTime parsedDate))
                {
                    parsedDate = DateTime.SpecifyKind(parsedDate, DateTimeKind.Utc);
                    query = query.Where(f => f.ScheduledLocal.Date == parsedDate.Date);
                }
                else
                {

                    return BadRequest(new { status = 400, message = "Use YYYY-MM-DD." });
                }
            }

            if (!string.IsNullOrEmpty(country))
            {
                query = query.Where(f => f.Country.ToLower() == country.ToLower());
            }

            var totalItems = await query.CountAsync();
            var totalPages = (int)Math.Ceiling(totalItems / (double)pageSize);

            var flights = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToArrayAsync();

            if (!flights.Any())
            {
                Error error = new Error(404, Request.Path, "No flights found for the given criteria.");
                _cache.Set(cacheKey, error, TimeSpan.FromSeconds(300));
                return NotFound(error);
            }

            PageManagerFlights results = new PageManagerFlights(page, 100, totalItems, flights);
            _cache.Set(cacheKey, results, TimeSpan.FromSeconds(300));
            return Ok(results);
        }
    }
}
