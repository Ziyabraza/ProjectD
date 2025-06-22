﻿using ProjectD.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Microsoft.AspNetCore.Http;
using Microsoft.VisualStudio.TestPlatform.CommunicationUtilities;

namespace ProjectD
{
    
    public class TouchpointTest
    {
        // when no message is given it will give default message
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

        [Fact]
        public async Task GetByID_Returns_Ok_When_FlightId_Matches_CheckSingle()
        {
            var context = GetInMemoryDbContext();
            var controller = new TouchpointController(context);
            controller.ControllerContext = new ControllerContext()
            {
                HttpContext = new DefaultHttpContext() // mocks HttpContext for null refrence used in chaching
            };

            var result = await controller.GetByID(100);
            var okResult = Assert.IsType<OkObjectResult>(result);
            var list = Assert.IsType<List<Touchpoint>>(okResult.Value);

            Assert.Single(list);
        }

        [Fact]
        public async Task GetByID_Returns_Ok_When_FlightId_Matches_FlightID()
        {
            var context = GetInMemoryDbContext();
            var controller = new TouchpointController(context);
            controller.ControllerContext = new ControllerContext()
            {
                HttpContext = new DefaultHttpContext() // mocks HttpContext for null refrence used in chaching
            };


            var result = await controller.GetByID(100);
            var okResult = Assert.IsType<OkObjectResult>(result);
            var list = Assert.IsType<List<Touchpoint>>(okResult.Value);

            // Assert.Single(list);
            Assert.Equal(100, list[0].FlightId);
        }

        [Fact]
        public async Task GetByID_Returns_Ok_When_FlightId_Matches_TouchPointType()
        {
            var context = GetInMemoryDbContext();
            var controller = new TouchpointController(context);
            controller.ControllerContext = new ControllerContext()
            {
                HttpContext = new DefaultHttpContext() // mocks HttpContext for null refrence used in chaching
            };

            var result = await controller.GetByID(100);
            var okResult = Assert.IsType<OkObjectResult>(result);
            var list = Assert.IsType<List<Touchpoint>>(okResult.Value);

            Assert.Equal("Boarding", list[0].TouchpointType);
        }

        [Fact]
        public async Task GetByID_Returns_Ok_When_FlightId_Matches_TouchpointTime()
        {
            var context = GetInMemoryDbContext();
            var controller = new TouchpointController(context);
            controller.ControllerContext = new ControllerContext()
            {
                HttpContext = new DefaultHttpContext() // mocks HttpContext for null refrence used in chaching
            };

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
            var context = GetInMemoryDbContext();
            var controller = new TouchpointController(context);
            controller.ControllerContext = new ControllerContext()
            {
                HttpContext = new DefaultHttpContext() // mocks HttpContext for null refrence used in chaching
            };

            var result = await controller.GetByID(100);
            var okResult = Assert.IsType<OkObjectResult>(result);
            var list = Assert.IsType<List<Touchpoint>>(okResult.Value);

            Assert.Equal(180, list[0].TouchpointPax);
        }

        [Fact]
        public async Task GetByID_Returns_NotFound_When_No_Match()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<FlightDBContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;
            using var emptyContext = new FlightDBContext(options);
            var controller = new TouchpointController(emptyContext);

            var httpContext = new DefaultHttpContext();
            httpContext.Request.Path = "/SearchByFlightID/2"; // Set desired path

            controller.ControllerContext = new ControllerContext
            {
                HttpContext = httpContext
            };

            var result = await controller.GetByID(2); // No such ID
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
            var error = Assert.IsType<Error>(notFoundResult.Value); // check if NotFoundObjectResult has an Error object

            Assert.Equal(404, error.StatusCode); // check error.
            Assert.Equal("/SearchByFlightID/2", error.Url); // check Url.
            Assert.Contains("Not Found", error.Details); // check detail.
            Assert.False("An Error occured" == error.Message); // check if default message is NOT used.
        }

