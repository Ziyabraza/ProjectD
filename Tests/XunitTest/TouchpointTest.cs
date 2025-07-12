﻿using ProjectD.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Microsoft.AspNetCore.Http;
using Microsoft.VisualStudio.TestPlatform.CommunicationUtilities;
using Microsoft.Extensions.Caching.Memory;
using System.Security.Claims;

namespace ProjectD
{

    public class TouchpointTest
    {
        // when no message is given it will give default message
        private string UnautorizeMessage() => "You must be logged in to access this resource.";
        private FlightDBContext GetInMemoryDbContext()
        {
            var options = new DbContextOptionsBuilder<FlightDBContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            var context = new FlightDBContext(options);

            // Seed with mock data
            context.Touchpoints.AddRange(
                new Touchpoint { Id = 1, FlightId = 100, TouchpointType = "Boarding", TouchpointTime = DateTime.Now, TouchpointPax = 180 },
                new Touchpoint { Id = 2, FlightId = 200, TouchpointType = "Landing", TouchpointTime = DateTime.Now, TouchpointPax = 170 }
            );
            context.SaveChanges();

            return context;
        }

        private FlightDBContext GetInMemoryDbContextEmpty()
        {
            var options = new DbContextOptionsBuilder<FlightDBContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            return new FlightDBContext(options);
        }

        private void Login(TouchpointController controller, string role = "User")
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
        private void LoginAsAnonymous(TouchpointController controller)
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
        public async Task GetByID_Unautorized_Result()
        {
            var options = new MemoryCacheOptions();
            var memoryCache = new MemoryCache(options);
            var context = GetInMemoryDbContext();
            var controller = new TouchpointController(context, memoryCache);
            LoginAsAnonymous(controller);

            var result = await controller.GetByID(100);
            Assert.IsType<UnauthorizedObjectResult>(result);
        }
        [Fact]
        public async Task GetByPage_Unautorized_Result()
        {
            var options = new MemoryCacheOptions();
            var memoryCache = new MemoryCache(options);
            var context = GetInMemoryDbContext();
            var controller = new TouchpointController(context, memoryCache);
            LoginAsAnonymous(controller);

            var result = await controller.GetByPage(1);
            Assert.IsType<UnauthorizedObjectResult>(result);
        }
        [Fact]
        public async Task GetByID_Unautorized_Message()
        {
            var options = new MemoryCacheOptions();
            var memoryCache = new MemoryCache(options);
            var context = GetInMemoryDbContext();
            var controller = new TouchpointController(context, memoryCache);
            LoginAsAnonymous(controller);

            var result = await controller.GetByID(100);
            var UNAresult = Assert.IsType<UnauthorizedObjectResult>(result);
            var error = Assert.IsType<Error>(UNAresult.Value);
            Assert.Equal(UnautorizeMessage(), error.Message);
        }
        [Fact]
        public async Task GetByPage_Unautorized_Message()
        {
            var options = new MemoryCacheOptions();
            var memoryCache = new MemoryCache(options);
            var context = GetInMemoryDbContext();
            var controller = new TouchpointController(context, memoryCache);
            LoginAsAnonymous(controller);

            var result = await controller.GetByPage(100);
            var UNAresult = Assert.IsType<UnauthorizedObjectResult>(result);
            var error = Assert.IsType<Error>(UNAresult.Value);
            Assert.Equal(UnautorizeMessage(), error.Message);
        }
        [Fact]
        public async Task GetByID_Unautorized_StatusCode()
        {
            var options = new MemoryCacheOptions();
            var memoryCache = new MemoryCache(options);
            var context = GetInMemoryDbContext();
            var controller = new TouchpointController(context, memoryCache);
            LoginAsAnonymous(controller);

            var result = await controller.GetByID(100);
            var UNAresult = Assert.IsType<UnauthorizedObjectResult>(result);
            var error = Assert.IsType<Error>(UNAresult.Value);
            Assert.Equal(401, error.StatusCode);
        }
        [Fact]
        public async Task GetByPage_Unautorized_StatusCode()
        {
            var options = new MemoryCacheOptions();
            var memoryCache = new MemoryCache(options);
            var context = GetInMemoryDbContext();
            var controller = new TouchpointController(context, memoryCache);
            LoginAsAnonymous(controller);

            var result = await controller.GetByPage(1);
            var UNAresult = Assert.IsType<UnauthorizedObjectResult>(result);
            var error = Assert.IsType<Error>(UNAresult.Value);
            Assert.Equal(401, error.StatusCode);
        }
        [Fact]
        public async Task GetByPage_Unautorized_Details()
        {
            var options = new MemoryCacheOptions();
            var memoryCache = new MemoryCache(options);
            var context = GetInMemoryDbContext();
            var controller = new TouchpointController(context, memoryCache);
            LoginAsAnonymous(controller);

            var result = await controller.GetByPage(1);
            var UNAresult = Assert.IsType<UnauthorizedObjectResult>(result);
            var error = Assert.IsType<Error>(UNAresult.Value);
            Assert.Equal(new Error(401, "api/SearchByFlightID/100", "Test Unoutorized").Details, error.Details);
        }
        [Fact]
        public async Task GetByID_Unautorized_Details()
        {
            var options = new MemoryCacheOptions();
            var memoryCache = new MemoryCache(options);
            var context = GetInMemoryDbContext();
            var controller = new TouchpointController(context, memoryCache);
            LoginAsAnonymous(controller);

            var result = await controller.GetByID(100);
            var UNAresult = Assert.IsType<UnauthorizedObjectResult>(result);
            var error = Assert.IsType<Error>(UNAresult.Value);
            Assert.Equal(new Error(401, "api/Touchpoint/page/1", "Test Unoutorized").Details, error.Details);
        }
        [Fact]
        public async Task GetByID_Returns_Ok_When_FlightId_Matches_CheckSingle()
        {
            var options = new MemoryCacheOptions();
            var memoryCache = new MemoryCache(options);
            var context = GetInMemoryDbContext();
            var controller = new TouchpointController(context, memoryCache);
            Login(controller);

            var result = await controller.GetByID(100);
            var okResult = Assert.IsType<OkObjectResult>(result);
            var list = Assert.IsType<List<Touchpoint>>(okResult.Value);

            Assert.Single(list);
        }

