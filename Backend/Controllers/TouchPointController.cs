using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProjectD.Models; // Ensure you have the correct namespace for your models

namespace ProjectD.Controllers
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

        private Touchpoint[] PageMessage(int page, int start, int end, Touchpoint[] touchpoints)
        {
            string message = $"Touchpoint page{page} ({start+1} - {end+1}):\n\n";

            if(touchpoints.Length == 0) { return null; } // this will throw status NotFound
            Touchpoint[] newTouchpoints = new Touchpoint[100]; // default == null
            if(touchpoints.Length < start) { return newTouchpoints; } // this will throw status NotFound

            for(int i = start; i < end; i++)
            {
                if(touchpoints.Length>i) 
                { 
                    newTouchpoints[i%100] = touchpoints[i];
                    // message += $"{i + 1} {touchpoints[i].FlightId} || {touchpoints[i].TouchpointType} || {touchpoints[i].TouchpointTime} || {touchpoints[i].TouchpointPax} \n"; 
                }
            }
            return newTouchpoints;
        }

        private string ToString(Touchpoint touchpoint) => $"{touchpoint.FlightId} || {touchpoint.TouchpointType} || {touchpoint.TouchpointTime} || {touchpoint.TouchpointPax}";

        [Authorize(Roles = "Admin")]
        [HttpGet("page/{page}")]
        public async Task<IActionResult> GetPage1(int page)
        {
            if(page < 1) 
            { 
                new Error(302, Request.Path, "Page number must be greater than 0.");
                return Redirect("1"); // moves user to page 1 if page is less than 1
            }
            int start = page > 1 ? 100*(page-1) : 0;
            int end = 100*page;
            var touchpoints = await _context.Touchpoints.ToArrayAsync();
            if(PageMessage(page, start, end, touchpoints) == null) 
            { return NotFound(new Error(404, Request.Path, $"An error acured.\nThere are no Touchpoints found make contact with Webprovider if its ongoing issue.\nSorry for inconvinence.")); }
            if(PageMessage(page, start, end, touchpoints)?.Any(tp => tp != null) == false) // checks if there is any touchpoint availble past this page to display.
            { 
                int redirectObj = Convert.ToInt32(Math.Ceiling(Convert.ToDouble(touchpoints.Length)/100.00)); // calculates what is the last page for redirect
                new Error(302, Request.Path, $"No touchpoints found past this page. only {touchpoints.Length} touchpoints recorded. {redirectObj} pages recorded");
                return Redirect(redirectObj.ToString()); // moves user to last page if page is out of range
            }
            // if no null's detected on page execute it imidiatly to save on performance
            // if null('s) detected take extra masures to NOT show this to the user: 
            List<Touchpoint> TempTouchpoints = new();
            foreach(Touchpoint touchpoint in PageMessage(page, start, end, touchpoints))
            {
                if(touchpoint != null) { TempTouchpoints.Add(touchpoint); }
            }

            return Ok
            (
                new PageManager
                (
                    page,
                    Convert.ToInt32(Math.Ceiling(Convert.ToDouble(touchpoints.Length) / 100.00)),
                    TempTouchpoints.ToArray()
                )
            );
        }
        [Authorize(Roles = "User, Admin")]
        [HttpGet("SearchByFlightID/{id}")]
        public async Task<IActionResult> GetByID(int id)
        {

            bool found = false;
            // string message = "Match(es) found:\n\n";
            List<Touchpoint> results = new();

            var touchpoints = await _context.Touchpoints.ToArrayAsync();
            foreach(var touchpoint in touchpoints)
            {
                if(touchpoint.FlightId == id)
                {

                    // found = true;
                    results.Add(touchpoint);
                }
            }
            if(touchpoints == null || touchpoints.Length == 0) 
            { 
                return NotFound(new Error(404, Request.Path, "An error acured.\nthere are no Touchpoints found make contact with Webprovider if its ongoing issue.\nSorry for inconvinence.")); 
            }
            if(results == null || results.Count == 0) 
            { 
                return NotFound(new Error(404, Request.Path, $"No touchpoints found for Flight ID {id}.")); 
            }
            return Ok(results);

        }
    }
}