using FsToolkit.ErrorHandling;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
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
        private readonly IMemoryCache _cache;

        public TouchpointController(FlightDBContext context, IMemoryCache cache)
        {
            _context = context;
            _cache = cache;
        }

        [Authorize(Roles = "User, Admin")]
        [HttpGet("page/{page}")]
        public async Task<IActionResult> GetByPage(int page)
        {
            if (page < 1)
            {
                new Error(302, Request.Path, "Page number must be greater than 0.");
                return Redirect("1");
            }
            
            string userId = User == null ? "anonymous" : "autorized";
            string cacheKey = $"user:{userId}:touchpoints:page:{page}";
            if (userId == "anonymous")
            {
                return Unauthorized(new Error(401, Request.Path, "You must be logged in to access this resource."));
            }
            if (_cache.TryGetValue(cacheKey, out PageManagerTouchpoints cachedResult))
            {
                return Ok(cachedResult);
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

            PageManagerTouchpoints result = new PageManagerTouchpoints(page, totalPages, touchpoints);
            _cache.Set(cacheKey, result, TimeSpan.FromSeconds(300));

            return Ok(new PageManagerTouchpoints(page, totalPages, touchpoints));
        }


        [Authorize(Roles = "User, Admin")]
        [HttpGet("SearchByFlightID/{id}")]
        public async Task<IActionResult> GetByID(int id)
        {
            // try to extract out of server sided cache
            string userId = User == null ? "anonymous" : "autorized";
            string cacheKey = $"user:{userId}:touchpoints:SearchByFlightID:{id}";
            if (userId == "anonymous")
            {
                // gebruiker is niet ingeloged
                return Unauthorized(new Error(401, Request.Path, "You must be logged in to access this resource.")); 
            }
            if (_cache.TryGetValue(cacheKey, out List<Touchpoint> cachedResult)) // try List<Touchpoint result
            {
                return Ok(cachedResult);
            }
            if (_cache.TryGetValue(cacheKey, out Error cachedError)) // try Error result
            {
                return NotFound(cachedError);
            }
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
                Error error = new Error(404, Request.Path, "An error occured.\nthere are no Touchpoints found make contact with Webprovider if its ongoing issue.\nSorry for inconvinence.");
                _cache.Set(cacheKey, error, TimeSpan.FromSeconds(300));
                return NotFound(error);
            }
            if (results == null || results.Count == 0)
            {
                Error error = new Error(404, Request.Path, $"No touchpoints found for Flight ID {id}.");
                _cache.Set(cacheKey, error, TimeSpan.FromSeconds(300));
                return NotFound(error);
            }

            _cache.Set(cacheKey, results, TimeSpan.FromSeconds(300));
            return Ok(results);
        }
    }
}