        [Fact]
        public async Task GetByID_Returns_Ok_When_FlightId_Matches_FlightID()
        {
            var options = new MemoryCacheOptions();
            var memoryCache = new MemoryCache(options);
            var context = GetInMemoryDbContext();
            var controller = new TouchpointController(context, memoryCache);
            Login(controller);


            var result = await controller.GetByID(100);
            var okResult = Assert.IsType<OkObjectResult>(result);
            var list = Assert.IsType<List<Touchpoint>>(okResult.Value);

            // Assert.Single(list);
            Assert.Equal(100, list[0].FlightId);
        }

        [Fact]
        public async Task GetByID_Returns_Ok_When_FlightId_Matches_TouchPointType()
        {
            var options = new MemoryCacheOptions();
            var memoryCache = new MemoryCache(options);
            var context = GetInMemoryDbContext();
            var controller = new TouchpointController(context, memoryCache);
            Login(controller);

            var result = await controller.GetByID(100);
            var okResult = Assert.IsType<OkObjectResult>(result);
            var list = Assert.IsType<List<Touchpoint>>(okResult.Value);

            Assert.Equal("Boarding", list[0].TouchpointType);
        }

        [Fact]
        public async Task GetByID_Returns_Ok_When_FlightId_Matches_TouchpointTime()
        {
            var options = new MemoryCacheOptions();
            var memoryCache = new MemoryCache(options);
            var context = GetInMemoryDbContext();
            var controller = new TouchpointController(context, memoryCache);
            Login(controller);

            var result = await controller.GetByID(100);
            var okResult = Assert.IsType<OkObjectResult>(result);
            var list = Assert.IsType<List<Touchpoint>>(okResult.Value);
            DateTime now = DateTime.Now;

            // testing with Minutes and Hour only do to miliseconds diffrences
            Assert.Equal(now.Hour, list[0].TouchpointTime.Hour);
            Assert.Equal(now.Minute, list[0].TouchpointTime.Minute);
        }