        [Fact]
        public async Task GetPage1_OkResult()
        {
            FlightDBContext context = GetInMemoryDbContext();
            TouchpointController controller = new TouchpointController(context);
            controller.ControllerContext = new ControllerContext()
            {
                HttpContext = new DefaultHttpContext() // mocks HttpContext for null refrence used in chaching
            };

            var result = await controller.GetPage1(1);
            var OkResult = Assert.IsType<OkObjectResult>(result);
        }

        [Fact]
        public async Task GetPage1_Returns_Ok_For_Valid_Page()
        {
            var context = GetInMemoryDbContext();
            var controller = new TouchpointController(context);
            controller.ControllerContext = new ControllerContext()
            {
                HttpContext = new DefaultHttpContext() // mocks HttpContext for null refrence used in chaching
            }; 
            
            var result = await controller.GetPage1(1);
            var okResult = Assert.IsType<OkObjectResult>(result);
        }
        [Fact]
        public async Task GetPage1_Returns_PageManager()
        {
            FlightDBContext context = GetInMemoryDbContext();
            TouchpointController controller = new TouchpointController(context);
            controller.ControllerContext = new ControllerContext()
            {
                HttpContext = new DefaultHttpContext() // mocks HttpContext for null refrence used in chaching
            };

            var result = await controller.GetPage1(1);
            var okResult = Assert.IsType<OkObjectResult>(result);
            var message = Assert.IsType<PageManager>(okResult.Value);
        }
        [Fact]
        public async Task GetPage1_Returns_NotNullMessage()
        {
            FlightDBContext context = GetInMemoryDbContext();
            TouchpointController controller = new TouchpointController(context);
            controller.ControllerContext = new ControllerContext()
            {
                HttpContext = new DefaultHttpContext() // mocks HttpContext for null refrence used in chaching
            };

            var result = await controller.GetPage1(1);
            var okResult = Assert.IsType<OkObjectResult>(result);
            var message = Assert.IsType<PageManager>(okResult.Value);

            Assert.False(message == null); // checks if message is null
        }
        [Fact]
        public async Task GetPage1_Returns_NoNullTouchpoints()
        {
            FlightDBContext context = GetInMemoryDbContext();
            TouchpointController controller = new TouchpointController(context);
            controller.ControllerContext = new ControllerContext()
            {
                HttpContext = new DefaultHttpContext() // mocks HttpContext for null refrence used in chaching
            };

            var result = await controller.GetPage1(1);
            var okResult = Assert.IsType<OkObjectResult>(result);
            var message = Assert.IsType<PageManager>(okResult.Value);

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
        public async Task GetPage1_Returns_Message()
        {
            FlightDBContext context = GetInMemoryDbContext();
            TouchpointController controller = new TouchpointController(context);
            controller.ControllerContext = new ControllerContext()
            {
                HttpContext = new DefaultHttpContext() // mocks HttpContext for null refrence used in chaching
            };

            var result = await controller.GetPage1(1);
            var okResult = Assert.IsType<OkObjectResult>(result);
            var message = Assert.IsType<PageManager>(okResult.Value);

            Assert.False(message == null); // checks if message is null
            Assert.False(message.Touchpoints?.Any(x => x == null)); // Checks if it does NOT contain any null's
            Assert.Equal(1, message.TotalPages); // Check total pages evry 100 touchpoints should have 1 page should only have 1 page
            Assert.Equal(2, message.Touchpoints.Length);
            Assert.Equal(2, message.TotalTouchpointRecords);
        }


        [Fact]
        public async Task GetPage1_Returns_Redirect()
        {
            var context = GetInMemoryDbContext();
            var controller = new TouchpointController(context);

            var httpContext = new DefaultHttpContext();
            httpContext.Request.Path = "/api/page/2/"; // Set desired path

            controller.ControllerContext = new ControllerContext
            {
                HttpContext = httpContext
            };

            var result = await controller.GetPage1(2);
            var okResult = Assert.IsType<RedirectResult>(result);
            Assert.Equal("1", okResult.Url); // Redirect URL result
        }

        [Fact]
        public async Task GetPage1_Returns_NotFound_When_Empty_Database()
        {
            // Arrange
            FlightDBContext emptyContext = GetInMemoryDbContextEmpty();
            TouchpointController controller = new TouchpointController(emptyContext);

            var httpContext = new DefaultHttpContext();
            httpContext.Request.Path = "/api/page/1"; // Set desired path

            controller.ControllerContext = new ControllerContext
            {
                HttpContext = httpContext
            };

            var result = await controller.GetPage1(1);
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
        public async Task GetPage1_Returns_NotFound_When_Empty_Database_Status()
        {
            // Arrange
            FlightDBContext emptyContext = GetInMemoryDbContextEmpty();
            TouchpointController controller = new TouchpointController(emptyContext);

            var httpContext = new DefaultHttpContext();
            httpContext.Request.Path = "/api/page/1"; // Set desired path

            controller.ControllerContext = new ControllerContext
            {
                HttpContext = httpContext
            };

            var result = await controller.GetPage1(1);
            Assert.IsType<NotFoundObjectResult>(result);
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
            var error = Assert.IsType<Error>(notFoundResult.Value); // check if NotFoundObjectResult has an Error object

            Assert.Equal(404, error.StatusCode); // check error.
        }

        [Fact]
        public async Task GetPage1_Returns_NotFound_When_Empty_Database_Url()
        {
            // Arrange
            FlightDBContext emptyContext = GetInMemoryDbContextEmpty();
            TouchpointController controller = new TouchpointController(emptyContext);

            var httpContext = new DefaultHttpContext();
            httpContext.Request.Path = "/api/page/1"; // Set desired path

            controller.ControllerContext = new ControllerContext
            {
                HttpContext = httpContext
            };

            var result = await controller.GetPage1(1);
            Assert.IsType<NotFoundObjectResult>(result);
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
            var error = Assert.IsType<Error>(notFoundResult.Value); // check if NotFoundObjectResult has an Error object

            Assert.Equal("/api/page/1", error.Url); // check Url.

        }

        [Fact]
        public async Task GetPage1_Returns_NotFound_When_Empty_Database_Detail()
        {
            // Arrange
            FlightDBContext emptyContext = GetInMemoryDbContextEmpty();
            TouchpointController controller = new TouchpointController(emptyContext);

            var httpContext = new DefaultHttpContext();
            httpContext.Request.Path = "/api/page/1"; // Set desired path

            controller.ControllerContext = new ControllerContext
            {
                HttpContext = httpContext
            };

            var result = await controller.GetPage1(1);
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
        public async Task GetPage1_Returns_NotFound_When_Empty_Database_Message_NotDeffault()
        {
            // Arrange
            FlightDBContext emptyContext = GetInMemoryDbContextEmpty();
            TouchpointController controller = new TouchpointController(emptyContext);

            var httpContext = new DefaultHttpContext();
            httpContext.Request.Path = "/api/page/1"; // Set desired path

            controller.ControllerContext = new ControllerContext
            {
                HttpContext = httpContext
            };

            var result = await controller.GetPage1(1);
            Assert.IsType<NotFoundObjectResult>(result);
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
            var error = Assert.IsType<Error>(notFoundResult.Value); // check if NotFoundObjectResult has an Error object

            Assert.NotEqual(ErrorTest.ErrorDefaultMessage(), error.Message); // check if default message is NOT used.
        }
        [Fact]
        public async Task GetPage1_Returns_NotFound_When_Empty_Database_CorrectMessage()
        {
            // Arrange
            FlightDBContext emptyContext = GetInMemoryDbContextEmpty();
            TouchpointController controller = new TouchpointController(emptyContext);

            var httpContext = new DefaultHttpContext();
            httpContext.Request.Path = "/api/page/1"; // Set desired path

            controller.ControllerContext = new ControllerContext
            {
                HttpContext = httpContext
            };

            var result = await controller.GetPage1(1);
            Assert.IsType<NotFoundObjectResult>(result);
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
            var error = Assert.IsType<Error>(notFoundResult.Value); // check if NotFoundObjectResult has an Error object

            Assert.Equal("An error occured. There are no Touchpoints found make contact with Webprovider if its ongoing issue. Sorry for inconvinence.", error.Message);
        }
    }
}