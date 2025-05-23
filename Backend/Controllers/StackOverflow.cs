using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProjectD.Models; // Ensure you have the correct namespace for your models

namespace ProjectD
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class StackOverflowController : ControllerBase
    {
        private readonly FlightDBContext _context; // Your DbContext

        public StackOverflowController(FlightDBContext context)
        {
            _context = context;
        }
        [Authorize(Roles = "User, Admin")]
        [HttpGet("Expirimental/GetAll")]
        public async Task<IActionResult> GetAllExpitrimental()
        {
            var touchpoints = await _context.Touchpoints.ToArrayAsync();
            var flights = await _context.Flights.ToArrayAsync();
            var overflowTest = new StressTester(touchpoints, flights);
            return Ok(overflowTest);
        }
        [Authorize(Roles = "User, Admin")]
        [HttpGet("Expirimental/GetWithLimit/{limit}")]
        public async Task<IActionResult> GetLimit(int limit)
        {
            Flight[] LimitedFlights = new Flight[limit];
            Touchpoint[] LimitedTouchpoints = new Touchpoint[limit];
            var touchpoints = await _context.Touchpoints.ToArrayAsync();
            var flights = await _context.Flights.ToArrayAsync();
            for (int i = 0; i < limit; i++)
            {
                LimitedFlights[i] = flights[i];
                LimitedTouchpoints[i] = touchpoints[i];
            }
            var overflowTest = new StressTester(LimitedTouchpoints, LimitedFlights);
            return Ok(overflowTest);
        }
    }
}