        [Fact]
        public async Task GetByID_Returns_Ok_When_FlightId_Matches_TouchpointPax()
        {
            var options = new MemoryCacheOptions();
            var memoryCache = new MemoryCache(options);
            var context = GetInMemoryDbContext();
            var controller = new TouchpointController(context, memoryCache);
            Login(controller);

            var result = await controller.GetByID(100);
            var okResult = Assert.IsType<OkObjectResult>(result);
            var list = Assert.IsType<List<Touchpoint>>(okResult.Value);

            Assert.Equal(180, list[0].TouchpointPax);
        }

        [Fact]
        public async Task GetByID_Returns_NotFound_When_No_Match()
        {
            // Arrange
            var memoryOptions = new MemoryCacheOptions();
            var memoryCache = new MemoryCache(memoryOptions);
            var options = new DbContextOptionsBuilder<FlightDBContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;
            using var emptyContext = new FlightDBContext(options);
            var controller = new TouchpointController(emptyContext, memoryCache);
            Login(controller);

            var httpContext = new DefaultHttpContext();
            controller.Request.Path = "/SearchByFlightID/2"; // Set desired path

            var result = await controller.GetByID(2); // No such ID
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
            var error = Assert.IsType<Error>(notFoundResult.Value); // check if NotFoundObjectResult has an Error object

            Assert.Equal(404, error.StatusCode); // check error.
            Assert.Equal("/SearchByFlightID/2", error.Url); // check Url.
            Assert.Contains("Not Found", error.Details); // check detail.
            Assert.False("An Error occured" == error.Message); // check if default message is NOT used.
        }

        [Fact]
        public async Task GetByPage_OkResult()
        {
            var options = new MemoryCacheOptions();
            var memoryCache = new MemoryCache(options);
            FlightDBContext context = GetInMemoryDbContext();
            TouchpointController controller = new TouchpointController(context, memoryCache);
            Login(controller);


            var result = await controller.GetByPage(1);
            var OkResult = Assert.IsType<OkObjectResult>(result);
        }

