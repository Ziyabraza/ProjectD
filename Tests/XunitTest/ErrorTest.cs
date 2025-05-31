﻿using Newtonsoft.Json.Linq;
using Moq;

namespace ProjectD
{
    public class ErrorTest
    {
        public static string ErrorDefaultMessage() => new Error(200, "Returning 'An Error has Acccured' default message").Message;
        [Fact]
        public async Task TestNoMessageErrorObject()
        {
            // Arrange
            int statusCode = 404;
            string url = "https://example.com/notfound";
            
            // Act
            Error error = new Error(statusCode, url); // Message not filled
            
            // Assert
            Assert.Equal("An error occurred.", error.Message);
            Assert.Equal("Not Found - The resource could not be found.", error.Details);
            Assert.Equal(statusCode, error.StatusCode);
            Assert.Equal(url, error.Url);
        }
        [Fact]
        public async Task TestWithMessageErrorObject()
        {
            // Arrange
            int statusCode = 401;
            string url = "https://example.com/protected";
            string message = "User not authenticated.";

            // Act
            Error error = new Error(statusCode, url, message);

            // Assert
            Assert.Equal(message, error.Message);
            Assert.Equal("Unauthorized - Authentication is required.", error.Details);
        }
        [Fact]
        public async Task TestErrorLog()
        {
            // Arrange
            string testUrl = "https://test.com/test";
            string customMessage = "Test error logging";
            int statusCode = 500;
            DateTime date = DateTime.Now;
            DateOnly dateOnly = new DateOnly(date.Year, date.Month, date.Day);

            // Clean or prepare path
            string path = @$"Backend/Data/ErrorLogs/{dateOnly}.json";

            Assert.True(File.Exists(path));
            if(File.Exists(path))
            {
                File.Delete(path);
            }

            // Act
            Error error1 = new Error(statusCode, testUrl, customMessage);
            Error error2 = new Error(statusCode, testUrl, customMessage);

            // Assert
            

            string fileContent = File.ReadAllText(path);
            JArray logs = JArray.Parse(fileContent);

            Assert.True(logs.Count >= 2); // Should have at least two entries

            foreach (var log in logs)
            {
                Assert.Equal(statusCode, (int)log["statusCode"]);
                Assert.Equal(testUrl, (string)log["url"]);
                Assert.Equal(customMessage, (string)log["message"]);
            }
        }
    }
}