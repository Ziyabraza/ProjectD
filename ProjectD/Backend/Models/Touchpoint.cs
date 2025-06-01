using System;

namespace ProjectD
{
    public class Touchpoint
    {
        public int Id { get; set; }
        public int FlightId { get; set; }
        public Flight Flight { get; set; }
        public string TouchpointType { get; set; }
        public DateTime TouchpointTime { get; set; }
        public double TouchpointPax { get; set; }
    }
}
