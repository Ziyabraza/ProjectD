﻿using ProjectD.Models;
using ProjectD.Controllers;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Microsoft.AspNetCore.Http;
using Microsoft.VisualStudio.TestPlatform.CommunicationUtilities;

namespace ProjectD
{
    public class TouchpointTest
    {
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
        [Fact]
        public async Task GetByID_Returns_Ok_When_FlightId_Matches()
        {
            var context = GetInMemoryDbContext();
            var controller = new TouchpointController(context);

            var result = await controller.GetByID(100);
            var okResult = Assert.IsType<OkObjectResult>(result);
            var list = Assert.IsType<List<Touchpoint>>(okResult.Value);

            Assert.Single(list);
            Assert.Equal(100, list[0].FlightId);
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
            Assert.False("An Error acured" == error.Message); // check if default message is NOT used.
        }

        [Fact]
        public async Task GetPage1_Returns_Ok_For_Valid_Page()
        {
            var context = GetInMemoryDbContext();
            var controller = new TouchpointController(context);

            var result = await controller.GetPage1(1);
            var okResult = Assert.IsType<OkObjectResult>(result);
            var message = Assert.IsType<PageManager>(okResult.Value);

            Assert.False(message == null); // checks if message is null
            Assert.False(message.Touchpoints?.Any(x => x == null)); // Checks if it does NOT contain any null's
            Assert.True(message.TotalPages == 1); // Check total pages evry 100 touchpoints should have 1 page should only have 1 page
            Assert.True(message.Touchpoints.Length == 2);
            Assert.True(message.TotalTouchpointRecords == 2);
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

            Assert.True(okResult.Url == "1"); // Redirect URL result
        }

        [Fact]
        public async Task GetPage1_Returns_NotFound_When_Empty_Database()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<FlightDBContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;
            using var emptyContext = new FlightDBContext(options);
            var controller = new TouchpointController(emptyContext);

            var httpContext = new DefaultHttpContext();
            httpContext.Request.Path = "/api/page/1"; // Set desired path

            controller.ControllerContext = new ControllerContext
            {
                HttpContext = httpContext
            };

            var result = await controller.GetPage1(1);
            Console.WriteLine(result);
            Assert.IsType<NotFoundObjectResult>(result);
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
            var error = Assert.IsType<Error>(notFoundResult.Value); // check if NotFoundObjectResult has an Error object

            // Now assert on the Error object
            Assert.Equal(404, error.StatusCode); // check error.
            Assert.Equal("/api/page/1", error.Url); // check Url.
            Assert.Contains("not found", error.Details.ToLower()); // check detail.
            Assert.False("An Error acured" == error.Message); // check if default message is NOT used.
            
        }
    }
}