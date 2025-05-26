using System;
using ExcelDataReader;


namespace ProjectD;

public class ExcelImportService
{
    private readonly FlightDBContext _context;

    public ExcelImportService(FlightDBContext context)
    {
        _context = context;
    }

    public async Task ImportFlights(Stream flightStream, Stream touchpointStream)
    {
        System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);


        using var reader = ExcelReaderFactory.CreateReader(flightStream);
        var result = reader.AsDataSet();
        var table = result.Tables[0];

        var flights = new Dictionary<int, Flight>();

        for (int i = 1; i < table.Rows.Count; i++)
        {
            var row = table.Rows[i];
            var flight = new Flight
            {
                Type = row[0]?.ToString(),
                Id = ParseInt(row[1]),
                TimetableId = ParseInt(row[2]),
                TrafficType = row[3]?.ToString(),
                FlightNumber = row[4]?.ToString(),
                Diverted = ParseBool(row[5]),
                Nachtvlucht = ParseBool(row[6]),
                FlightCode = row[7]?.ToString(),
                FlightCodeDescription = row[8]?.ToString(),
                FlightCodeIATA = row[9]?.ToString(),
                PublicAnnouncement = ParseBool(row[10]),
                ScheduledUTC = ParseDate(row[11]),
                ActualUTC = ParseDate(row[12]),
                ScheduledLocal = ParseDate(row[13]),
                ActualLocal = ParseDate(row[14]),
                Bewegingen = row[15]?.ToString(),
                Parkeerpositie = row[16]?.ToString(),
                // Skip row[17] → Parkeercontract
                Bus = ParseBool(row[18]),
                Gate = row[19]?.ToString(),
                Bagageband = ParseInt(row[20]),
                AirportICAO = row[21]?.ToString(),
                Airport = row[22]?.ToString(),
                Country = row[23]?.ToString(),
                ViaAirportICAO = row[24]?.ToString(),
                ViaAirport = row[25]?.ToString(),
                AircraftRegistration = row[26]?.ToString(),
                Seats = ParseInt(row[27]),
                MTOW = ParseInt(row[28]),
                AircraftType = row[29]?.ToString(),
                AircraftDescription = row[30]?.ToString(),
                EU = ParseBool(row[31]),
                Schengen = ParseBool(row[32]),
                AirlineFullname = row[33]?.ToString(),
                AirlineShortname = row[34]?.ToString(),
                AirlineICAO = row[35]?.ToString(),
                AirlineIATA = row[36]?.ToString(),
                Debiteur = row[37]?.ToString(),
                DebiteurNr = ParseInt(row[38]),
                PaxMale = ParseInt(row[39]),
                PaxFemale = ParseInt(row[40]),
                PaxChild = ParseInt(row[41]),
                PaxInfant = ParseInt(row[42]),
                PaxTransitMale = ParseInt(row[43]),
                PaxTransitFemale = ParseInt(row[44]),
                PaxTransitChild = ParseInt(row[45]),
                PaxTransitInfant = ParseInt(row[46]),
                CrewCabin = ParseInt(row[47]),
                CrewCockpit = ParseInt(row[48]),
                BagsWeight = ParseDouble(row[49]),
                BagsTransitWeight = ParseDouble(row[50]),
                Bags = ParseInt(row[51]),
                BagsTransit = ParseInt(row[52]),
                Afhandelaar = row[53]?.ToString(),
                ForecastPercentage = ParseDouble(row[54]),
                ForecastPax = ParseInt(row[55]),
                ForecastBabys = ParseInt(row[56]),
                FlightClass = row[57]?.ToString(),
                Datasource = row[58]?.ToString(),
                TotalPax = ParseInt(row[59]),
                TerminalPax = ParseInt(row[60]),
                TotalPaxBetalend = ParseInt(row[61]),
                TerminalPaxBetalend = ParseInt(row[62]),
                TransitPax = ParseInt(row[63]),
                TransitPaxBetalend = ParseInt(row[64]),
                TotalCrew = ParseInt(row[65]),
                TerminalCrew = ParseInt(row[66]),
                TotalSeats = ParseInt(row[67]),
                TerminalSeats = ParseInt(row[68]),
                TotalBags = ParseInt(row[69]),
                TerminalBags = ParseInt(row[70]),
                TransitBags = ParseInt(row[71]),
                TotalBagsWeight = ParseDouble(row[72]),
                TerminalBagsWeight = ParseDouble(row[73]),
                TransitBagsWeight = ParseDouble(row[74]),
                Runway = row[75]?.ToString(),
                Longitude = ParseDouble(row[76]),
                Elevation = ParseDouble(row[77]),
                Latitude = ParseDouble(row[78]),
                DistanceKilometers = ParseDouble(row[79]),
                Direction = row[80]?.ToString(),
                AirportIATA = row[81]?.ToString(),
                // Skip row[82] → Forecast
                Parked = ParseBool(row[83]),
                Seizoen = row[84]?.ToString(),
            };
            flights[flight.Id] = flight;
            _context.Flights.Add(flight);

        }

        using var reader2 = ExcelReaderFactory.CreateReader(touchpointStream);
        var result2 = reader2.AsDataSet();
        var table2 = result2.Tables[0];

        for (int i = 1; i < table2.Rows.Count; i++)
        {
            var row = table2.Rows[i];
            int flightId = int.Parse(row[0].ToString());

            if (flights.ContainsKey(flightId))
            {
                var touchpoint = new Touchpoint
                {
                    FlightId = flightId,
                    TouchpointType = row[10]?.ToString(),
                    TouchpointTime = ParseDate(row[11]),
                    TouchpointPax = ParseDouble(row[12]),
                };
                _context.Touchpoints.Add(touchpoint);
                flights[flightId].Touchpoints.Add(touchpoint);
            }
        }


        _context.Flights.AddRange(flights.Values);
        await _context.SaveChangesAsync();
    }


    private DateTime ParseDate(object value)
    {
        if (value is DateTime dt)
            return DateTime.SpecifyKind(dt, DateTimeKind.Utc);

        if (DateTime.TryParse(value?.ToString(), out var parsed))
            return DateTime.SpecifyKind(parsed, DateTimeKind.Utc);

        return DateTime.SpecifyKind(DateTime.MinValue, DateTimeKind.Utc);
    }

    private bool ParseBool(object value)
    {
        return value?.ToString().Trim().ToLower() switch
        {
            "true" => true,
            "false" => false,
            _ => false
        };
    }

    private int ParseInt(object value)
    {
        return int.TryParse(value?.ToString(), out var val) ? val : 0;
    }

    private double ParseDouble(object value)
    {
        var str = value?.ToString().Replace(",", ".") ?? "0";
        return double.TryParse(str, System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out var val)
            ? val
            : 0;
    }
}
