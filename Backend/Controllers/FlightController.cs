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

        // Haal vlucht op op basis van een FlightNumber (string)
        [HttpGet("{flightNumber}")]
        public async Task<ActionResult<Flight>> GetFlightByFlightNumber(string flightNumber)
        {
            Console.WriteLine("[DEBUG] FlightNumber ontvangen: " + flightNumber);  // Debuglog

            var flight = await _context.Flights.FirstOrDefaultAsync(f => f.FlightNumber == flightNumber);  // Match op string

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