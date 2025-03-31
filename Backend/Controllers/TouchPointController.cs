using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProjectD.Models; // Ensure you have the correct namespace for your models

namespace ProjectD.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TouchpointController : ControllerBase
    {
        private readonly FlightDBContext _context; // Your DbContext

        public TouchpointController(FlightDBContext context)
        {
            _context = context;
        }

        private string PageMessage(int page, int start, int end, Touchpoint[] touchpoints)
        {
            string message = $"Touchpoint page{page} ({start+1} - {end+1}):\n\n";

            if(touchpoints.Length == 0) { return "Status 600"; } // this will throw status NotFound
            if(touchpoints.Length < start) { return "Status 601"; } // this will throw status NotFound

            for(int i = start; i < end; i++)
            {
                if(touchpoints.Length>i) { message += $"{i + 1} {touchpoints[i].FlightId} || {touchpoints[i].TouchpointType} || {touchpoints[i].TouchpointTime} || {touchpoints[i].TouchpointPax} \n"; }
            }
            return message;
        }

        private string ToString(Touchpoint touchpoint) => $"{touchpoint.FlightId} || {touchpoint.TouchpointType} || {touchpoint.TouchpointTime} || {touchpoint.TouchpointPax}";


        [HttpGet("page/{page}")]
        public async Task<IActionResult> GetPage1(int page)
        {
            if(page < 1) 
            { 
                new Error(302, Request.Path, "Page number must be greater than 0.");
                return Redirect("1"); // moves user to page 1 if page is less than 1
            }
            int start = page > 1 ? 1000*(page-1) - 1 : 0;
            int end = (1000*page) - 1;
            var touchpoints = await _context.Touchpoints.ToArrayAsync();
            if(PageMessage(page, start, end, touchpoints) == "Status 600") 
            { return NotFound(new Error(404, Request.Path, $"An error acured.\nThere are no Touchpoints found make contact with Webprovider if its ongoing issue.\nSorry for inconvinence.")); }
            if(PageMessage(page, start, end, touchpoints) == "Status 601") 
            { 
                int redirectObj = Convert.ToInt32(Math.Ceiling(Convert.ToDouble(touchpoints.Length)/1000.00));
                new Error(302, Request.Path, $"No touchpoints found past this page.\nonly {touchpoints.Length} touchpoints recorded.\n{redirectObj} pages recorded");
                return Redirect(redirectObj.ToString()); // moves user to last page if page is out of range
            }  
            return Ok(PageMessage(page, start, end, touchpoints));
        }
        
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
            if(touchpoints.Length == 0) { return NotFound(new Error(404, Request.Path, "An error acured.\nthere are no Touchpoints found make contact with Webprovider if its ongoing issue.\nSorry for inconvinence.")); }
            if(results.Count == 0) { return NotFound(new Error(404, Request.Path, $"No touchpoints found for Flight ID {id}.")); }
            return Ok(results);

        }
    }
}