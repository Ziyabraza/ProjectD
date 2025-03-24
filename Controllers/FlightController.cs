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
    }
}