        [Fact]
        public async Task GetByPage_Returns_Ok_For_Valid_Page()
        {
            var options = new MemoryCacheOptions();
            var memoryCache = new MemoryCache(options);
            var context = GetInMemoryDbContext();
            var controller = new TouchpointController(context, memoryCache);
            Login(controller);


            var result = await controller.GetByPage(1);
            var okResult = Assert.IsType<OkObjectResult>(result);
        }
        [Fact]
        public async Task GetByPage_Returns_PageManagerTouchpoints()
        {
            var options = new MemoryCacheOptions();
            var memoryCache = new MemoryCache(options);
            FlightDBContext context = GetInMemoryDbContext();
            TouchpointController controller = new TouchpointController(context, memoryCache);
            Login(controller);
            
            var result = await controller.GetByPage(1);
            var okResult = Assert.IsType<OkObjectResult>(result);
            var message = Assert.IsType<PageManagerTouchpoints>(okResult.Value);
        }
        [Fact]
        public async Task GetByPage_Returns_NotNullMessage()
        {
            var options = new MemoryCacheOptions();
            var memoryCache = new MemoryCache(options);
            FlightDBContext context = GetInMemoryDbContext();
            TouchpointController controller = new TouchpointController(context, memoryCache);
            Login(controller);


            var result = await controller.GetByPage(1);
            var okResult = Assert.IsType<OkObjectResult>(result);
            var message = Assert.IsType<PageManagerTouchpoints>(okResult.Value);

            Assert.False(message == null); // checks if message is null
        }
        [Fact]
        public async Task GetByPage_Returns_NoNullTouchpoints()
        {
            var options = new MemoryCacheOptions();
            var memoryCache = new MemoryCache(options);
            FlightDBContext context = GetInMemoryDbContext();
            TouchpointController controller = new TouchpointController(context, memoryCache);
            Login(controller);


            var result = await controller.GetByPage(1);
            var okResult = Assert.IsType<OkObjectResult>(result);
            var message = Assert.IsType<PageManagerTouchpoints>(okResult.Value);

            Assert.False(message.Touchpoints?.Any(x => x == null)); // Checks if it does NOT contain any null's
                                                                    // example:
            /*{
            {
                Touchpoint { Id = 1, FlightId = 100, TouchpointType = "Boarding", TouchpointTime = DateTime.Now, TouchpointPax = 180 },
                Touchpoint { Id = 2, FlightId = 200, TouchpointType = "Landing", TouchpointTime = DateTime.Now, TouchpointPax = 170 },
                null,
                null,
                null
            }
            // is true because contains null's
            */
        }
        [Fact]
        public async Task GetByPage_Returns_Message()
        {
            var options = new MemoryCacheOptions();
            var memoryCache = new MemoryCache(options);
            FlightDBContext context = GetInMemoryDbContext();
            TouchpointController controller = new TouchpointController(context, memoryCache);
            Login(controller);

            var result = await controller.GetByPage(1);
            var okResult = Assert.IsType<OkObjectResult>(result);
            var message = Assert.IsType<PageManagerTouchpoints>(okResult.Value);

            Assert.False(message == null); // checks if message is null
            Assert.False(message.Touchpoints?.Any(x => x == null)); // Checks if it does NOT contain any null's
            Assert.Equal(1, message.TotalPages); // Check total pages evry 100 touchpoints should have 1 page should only have 1 page
            Assert.Equal(2, message.Touchpoints.Length);
            Assert.Equal(2, message.TotalRecords);
        }


        [Fact]
        public async Task GetByPage_Returns_Redirect()
        {
            var options = new MemoryCacheOptions();
            var memoryCache = new MemoryCache(options);
            var context = GetInMemoryDbContext();
            var controller = new TouchpointController(context, memoryCache);
            Login(controller);
            var httpContext = new DefaultHttpContext();
            controller.Request.Path = "/api/page/2/"; // Set desired path


            var result = await controller.GetByPage(2);
            var okResult = Assert.IsType<RedirectResult>(result);
            Assert.Equal("1", okResult.Url); // Redirect URL result
        }

        [Fact]
        public async Task GetByPage_Returns_NotFound_When_Empty_Database()
        {
            // Arrange
            var options = new MemoryCacheOptions();
            var memoryCache = new MemoryCache(options);
            FlightDBContext emptyContext = GetInMemoryDbContextEmpty();
            TouchpointController controller = new TouchpointController(emptyContext, memoryCache);
            Login(controller);
            var httpContext = new DefaultHttpContext();
            controller.Request.Path = "/api/page/1"; // Set desired path

            var result = await controller.GetByPage(1);
            Assert.IsType<NotFoundObjectResult>(result);
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
            var error = Assert.IsType<Error>(notFoundResult.Value); // check if NotFoundObjectResult has an Error object

            // Now assert on the Error object
            Assert.Equal(404, error.StatusCode); // check error.
            Assert.Equal("/api/page/1", error.Url); // check Url.
            Assert.Contains("not found", error.Details.ToLower()); // check detail.
            Assert.NotEqual("An Error occured", error.Message); // check if default message is NOT used.
            Assert.Equal("An error occured. There are no Touchpoints found make contact with Webprovider if its ongoing issue. Sorry for inconvinence.", error.Message);
        }

