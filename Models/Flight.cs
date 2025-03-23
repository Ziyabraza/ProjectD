using System;

namespace ProjectD;

public class Flight
{
    public string Type { get; set; }
    public int Id { get; set; }
    public int TimetableId { get; set; }
    public string TrafficType { get; set; }
    public string FlightNumber { get; set; }
    public bool Diverted { get; set; }
    public bool Nachtvlucht { get; set; }
    public string FlightCode { get; set; }
    public string FlightCodeDescription { get; set; }
    public string FlightCodeIATA { get; set; }
    public bool PublicAnnouncement { get; set; }

    public DateTime ScheduledUTC { get; set; }
    public DateTime ActualUTC { get; set; }
    public DateTime ScheduledLocal { get; set; }
    public DateTime ActualLocal { get; set; }

    public string Bewegingen { get; set; }
    public string Parkeerpositie { get; set; }
    public bool Bus { get; set; }
    public string Gate { get; set; }
    public int Bagageband { get; set; }

    public string AirportICAO { get; set; }
    public string Airport { get; set; }
    public string Country { get; set; }
    public string? ViaAirportICAO { get; set; }
    public string? ViaAirport { get; set; }

    public string AircraftRegistration { get; set; }
    public int Seats { get; set; }
    public int MTOW { get; set; }
    public string AircraftType { get; set; }
    public string AircraftDescription { get; set; }

    public bool EU { get; set; }
    public bool Schengen { get; set; }

    public string AirlineFullname { get; set; }
    public string AirlineShortname { get; set; }
    public string AirlineICAO { get; set; }
    public string AirlineIATA { get; set; }

    public string Debiteur { get; set; }
    public int DebiteurNr { get; set; }

    public int PaxMale { get; set; }
    public int PaxFemale { get; set; }
    public int PaxChild { get; set; }
    public int PaxInfant { get; set; }
    public int PaxTransitMale { get; set; }
    public int PaxTransitFemale { get; set; }
    public int PaxTransitChild { get; set; }
    public int PaxTransitInfant { get; set; }

    public int CrewCabin { get; set; }
    public int CrewCockpit { get; set; }

    public double BagsWeight { get; set; }
    public double BagsTransitWeight { get; set; }
    public int Bags { get; set; }
    public int BagsTransit { get; set; }

    public string Afhandelaar { get; set; }

    public double ForecastPercentage { get; set; }
    public int ForecastPax { get; set; }
    public int ForecastBabys { get; set; }

    public string FlightClass { get; set; }
    public string Datasource { get; set; }

    public int TotalPax { get; set; }
    public int TerminalPax { get; set; }
    public int TotalPaxBetalend { get; set; }
    public int TerminalPaxBetalend { get; set; }
    public int TransitPax { get; set; }
    public int TransitPaxBetalend { get; set; }

    public int TotalCrew { get; set; }
    public int TerminalCrew { get; set; }

    public int TotalSeats { get; set; }
    public int TerminalSeats { get; set; }

    public int TotalBags { get; set; }
    public int TerminalBags { get; set; }
    public int TransitBags { get; set; }

    public double TotalBagsWeight { get; set; }
    public double TerminalBagsWeight { get; set; }
    public double TransitBagsWeight { get; set; }

    public string Runway { get; set; }
    public double Longitude { get; set; }
    public double Elevation { get; set; }
    public double Latitude { get; set; }
    public double DistanceKilometers { get; set; }

    public string Direction { get; set; }
    public string AirportIATA { get; set; }

    public bool Parked { get; set; }
    public string Seizoen { get; set; }

    public ICollection<Touchpoint> Touchpoints { get; set; } = new List<Touchpoint>();
}
