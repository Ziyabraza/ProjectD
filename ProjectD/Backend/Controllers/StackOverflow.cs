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
        // [Authorize(Roles = "User, Admin")]
        // This does not work as intended check DEBUGGER CONSOLE for details
        [HttpGet("Expirimental/GetAll/All")]
        public async Task<IActionResult> GetAllExpitrimental()
        {
            var touchpoints = await _context.Touchpoints.ToArrayAsync();
            var flights = await _context.Flights.ToArrayAsync();
            var overflowTest = new StressTester(touchpoints, flights);
            return Ok(overflowTest);
        }
        [HttpGet("Expirimental/GetAll/Touchpoints")]
        public async Task<IActionResult> GetAllTouchpoints()
        {
            var touchpoints = await _context.Touchpoints.ToArrayAsync();
            return Ok(touchpoints);
        }
        [HttpGet("Expirimental/GetAll/Flights")]
        public async Task<IActionResult> GetAllFlights()
        {
            var flights = await _context.Flights.ToArrayAsync();
            return Ok(flights);
        }
        // [Authorize(Roles = "User, Admin")]
        [HttpGet("Expirimental/GetWithLimit/{limit}/All")]
        public async Task<IActionResult> GetLimitAll(int limit)
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
        [HttpGet("Expirimental/GetWithLimit/{limit}/Touchpoints")]
        public async Task<IActionResult> GetLimitTouchpoints(int limit)
        {
            Touchpoint[] LimitedTouchpoints = new Touchpoint[limit];
            var touchpoints = await _context.Touchpoints.ToArrayAsync();
            for (int i = 0; i < limit; i++)
            {
                LimitedTouchpoints[i] = touchpoints[i];
            }
            return Ok(LimitedTouchpoints);
        }
        [HttpGet("Expirimental/GetWithLimit/{limit}/Flights")]
        public async Task<IActionResult> GetLimitFlights(int limit)
        {
            Flight[] LimitedFlights = new Flight[limit];
            var flights = await _context.Flights.ToArrayAsync();
            for (int i = 0; i < limit; i++)
            {
                LimitedFlights[i] = flights[i];
            }
            return Ok(flights);
        }
    }
}