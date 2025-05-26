namespace ProjectD.Models;

public class StressTester
{
    public Touchpoint[] Touchpoints { get; set; }
    public Flight[] Flights { get; set; }
    public StressTester(Touchpoint[] touchpoints, Flight[] flights)
    {
        Touchpoints = touchpoints; Flights = flights;
    }
}