using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using System;
using Microsoft.AspNetCore.Authorization;

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

        // Zoek vlucht op basis van een FlightId als string
        [Authorize(Roles = "User, Admin")]
        [HttpGet("{id}")]
        public async Task<ActionResult<Flight>> GetFlightById(int id)
        {
            Console.WriteLine("[DEBUG] Flight ID received: " + id);

            var flight = await _context.Flights.FirstOrDefaultAsync(f => f.Id == id);
            if (flight == null)
            {
                Console.WriteLine("[DEBUG] Flight not found.");
                return NotFound(new { message = $"Flight with ID {id} not found." });
            }

            Console.WriteLine("[DEBUG] Flight found: " + flight.Id);
            return Ok(flight);
        }
        [Authorize(Roles = "Admin")]
        [HttpGet("Flights with IDs and URL")]
        public async Task<ActionResult<Dictionary<int, string>>> GetFlightsWithID()
        {
            var URL = $"{Request.Scheme}://{Request.Host}/api/flight";
            var flightsLinks = await _context.Flights
            .Select(f => new 
            {
                f.Id
            })
            .ToDictionaryAsync(
                f => f.Id,
                f => $"{URL}/{f.Id}"
            );
            if(!flightsLinks.Any())
            {
                return NotFound("No flights found");
            }
            return Ok(flightsLinks);
        }
    }
}