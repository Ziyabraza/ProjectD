using Xunit;
using Moq;
using Microsoft.EntityFrameworkCore;
using ProjectD;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using Microsoft.AspNetCore.Http;
using System.Linq;
using Microsoft.Extensions.Caching.Memory;
using ProjectD.Models;
using System.Security.Claims;

namespace ProjectD
{
    public class FlightControllerTests
    {
        private string UnautorizeMessage() => "You must be logged in to access this resource.";
        private FlightDBContext GetDbContext(string dbName, int aantalVluchten = 1)
        {
            var options = new DbContextOptionsBuilder<FlightDBContext>()
                .UseInMemoryDatabase(databaseName: dbName)
                .Options;

            var context = new FlightDBContext(options);

            if (aantalVluchten > 0)
            {
                var vluchten = new List<Flight>();

                if (aantalVluchten >= 1)
                {
                    vluchten.Add(new Flight
                    {
                        Id = 1,
                        Type = "Arrival",
                        TimetableId = 101,
                        TrafficType = "Commercial",
                        FlightNumber = "KL1234",
                        Diverted = false,
                        Nachtvlucht = true,
                        FlightCode = "KL1234A",
                        FlightCodeDescription = "KLM Amsterdam Arrival",
                        FlightCodeIATA = "KL",
                        PublicAnnouncement = true,
                        ScheduledUTC = DateTime.UtcNow.AddHours(-1),
                        ActualUTC = DateTime.UtcNow,
                        ScheduledLocal = DateTime.Now.AddHours(-1),
                        ActualLocal = DateTime.Now,
                        Bewegingen = "Landing",
                        Parkeerpositie = "B32",
                        Bus = false,
                        Gate = "G5",
                        Bagageband = 3,
                        AirportICAO = "EHAM",
                        Airport = "Amsterdam Schiphol",
                        Country = "Netherlands",
                        ViaAirportICAO = null,
                        ViaAirport = null,
                        AircraftRegistration = "PH-BFW",
                        Seats = 180,
                        MTOW = 70000,
                        AircraftType = "Boeing 737",
                        AircraftDescription = "Boeing 737-800",
                        EU = true,
                        Schengen = true,
                        AirlineFullname = "KLM Royal Dutch Airlines",
                        AirlineShortname = "KLM",
                        AirlineICAO = "KLM",
                        AirlineIATA = "KL",
                        Debiteur = "Schiphol BV",
                        DebiteurNr = 1001,
                        PaxMale = 60,
                        PaxFemale = 65,
                        PaxChild = 10,
                        PaxInfant = 5,
                        PaxTransitMale = 5,
                        PaxTransitFemale = 6,
                        PaxTransitChild = 1,
                        PaxTransitInfant = 0,
                        CrewCabin = 4,
                        CrewCockpit = 2,
                        BagsWeight = 1230.5,
                        BagsTransitWeight = 230.0,
                        Bags = 95,
                        BagsTransit = 15,
                        Afhandelaar = "Swissport",
                        ForecastPercentage = 87.5,
                        ForecastPax = 160,
                        ForecastBabys = 3,
                        FlightClass = "Economy",
                        Datasource = "API",
                        TotalPax = 140,
                        TerminalPax = 130,
                        TotalPaxBetalend = 120,
                        TerminalPaxBetalend = 110,
                        TransitPax = 12,
                        TransitPaxBetalend = 10,
                        TotalCrew = 6,
                        TerminalCrew = 5,
                        TotalSeats = 180,
                        TerminalSeats = 150,
                        TotalBags = 110,
                        TerminalBags = 100,
                        TransitBags = 10,
                        TotalBagsWeight = 1460.5,
                        TerminalBagsWeight = 1230.5,
                        TransitBagsWeight = 230.0,
                        Runway = "18C",
                        Longitude = 4.7634,
                        Elevation = 3.4,
                        Latitude = 52.3086,
                        DistanceKilometers = 1280.0,
                        Direction = "NW",
                        AirportIATA = "AMS",
                        Parked = true,
                        Seizoen = "Zomer"
                    });
                }
                if (aantalVluchten >= 2)
                {
                    vluchten.Add(new Flight
                    {
                        Id = 2,
                        Type = "Departure",
                        TimetableId = 1002,
                        TrafficType = "Commercial",
                        FlightNumber = "LH456",
                        Diverted = false,
                        Nachtvlucht = false,
                        FlightCode = "LH456D",
                        FlightCodeDescription = "Lufthansa to Frankfurt",
                        FlightCodeIATA = "LH",
                        PublicAnnouncement = true,
                        ScheduledUTC = DateTime.UtcNow.AddMinutes(30),
                        ActualUTC = DateTime.UtcNow.AddMinutes(45),
                        ScheduledLocal = DateTime.Now.AddMinutes(30),
                        ActualLocal = DateTime.Now.AddMinutes(45),
                        Bewegingen = "Takeoff",
                        Parkeerpositie = "D22",
                        Bus = true,
                        Gate = "F8",
                        Bagageband = 0,
                        AirportICAO = "EDDF",
                        Airport = "Frankfurt Main",
                        Country = "Germany",
                        ViaAirportICAO = "EDDM",
                        ViaAirport = "Munich",
                        AircraftRegistration = "D-AINX",
                        Seats = 180,
                        MTOW = 78000,
                        AircraftType = "Airbus A320",
                        AircraftDescription = "Airbus A320neo",
                        EU = true,
                        Schengen = true,
                        AirlineFullname = "Lufthansa German Airlines",
                        AirlineShortname = "Lufthansa",
                        AirlineICAO = "DLH",
                        AirlineIATA = "LH",
                        Debiteur = "Lufthansa Group",
                        DebiteurNr = 202,
                        PaxMale = 72,
                        PaxFemale = 68,
                        PaxChild = 14,
                        PaxInfant = 3,
                        PaxTransitMale = 0,
                        PaxTransitFemale = 1,
                        PaxTransitChild = 1,
                        PaxTransitInfant = 0,
                        CrewCabin = 6,
                        CrewCockpit = 2,
                        BagsWeight = 1495.8,
                        BagsTransitWeight = 100.0,
                        Bags = 102,
                        BagsTransit = 5,
                        Afhandelaar = "Aviapartner",
                        ForecastPercentage = 90.2,
                        ForecastPax = 165,
                        ForecastBabys = 2,
                        FlightClass = "Business",
                        Datasource = "ManualEntry",
                        TotalPax = 157,
                        TerminalPax = 155,
                        TotalPaxBetalend = 150,
                        TerminalPaxBetalend = 148,
                        TransitPax = 2,
                        TransitPaxBetalend = 1,
                        TotalCrew = 8,
                        TerminalCrew = 8,
                        TotalSeats = 180,
                        TerminalSeats = 170,
                        TotalBags = 107,
                        TerminalBags = 102,
                        TransitBags = 5,
                        TotalBagsWeight = 1595.8,
                        TerminalBagsWeight = 1495.8,
                        TransitBagsWeight = 100.0,
                        Runway = "24",
                        Longitude = 8.5706,
                        Elevation = 111.0,
                        Latitude = 50.0333,
                        DistanceKilometers = 420.0,
                        Direction = "SE",
                        AirportIATA = "FRA",
                        Parked = false,
                        Seizoen = "Herfst"
                    });
                }
                context.Flights.AddRange(vluchten);
                context.SaveChanges();
            }
            return context;
        }

