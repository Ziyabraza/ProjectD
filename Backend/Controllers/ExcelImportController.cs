using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ProjectD.Models;
using System.Threading.Tasks;

namespace ProjectD.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ExcelImportController : ControllerBase
    {
        private readonly ExcelImportService _importService;

        public ExcelImportController(ExcelImportService importService)
        {
            _importService = importService;
        }

        [HttpPost("flights")]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> Import([FromForm] ExcelImportRequest request)
        {
            if (request.FlightsFile == null || request.TouchpointsFile == null)
                return BadRequest("Both Excel files are required.");

            using var f1 = request.FlightsFile.OpenReadStream();
            using var f2 = request.TouchpointsFile.OpenReadStream();

            await _importService.ImportFlights(f1, f2);

            return Ok("Data imported into PostgreSQL successfully.");
        }
    }
}