using Microsoft.AspNetCore.Http;

namespace ProjectD.Models
{
    public class ExcelImportRequest
    {
        public IFormFile FlightsFile { get; set; }
        public IFormFile TouchpointsFile { get; set; }
    }
}