        private void Login(FlightController controller, string role = "User")
        {
            controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext
                {
                    User = new ClaimsPrincipal(new ClaimsIdentity(new[]
                    {
                        new Claim(ClaimTypes.Name, "TestUser"),
                        new Claim(ClaimTypes.Role, role)
                    }, "mock"))
                }
            };
        }
        private void LoginAsAnonymous(FlightController controller)
        {
            controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext
                {
                    User = new ClaimsPrincipal() // No identity = anonymous
                }
            };
        }
        [Fact]
        public async Task GetFlightById_Unauthorized_Result()
        {
            var options = new MemoryCacheOptions();
            var memoryCache = new MemoryCache(options);
            var context = GetDbContext(Guid.NewGuid().ToString());
            var controller = new FlightController(context, memoryCache);
            LoginAsAnonymous(controller);

            var result = await controller.GetFlightById(100);
            Assert.IsType<UnauthorizedObjectResult>(result.Result);
        }
        [Fact]
        public async Task GetFlightsWithID_Unauthorized_Result()
        {
            var options = new MemoryCacheOptions();
            var memoryCache = new MemoryCache(options);
            var context = GetDbContext(Guid.NewGuid().ToString());
            var controller = new FlightController(context, memoryCache);
            LoginAsAnonymous(controller);

            var result = await controller.GetFlightsWithID();
            Assert.IsType<UnauthorizedObjectResult>(result.Result); // ActionResult<Dictionary<int, string>>
        }
        [Fact]
        public async Task FilterFlights_Unauthorized_Result()
        {
            var options = new MemoryCacheOptions();
            var memoryCache = new MemoryCache(options);
            var context = GetDbContext(Guid.NewGuid().ToString());
            var controller = new FlightController(context, memoryCache);
            LoginAsAnonymous(controller);

            var result = await controller.FilterFlights(null, null, null, 1);
            Assert.IsType<UnauthorizedObjectResult>(result); // IActionResult
        }
        [Fact]
        public async Task GetFlightById_Unautorized_Message()
        {
            var options = new MemoryCacheOptions();
            var memoryCache = new MemoryCache(options);
            var context = GetDbContext(Guid.NewGuid().ToString());
            var controller = new FlightController(context, memoryCache);
            LoginAsAnonymous(controller);

            var result = await controller.GetFlightById(100);
            var UNAresult = Assert.IsType<UnauthorizedObjectResult>(result.Result);
            var error = Assert.IsType<Error>(UNAresult.Value);
            Assert.Equal(UnautorizeMessage(), error.Message);
        }
        [Fact]
        public async Task GetFlightsWithID_Unautorized_Message()
        {
            var options = new MemoryCacheOptions();
            var memoryCache = new MemoryCache(options);
            var context = GetDbContext(Guid.NewGuid().ToString());
            var controller = new FlightController(context, memoryCache);
            LoginAsAnonymous(controller);

            var result = await controller.GetFlightsWithID();
            var UNAresult = Assert.IsType<UnauthorizedObjectResult>(result.Result);
            var error = Assert.IsType<Error>(UNAresult.Value);
            Assert.Equal(UnautorizeMessage(), error.Message);
        }
        [Fact]
        public async Task FilterFlights_Unautorized_Message()
        {
            var options = new MemoryCacheOptions();
            var memoryCache = new MemoryCache(options);
            var context = GetDbContext(Guid.NewGuid().ToString());
            var controller = new FlightController(context, memoryCache);
            LoginAsAnonymous(controller);

            var result = await controller.FilterFlights(null, null, null, 1);
            var UNAresult = Assert.IsType<UnauthorizedObjectResult>(result);
            var error = Assert.IsType<Error>(UNAresult.Value);
            Assert.Equal(UnautorizeMessage(), error.Message);
        }
        [Fact]
        public async Task GetFlightById_Unautorized_StatusCode()
        {
            var options = new MemoryCacheOptions();
            var memoryCache = new MemoryCache(options);
            var context = GetDbContext(Guid.NewGuid().ToString());
            var controller = new FlightController(context, memoryCache);
            LoginAsAnonymous(controller);

            var result = await controller.GetFlightById(100);
            var UNAresult = Assert.IsType<UnauthorizedObjectResult>(result.Result);
            var error = Assert.IsType<Error>(UNAresult.Value);
            Assert.Equal(401, error.StatusCode);
        }
        [Fact]
        public async Task FilterFlights_Unautorized_StatusCode()
        {
            var options = new MemoryCacheOptions();
            var memoryCache = new MemoryCache(options);
            var context = GetDbContext(Guid.NewGuid().ToString());
            var controller = new FlightController(context, memoryCache);
            LoginAsAnonymous(controller);

            var result = await controller.FilterFlights(null, null, null, 1);
            var UNAresult = Assert.IsType<UnauthorizedObjectResult>(result);
            var error = Assert.IsType<Error>(UNAresult.Value);
            Assert.Equal(401, error.StatusCode);
        }
        [Fact]
        public async Task GetFlightWithID_Unautorized_StatusCode()
        {
            var options = new MemoryCacheOptions();
            var memoryCache = new MemoryCache(options);
            var context = GetDbContext(Guid.NewGuid().ToString());
            var controller = new FlightController(context, memoryCache);
            LoginAsAnonymous(controller);

            var result = await controller.GetFlightsWithID();
            var UNAresult = Assert.IsType<UnauthorizedObjectResult>(result.Result);
            var error = Assert.IsType<Error>(UNAresult.Value);
            Assert.Equal(401, error.StatusCode);
        }
        [Fact]
        public async Task GetFlightById_Unautorized_Details()
        {
            var options = new MemoryCacheOptions();
            var memoryCache = new MemoryCache(options);
            var context = GetDbContext(Guid.NewGuid().ToString());
            var controller = new FlightController(context, memoryCache);
            LoginAsAnonymous(controller);

            var result = await controller.GetFlightById(100);
            var UNAresult = Assert.IsType<UnauthorizedObjectResult>(result.Result);
            var error = Assert.IsType<Error>(UNAresult.Value);
            Assert.Equal(new Error(401, "api/FlightController/100", "Test Unoutorized").Details, error.Details);
        }
        [Fact]
        public async Task GetFlightWithId_Unautorized_Details()
        {
            var options = new MemoryCacheOptions();
            var memoryCache = new MemoryCache(options);
            var context = GetDbContext(Guid.NewGuid().ToString());
            var controller = new FlightController(context, memoryCache);
            LoginAsAnonymous(controller);

            var result = await controller.GetFlightsWithID();
            var UNAresult = Assert.IsType<UnauthorizedObjectResult>(result.Result);
            var error = Assert.IsType<Error>(UNAresult.Value);
            Assert.Equal(new Error(401, "api/flight", "Test Unoutorized").Details, error.Details);
        }
        [Fact]
        public async Task FilterFlights_Unautorized_Details()
        {
            var options = new MemoryCacheOptions();
            var memoryCache = new MemoryCache(options);
            var context = GetDbContext(Guid.NewGuid().ToString());
            var controller = new FlightController(context, memoryCache);
            LoginAsAnonymous(controller);

            var result = await controller.FilterFlights(null, null, null, 1);
            var UNAresult = Assert.IsType<UnauthorizedObjectResult>(result);
            var error = Assert.IsType<Error>(UNAresult.Value);
            Assert.Equal(new Error(401, "api/flight", "Test Unoutorized").Details, error.Details);
        }
        private FlightController GetControllerWithMockedRequest(FlightDBContext context)
        {
            var options = new MemoryCacheOptions();
            var memoryCache = new MemoryCache(options);
            var controller = new FlightController(context, memoryCache);

            // Mock Request data
            var httpContext = new DefaultHttpContext();
            httpContext.Request.Scheme = "https";
            httpContext.Request.Host = new HostString("localhost");

            controller.ControllerContext = new ControllerContext()
            {
                HttpContext = httpContext
            };

            return controller;
        }

        [Fact]
        public async Task GetFlightById_ReturnsFlight_WhenExists()
        {
            // Arrange
            var options = new MemoryCacheOptions();
            var memoryCache = new MemoryCache(options);
            var context = GetDbContext(Guid.NewGuid().ToString(), aantalVluchten: 1);
            var controller = new FlightController(context, memoryCache);
            Login(controller);

            // Act
            var result = await controller.GetFlightById(1);
            Console.WriteLine("API call completed.");

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            Console.WriteLine("Asserted result is OkObjectResult.");
            var flight = Assert.IsType<Flight>(okResult.Value);
            Console.WriteLine($"Asserted result is of type Flight. ID = {flight.Id}");
            Assert.Equal(1, flight.Id);
            Console.WriteLine("Flight ID matched expected value (1). ✅ Test SUCCESS.");
        }

        [Fact]
        public async Task GetFlightById_ReturnsNotFound_WhenMissing()
        {
            var options = new MemoryCacheOptions();
            var memoryCache = new MemoryCache(options);
            var context = GetDbContext(Guid.NewGuid().ToString(), aantalVluchten: 1);
            var controller = new FlightController(context, memoryCache);
            Login(controller);

            var result = await controller.GetFlightById(999);
            Console.WriteLine("API call completed for missing ID.");


            Assert.IsType<NotFoundObjectResult>(result.Result);
            Console.WriteLine("Asserted result is NotFoundObjectResult. ✅ Test SUCCESS.");

        }
        [Fact]
        public async Task GetFlightsWithID_ReturnsDictionary_WhenFlightsExist()
        {
            Console.WriteLine("Running: GetFlightsWithID_ReturnsDictionary_WhenFlightsExist");

            var context = GetDbContext(Guid.NewGuid().ToString(), aantalVluchten: 2);
            var controller = GetControllerWithMockedRequest(context);
            Login(controller);
            controller.Request.Scheme = "https";
            controller.Request.Host = new HostString("localhost");

            Console.WriteLine("Calling GetFlightsWithID...");
            var result = await controller.GetFlightsWithID();
            Console.WriteLine("Result type: " + result.Result?.GetType().Name);


            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            Console.WriteLine("Asserted result is OkObjectResult.");

            var dictionary = Assert.IsType<Dictionary<int, string>>(okResult.Value);
            Console.WriteLine($"Asserted result is Dictionary<int, string> with {dictionary.Count} items.");

            Assert.Equal(2, dictionary.Count);
            Console.WriteLine("Dictionary count is correct (2).");

            Assert.Contains("https://localhost/api/flight/1", dictionary.Values);
            Assert.Contains("https://localhost/api/flight/2", dictionary.Values);
            Console.WriteLine("Both expected URLs are present in the dictionary. ✅ Test SUCCESS.");
        }

        [Fact]
        public async Task GetFlightsWithID_ReturnsNotFound_WhenNoFlightsExist()
        {
            Console.WriteLine("Running: GetFlightsWithID_ReturnsNotFound_WhenNoFlightsExist");
            var context = GetDbContext(Guid.NewGuid().ToString(), aantalVluchten: 0);
            var controller = GetControllerWithMockedRequest(context);
            Login(controller);

            Console.WriteLine("Calling GetFlightsWithID...");
            var result = await controller.GetFlightsWithID();
            Console.WriteLine("Result type: " + result.Result?.GetType().Name);


            var notFound = Assert.IsType<NotFoundObjectResult>(result.Result);
            Console.WriteLine("Asserted result is NotFoundObjectResult.");

            var error = Assert.IsType<Error>(notFound.Value);
            Console.WriteLine($"Asserted error object with status code: {error.StatusCode}");

            Assert.Equal(404, error.StatusCode);
            Console.WriteLine("Error status code is 404. ✅ Test SUCCESS.");
        }

        [Fact]
        public void GetFlightsWithID_HasAuthorize_Attribute_With_AdminRole()
        {
            Console.WriteLine("Running: GetFlightsWithID_HasAuthorize_Attribute_With_AdminRole");
            //check if method has [Authorize(Roles = "Admin")]
            var method = typeof(FlightController).GetMethod("GetFlightsWithID");
            var attr = method.GetCustomAttributes(typeof(Microsoft.AspNetCore.Authorization.AuthorizeAttribute), true)
                            .Cast<Microsoft.AspNetCore.Authorization.AuthorizeAttribute>()
                            .FirstOrDefault();

            Assert.NotNull(attr);
            Console.WriteLine("Authorize attribute is present.");

            Assert.Equal("Admin", attr.Roles);
            Console.WriteLine("Authorize role is 'Admin'. ✅ Test SUCCESS.");
        }

        [Fact]
        public async Task GetFlightById_ReturnsCachedFlight()
        {
            // Arrange
            var cache = new MemoryCache(new MemoryCacheOptions());
            var context = GetDbContext(Guid.NewGuid().ToString());

            var expectedFlight = new Flight { Id = 123, Country = "USA", ScheduledLocal = DateTime.UtcNow };
            string cacheKey = "user:authorized:flights:123";
            cache.Set(cacheKey, expectedFlight);

            var controller = new FlightController(context, cache);
            Login(controller);
            controller.Request.Scheme = "https";
            controller.Request.Host = new HostString("localhost");
            

            // Act
            var result = await controller.GetFlightById(123);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var flight = Assert.IsType<Flight>(okResult.Value);
            Assert.Equal(123, flight.Id);
        }

        [Fact]
        public async Task GetFlightById_CachesAndReturnsFlight_WhenNotInCache()
        {
            // Arrange
            var cache = new MemoryCache(new MemoryCacheOptions());
            var context = GetDbContext(Guid.NewGuid().ToString());

            var controller = new FlightController(context, cache);
            Login(controller);

            // Act
            var result = await controller.GetFlightById(1);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var flight = Assert.IsType<Flight>(okResult.Value);
            Assert.Equal(1, flight.Id);

            // Check if cached
            Assert.True(cache.TryGetValue("user:authorized:flights:1", out Flight _));
        }

        [Fact]
        public async Task GetFlightById_ReturnsCachedError_WhenCached()
        {
            // Arrange
            var cache = new MemoryCache(new MemoryCacheOptions());
            var context = GetDbContext(Guid.NewGuid().ToString());


            var error = new Error(404, "/api/flight/404", "Not found");
            string cacheKey = "user:authorized:flights:404";
            cache.Set(cacheKey, error);

            var controller = new FlightController(context, cache);
            Login(controller);

            // Act
            var result = await controller.GetFlightById(404);

            // Assert
            var notFound = Assert.IsType<NotFoundObjectResult>(result.Result);
            var returnedError = Assert.IsType<Error>(notFound.Value);
            Assert.Equal(404, returnedError.StatusCode);
        }
        [Fact]
        public async Task FilterFlights_CachesTheResultAfterExecution()
        {
            // Arrange
            var cache = new MemoryCache(new MemoryCacheOptions());
            var context = GetDbContext(Guid.NewGuid().ToString()); // bevat mockdata (bijv. 1 vlucht)

            var controller = new FlightController(context, cache);
            Login(controller);
            
            // Act
            var result = await controller.FilterFlights(null, null, null); // geen filters, dus alle vluchten

            // Assert
            var ok = Assert.IsType<OkObjectResult>(result);
            var page = Assert.IsType<PageManagerFlights>(ok.Value);

            // Controleren dat cache is gevuld
            var cacheKey = "user:authorized:flights:filter:id:?:date:?:country:?:page:1";
            Assert.True(cache.TryGetValue(cacheKey, out PageManagerFlights cachedPage));

            Assert.Equal(page.Flights.Length, cachedPage.TotalItems);
        }
    }
}
