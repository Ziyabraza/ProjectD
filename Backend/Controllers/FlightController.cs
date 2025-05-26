using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using System;

namespace ProjectD
{
    [Route("api/[controller]")]
    [ApiController]
    public class FlightController : ControllerBase
    {
        private readonly FlightDBContext _context;

        public FlightController(FlightDBContext context)
        {
            _context = context;
        }

        // Zoek vlucht op basis van een FlightNumber als string
        [HttpGet("{flightNumber}")]
        public async Task<ActionResult<Flight>> GetFlightByFlightNumber(string flightNumber)
        {
            Console.WriteLine("[DEBUG] FlightNumber ontvangen: " + flightNumber);  // Log de input

            var flight = await _context.Flights.FirstOrDefaultAsync(f => f.FlightNumber == flightNumber);
            if (flight == null)
            {
                Console.WriteLine("[DEBUG] Flight niet gevonden.");
                return NotFound(new { message = "Flight not found" });
            }

            Console.WriteLine("[DEBUG] Flight gevonden: " + flight.FlightNumber);
            return Ok(flight);
        }

        [HttpGet("filter")]
        public async Task<IActionResult> FilterFlights(
            [FromQuery] string? date, 
            [FromQuery] string? country, 
            [FromQuery] int page = 1)
        {
            const int pageSize = 100;

            if (page < 1) page = 1;

            var query = _context.Flights.AsQueryable();

            if (!string.IsNullOrEmpty(date))
            {
                if (DateTime.TryParse(date, out DateTime parsedDate))
                {
                    parsedDate = DateTime.SpecifyKind(parsedDate, DateTimeKind.Utc);

                    query = query.Where(f => f.ScheduledLocal.Date == parsedDate.Date);
                }
                else
                {
                    return BadRequest(new { status = 400, message = "Ongeldig datumformaat. Gebruik YYYY-MM-DD." });
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
                    message = "Geen vluchten gevonden voor de opgegeven filters."
                });
            }

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