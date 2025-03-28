using ExcelDataReader.Log;
using Serilog;
using Serilog.Sinks.PostgreSQL;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Microsoft.Extensions.Options;

public class Error
{
    public int StatusCode { get; set; }
    public string Details { 
        get
        {
            return StatusCode switch
            {
                400 => "Bad Request - The server could not understand the request.",
                401 => "Unauthorized - Authentication is required.",
                403 => "Forbidden - You do not have permission.",
                404 => "Not Found - The resource could not be found.",
                500 => "Internal Server Error - Something went wrong on the server.",
                _ => "Unknown Error - No details available."
            };
        } 
    }
    public string Message { get; set; }
    public string Url { get; set; } 
    public Error(int statusCode, string url, string message = null)
    {
        StatusCode = statusCode;
        Message = message == null ? "An error occurred." : message;
        Url = url;
        LogError();
    }

    private void LogError()
    {
        // Serialize the error object to JSON with indentation
        string errorJson = JsonConvert.SerializeObject(this, Formatting.Indented, new JsonSerializerSettings
        {
            ContractResolver = new CamelCasePropertyNamesContractResolver()
        });

        // Log the error as a string for Serilog
        Serilog.Log.Error(this.ToString());

        // Get today's date to name the log file
        DateTime date = DateTime.Now; 
        DateOnly dateOnly = new DateOnly(date.Year, date.Month, date.Day);
        string path = @$"Backend\Data\ErrorLogs\{dateOnly}.json";

        if(File.Exists(path) && new FileInfo(path).Length > 0)
        {
            // If the file exists, read the existing content (remove the closing bracket) and append the new error
            string existingJson = File.ReadAllText(path);
            existingJson = existingJson.TrimEnd(']'); // Remove the closing bracket
            existingJson += $",{Environment.NewLine}{errorJson}"; // Add the new error with a comma
            existingJson += Environment.NewLine + "]"; // Close the JSON array

            // Write the updated content back to the file
            File.WriteAllText(path, existingJson);
        }
        else
        {
            // If the file does not exist or is empty, start a new JSON array and add the first error
            string jsonArray = "[" + Environment.NewLine + errorJson + Environment.NewLine + "]";
            File.WriteAllText(path, jsonArray);
        }
    }

    // ToString() is mainly used for logging
    public override string ToString() => $"Error {StatusCode}: {Message} ({Details}) at {Url}";
}