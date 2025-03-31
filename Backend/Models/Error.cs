using ExcelDataReader.Log;
using Serilog;
using Serilog.Sinks.PostgreSQL;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Microsoft.Extensions.Options;
using Serilog.Sinks.PostgreSQL.ColumnWriters;
using NpgsqlTypes;

public class Error
{
    public int StatusCode { get; set; }
    public string Details { 
        get
        {
            return StatusCode switch
            {
                302 => "Redirected - User has been redirected to another URL.",
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
    public DateTime Date { get; set; }
    public Error(int statusCode, string url, string message = null)
    {
        StatusCode = statusCode;
        Message = message == null ? "An error occurred." : message;
        Url = url;
        Date = DateTime.Now;
        LogError(Date);
        LogSerilog();
    }
    private void LogSerilog()
    {
        if (!string.IsNullOrEmpty(Message) && !string.IsNullOrEmpty(Url))
        {
            Serilog.Log.ForContext("status_code", StatusCode)
                .ForContext("details", Details)
                .ForContext("message", Message)
                .ForContext("url", Url)
                .Error($"An error occurred:\n{this.ToString()}");
        }
        else
        {
            // Optionally log an internal message to inform why it's not logged
            Serilog.Log.Warning("Error log skipped: One or more required fields are missing (Message, Url, or StatusCode).");
        }
    }

    private void LogError(DateTime date)
    {
        // Serialize the error object to JSON with indentation
        string errorJson = JsonConvert.SerializeObject(this, Formatting.Indented, new JsonSerializerSettings
        {
            ContractResolver = new CamelCasePropertyNamesContractResolver()
        });

        // DateTime date = DateTime.Now; 
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

    // ToString() is mainly used for METADETA display for DEBUG
    public override string ToString() => $"\tStatusCode: {StatusCode}\n\tDetails: {Details}\n\tMessage: {Message}\n\tURL: {Url}\nDateTime(EU): {Date:dd/MM/yyyy HH:mm:ss}";
}