        [Fact]
        public async Task GetByPage_Returns_NotFound_When_Empty_Database_Status()
        {
            // Arrange
            var options = new MemoryCacheOptions();
            var memoryCache = new MemoryCache(options);
            FlightDBContext emptyContext = GetInMemoryDbContextEmpty();
            TouchpointController controller = new TouchpointController(emptyContext, memoryCache);
            Login(controller);
            controller.Request.Path = "/api/page/1"; // Set desired path

            var result = await controller.GetByPage(1);
            Assert.IsType<NotFoundObjectResult>(result);
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
            var error = Assert.IsType<Error>(notFoundResult.Value); // check if NotFoundObjectResult has an Error object

            Assert.Equal(404, error.StatusCode); // check error.
        }

        [Fact]
        public async Task GetByPage_Returns_NotFound_When_Empty_Database_Url()
        {
            // Arrange
            var options = new MemoryCacheOptions();
            var memoryCache = new MemoryCache(options);
            FlightDBContext emptyContext = GetInMemoryDbContextEmpty();
            TouchpointController controller = new TouchpointController(emptyContext, memoryCache);
            Login(controller);
            controller.Request.Path = "/api/page/1"; // Set desired path

            var result = await controller.GetByPage(1);
            Assert.IsType<NotFoundObjectResult>(result);
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
            var error = Assert.IsType<Error>(notFoundResult.Value); // check if NotFoundObjectResult has an Error object

            Assert.Equal("/api/page/1", error.Url); // check Url.

        }

        [Fact]
        public async Task GetByPage_Returns_NotFound_When_Empty_Database_Detail()
        {
            // Arrange
            var options = new MemoryCacheOptions();
            var memoryCache = new MemoryCache(options);
            FlightDBContext emptyContext = GetInMemoryDbContextEmpty();
            TouchpointController controller = new TouchpointController(emptyContext, memoryCache);
            Login(controller);
            var httpContext = new DefaultHttpContext();
            controller.Request.Path = "/api/page/1"; // Set desired path

            var result = await controller.GetByPage(1);
            Assert.IsType<NotFoundObjectResult>(result);
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
            var error = Assert.IsType<Error>(notFoundResult.Value); // check if NotFoundObjectResult has an Error object

            // Now assert on the Error object
            // Assert.Equal(404, error.StatusCode); // check error.
            // Assert.Equal("/api/page/1", error.Url); // check Url.
            Assert.Contains("not found", error.Details.ToLower()); // check detail.
            // Assert.NotEqual("An Error occured", error.Message); // check if default message is NOT used.
            // Assert.Equal("An error occured. There are no Touchpoints found make contact with Webprovider if its ongoing issue. Sorry for inconvinence.", error.Message);
        }
        [Fact]
        public async Task GetByPage_Returns_NotFound_When_Empty_Database_Message_NotDeffault()
        {
            // Arrange
            var options = new MemoryCacheOptions();
            var memoryCache = new MemoryCache(options);
            FlightDBContext emptyContext = GetInMemoryDbContextEmpty();
            TouchpointController controller = new TouchpointController(emptyContext, memoryCache);
            Login(controller);
            var httpContext = new DefaultHttpContext();
            controller.Request.Path = "/api/page/1"; // Set desired path

            var result = await controller.GetByPage(1);
            Assert.IsType<NotFoundObjectResult>(result);
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
            var error = Assert.IsType<Error>(notFoundResult.Value); // check if NotFoundObjectResult has an Error object

            Assert.NotEqual(ErrorTest.ErrorDefaultMessage(), error.Message); // check if default message is NOT used.
        }
        [Fact]
        public async Task GetByPage_Returns_NotFound_When_Empty_Database_CorrectMessage()
        {
            // Arrange
            var options = new MemoryCacheOptions();
            var memoryCache = new MemoryCache(options);
            FlightDBContext emptyContext = GetInMemoryDbContextEmpty();
            TouchpointController controller = new TouchpointController(emptyContext, memoryCache);
            Login(controller);
            var httpContext = new DefaultHttpContext();
            controller.Request.Path = "/api/page/1"; // Set desired path

            var result = await controller.GetByPage(1);
            Assert.IsType<NotFoundObjectResult>(result);
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
            var error = Assert.IsType<Error>(notFoundResult.Value); // check if NotFoundObjectResult has an Error object

            Assert.Equal("An error occured. There are no Touchpoints found make contact with Webprovider if its ongoing issue. Sorry for inconvinence.", error.Message);
        }

