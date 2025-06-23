using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Net.Http.Headers;
using ProjectD.Models; // Ensure you have the correct namespace for your models

namespace ProjectD
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class TouchpointController : ControllerBase
    {
        private readonly FlightDBContext _context; // Your DbContext

        public TouchpointController(FlightDBContext context)
        {
            _context = context;
        }

        [Authorize(Roles = "User, Admin")]
        [HttpGet("page/{page}")]
        [ResponseCache(Duration = 60)] // Cache response for 60 seconds
        public async Task<IActionResult> GetPage1(int page)
        {
            Response.GetTypedHeaders().CacheControl = new CacheControlHeaderValue()
            {
                Private = true,
                MaxAge = TimeSpan.FromSeconds(60)
            };
            Response.Headers["Vary"] = "Authorization";
            if (page < 1)
            {
                new Error(302, Request.Path, "Page number must be greater than 0.");
                return Redirect("1");
            }

            int totalCount = await _context.Touchpoints.CountAsync();
            int totalPages = (int)Math.Ceiling(totalCount / 100.0);

            if (page > totalPages && totalCount > 0)
            {
                new Error(302, Request.Path, $"No touchpoints found past this page. only {totalCount} touchpoints recorded. {totalPages} pages recorded");
                return Redirect(totalPages.ToString());
            }

            var touchpoints = await _context.Touchpoints
                .OrderBy(tp => tp.Id) // Change to a field that ensures consistent order
                .Skip((page - 1) * 100)
                .Take(100)
                .ToArrayAsync();

            if (touchpoints.Length == 0)
            {
                return NotFound(new Error(404, Request.Path, "An error occured. There are no Touchpoints found make contact with Webprovider if its ongoing issue. Sorry for inconvinence."));
            }

            return Ok(new PageManager(page, totalPages, touchpoints));
        }


        [Authorize(Roles = "User, Admin")]
        [HttpGet("SearchByFlightID/{id}")]
        [ResponseCache(Duration = 60)] // Cache response for 60 seconds
        public async Task<IActionResult> GetByID(int id)
        {
            Response.GetTypedHeaders().CacheControl = new CacheControlHeaderValue()
            {
                Private = true,
                MaxAge = TimeSpan.FromSeconds(60)
            };
            Response.Headers["Vary"] = "Authorization";
            bool found = false;
            // string message = "Match(es) found:\n\n";
            List<Touchpoint> results = new();

            var touchpoints = await _context.Touchpoints.ToArrayAsync();
            foreach (var touchpoint in touchpoints)
            {
                if (touchpoint.FlightId == id)
                {

                    // found = true;
                    results.Add(touchpoint);
                }
            }
            if (touchpoints == null || touchpoints.Length == 0)
            {
                return NotFound(new Error(404, Request.Path, "An error occured.\nthere are no Touchpoints found make contact with Webprovider if its ongoing issue.\nSorry for inconvinence."));
            }
            if (results == null || results.Count == 0)
            {
                return NotFound(new Error(404, Request.Path, $"No touchpoints found for Flight ID {id}."));
            }
            return Ok(results);
        }
    }
}