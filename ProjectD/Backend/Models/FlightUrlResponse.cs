using Swashbuckle.AspNetCore.Annotations;

public class FlightUrlResponse
{
    public int FlightId { get; set; }

    [SwaggerSchema(Format = "uri", Description = "Klikbare link naar de vluchtinformatie")]
    public string Url { get; set; }
}