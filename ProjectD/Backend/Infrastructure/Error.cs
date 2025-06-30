using ExcelDataReader.Log;
using Serilog;
using Serilog.Sinks.PostgreSQL;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Microsoft.Extensions.Options;
using Serilog.Sinks.PostgreSQL.ColumnWriters;
using NpgsqlTypes;
using Newtonsoft.Json.Linq;

public class Error
{
    public int StatusCode { get; set; }
    public string Details { 
        get
        {
            return StatusCode switch
            {
                204 => "No Content - Recource is gone or missing",
                302 => "Redirected - User has been redirected to another URL.",
                400 => "Bad Request - The server could not understand the request.",
                401 => "Unauthorized - Authentication is required.",
                403 => "Forbidden - You do not have permission.",
                404 => "Not Found - The resource could not be found.",
                405 => "Method Not Allowed - The HTTP method is not allowed for the requested resource.",
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
        // LogError(Date);
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
        try
        {
        string path = @$"Backend\Data\ErrorLogs\{date:yyyy-MM-dd}.json";
        var errorJToken = JToken.FromObject(this);

        JArray logArray = new JArray();

        if (File.Exists(path) && new FileInfo(path).Length > 0)
        {
            var existingContent = File.ReadAllText(path);
            logArray = JArray.Parse(existingContent);
        }

        logArray.Add(errorJToken);

        File.WriteAllText(path, logArray.ToString(Formatting.Indented));
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error logging to file: {ex.Message}");
        }
    }

    // ToString() is mainly used for METADETA display for DEBUG
    public override string ToString() => $"\tStatusCode: {StatusCode}\n\tDetails: {Details}\n\tMessage: {Message}\n\tURL: {Url}\n\tDateTime(EU): {Date:dd/MM/yyyy HH:mm:ss}";
}