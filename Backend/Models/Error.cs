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
    public Error(int statusCode, string message = null)
    {
        StatusCode = statusCode;
        Message = message == null ? "An error occurred." : message;
    }

    // ToString() is mainly used for logging
    public override string ToString() => $"Error {StatusCode}: {Message} ({Details})";
}