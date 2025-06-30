using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using System;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Net.Http.Headers;
using System.Collections.Generic;
using System.Linq;

namespace ProjectD
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class FlightController : ControllerBase
    {
        private readonly FlightDBContext _context;

        public FlightController(FlightDBContext context)
        {
            _context = context;
        }

        private void CacheRequest()
        {
            Response.GetTypedHeaders().CacheControl = new CacheControlHeaderValue()
            {
                Private = true,
                MaxAge = TimeSpan.FromSeconds(60)
            };
            Response.Headers["Vary"] = "Authorization";
        }

        [Authorize(Roles = "User, Admin")]
        [HttpGet("{id}")]
        [ResponseCache(Duration = 60)]
        public async Task<ActionResult<Flight>> GetFlightById(int id)
        {
            Console.WriteLine("[DEBUG] Flight ID received: " + id);

            var flight = await _context.Flights.FirstOrDefaultAsync(f => f.Id == id);
            if (flight == null)
            {
                Console.WriteLine("[DEBUG] Flight not found.");
                return NotFound(new Error(404, Request?.Path ?? "/unknown", $"Flight with ID {id} not found."));
            }

            Console.WriteLine("[DEBUG] Flight found: " + flight.Id);
            CacheRequest();
            return Ok(flight);
        }

        [Authorize(Roles = "Admin")]
        [HttpGet("Flights with IDs and URL")]
        [ResponseCache(Duration = 60)]
        public async Task<ActionResult<Dictionary<int, string>>> GetFlightsWithID()
        {
            var URL = $"{Request.Scheme}://{Request.Host}/api/flight";

            var flightsLinks = await _context.Flights
                .Select(f => new { f.Id })
                .ToDictionaryAsync(
                    f => f.Id,
                    f => $"{URL}/{f.Id}"
                );

            if (!flightsLinks.Any())
            {
                return NotFound(new Error(404, Request?.Path ?? "/unknown", "No flights found"));
            }
            CacheRequest();
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
                .ToListAsync();

            if (!flights.Any())
            {
                return NotFound(new
                {
                    status = 404,
                    path = Request.Path,
                    message = "No flights found for the given criteria."
                });
            }
            CacheRequest();
            return Ok(new
            {
                page,
                pageSize,
                totalPages,
                totalItems,
                data = flights
            });
        }
    }
}