        [Fact]
        public async Task GetByID_Returns_From_Cache_When_Available()
        {
            // Arrange
            var context = GetInMemoryDbContext();
            var cache = new MemoryCache(new MemoryCacheOptions());
            var controller = new TouchpointController(context, cache);
            Login(controller);


            string cacheKey = "user:authorized:touchpoints:SearchByFlightID:1001";

            // Simulate data in cache
            var expected = new List<Touchpoint> { new Touchpoint { Id = 99, FlightId = 1001 } };
            cache.Set(cacheKey, expected);

            // Act
            var result = await controller.GetByID(1001);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var data = Assert.IsAssignableFrom<List<Touchpoint>>(okResult.Value);
            Assert.Equal(99, data[0].Id); // From cache, not DB
        }

        [Fact]
        public async Task GetByID_Caches_And_Returns_Data_When_Not_In_Cache()
        {
            // Arrange
            var context = GetInMemoryDbContext();
            var cache = new MemoryCache(new MemoryCacheOptions());
            var controller = new TouchpointController(context, cache);
            Login(controller);

            string cacheKey = "user:authorized:touchpoints:SearchByFlightID:200";

            // Ensure cache is empty
            Assert.False(cache.TryGetValue(cacheKey, out _));

            // Act
            var result = await controller.GetByID(200);

            // Assert result is OK and data is returned
            var okResult = Assert.IsType<OkObjectResult>(result);
            var data = Assert.IsAssignableFrom<List<Touchpoint>>(okResult.Value);
            Assert.Single(data);
            Assert.Equal(200, data[0].FlightId);

            // Cache should now contain it
            Assert.True(cache.TryGetValue(cacheKey, out var cached));
            var cachedList = Assert.IsAssignableFrom<List<Touchpoint>>(cached);
            Assert.Equal(200, cachedList[0].FlightId);
        }

        [Fact]
        public async Task GetByID_Caches_Error_When_No_Data()
        {
            // Arrange
            var context = GetInMemoryDbContext();
            var cache = new MemoryCache(new MemoryCacheOptions());
            var controller = new TouchpointController(context, cache);
            Login(controller);

            string cacheKey = "user:authorized:touchpoints:SearchByFlightID:9999";

            // Act
            var result = await controller.GetByID(9999);

            // Assert result is NotFound with Error
            var notFound = Assert.IsType<NotFoundObjectResult>(result);
            var error = Assert.IsAssignableFrom<Error>(notFound.Value);
            Assert.Contains("No touchpoints found", error.Message);

            // Cache should now contain the error
            Assert.True(cache.TryGetValue(cacheKey, out var cached));
            Assert.IsType<Error>(cached);
        }
        [Fact]
        public async Task GetByPage_Returns_From_Cache_When_Available()
        {
            // Arrange
            var context = GetInMemoryDbContext();
            var cache = new MemoryCache(new MemoryCacheOptions());
            var controller = new TouchpointController(context, cache);
            Login(controller);

            int testPage = 1;
            string cacheKey = $"user:authorized:touchpoints:page:{testPage}";

            var cachedPage = new PageManagerTouchpoints(testPage, 1, new[]
            {
                new Touchpoint { Id = 1, FlightId = 123 }
            });

            cache.Set(cacheKey, cachedPage);

            // Act
            var result = await controller.GetByPage(testPage);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var pageResult = Assert.IsAssignableFrom<PageManagerTouchpoints>(okResult.Value);
            Assert.Equal(1, pageResult.PageNumber);
            Assert.Single(pageResult.Touchpoints);
        }
    }
}