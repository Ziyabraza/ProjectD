﻿using Newtonsoft.Json.Linq;
using Moq;

namespace ProjectD
{
    public class ErrorTest
    {
        public static string ErrorDefaultMessage() => new Error(200, "Returning 'An Error has Acccured' default message").Message;
        private Error GetStatus404NoMessage()
        {
            int statusCode = 404;
            string url = "https://example.com/notfound";
            return new Error(statusCode, url); // Message not filled
        }
        private Error GetStatus401WithMessage()
        {
            int statusCode = 401;
            string url = "https://example.com/protected";
            string message = "User not authenticated.";
            return new Error(statusCode, url, message);
        }
        private Error GetTestErrorObject()
        {
            string testUrl = "https://test.com/test";
            string customMessage = "Test error logging";
            int statusCode = 500;
            return new Error(statusCode, testUrl, customMessage);
        }
        private void DeleteTestJSON(string path)
        {
            if (File.Exists(path))
            {
                File.Delete(path);
            }
        }
        private JArray GetTestJSONLogs(string path)
        {
            if (!File.Exists(path))
            {
                return null;
            }
            string fileContent = File.ReadAllText(path);
            return JArray.Parse(fileContent);
        }
        [Fact]
        public async Task TestNoMessageErrorObject_Message()
        {
            Error error = GetStatus404NoMessage(); // Message not filled
            Assert.Equal(ErrorDefaultMessage(), error.Message);
        }
        [Fact]
        public async Task TestNoMessageErrorObject_Details()
        {
            Error error = GetStatus404NoMessage(); // Message not filled
            Assert.Equal("Not Found - The resource could not be found.", error.Details);
        }
        [Fact]
        public async Task TestNoMessageErrorObject_StatusCode()
        {
            Error error = GetStatus404NoMessage(); // Message not filled
            Assert.Equal(404, error.StatusCode);
        }
        [Fact]
        public async Task TestNoMessageErrorObject_Url()
        {
            Error error = GetStatus404NoMessage(); // Message not filled
            Assert.Equal("https://example.com/notfound", error.Url);
        }
        [Fact]
        public async Task TestWithMessageErrorObject_Message()
        {
            Error error = GetStatus401WithMessage();
            Assert.Equal("User not authenticated.", error.Message);
        }
        [Fact]
        public async Task TestWithMessageErrorObject_Details()
        {
            Error error = GetStatus401WithMessage();
            Assert.Equal("Unauthorized - Authentication is required.", error.Details);
        }
        [Fact]
        public async Task TestWithMessageErrorObject_StatusCode()
        {
            Error error = GetStatus401WithMessage();
            Assert.Equal(401, error.StatusCode);
        }
        [Fact]
        public async Task TestWithMessageErrorObject_Url()
        {
            Error error = GetStatus401WithMessage();
            Assert.Equal("https://example.com/protected", error.Url);
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
            if (File.Exists(path))
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
        [Fact]
        public async Task TestErrorLog_JSONFileCreation()
        {
            DateTime date = DateTime.Now;
            DateOnly dateOnly = new DateOnly(date.Year, date.Month, date.Day);
            string path = @$"Backend/Data/ErrorLogs/{dateOnly}.json";

            if (File.Exists(path)) { File.Delete(path); } // to make sure file does not exist before hand
            GetTestErrorObject(); // An new error generates a new JSON file
            Assert.True(File.Exists(path));
            if (File.Exists(path)) { File.Delete(path); } // delete file for next test
        }

        [Fact]
        public async Task TestErrorLog_JSONFileCount()
        {
            DateTime date = DateTime.Now;
            DateOnly dateOnly = new DateOnly(date.Year, date.Month, date.Day);
            string path = @$"Backend/Data/ErrorLogs/{dateOnly}.json";

            DeleteTestJSON(path); // to make sure file does not exist before hand
            for (int i = 0; i < 10; i++)
            {
                GetTestErrorObject(); // generate 10 Errors
            }
            JArray logs = GetTestJSONLogs(path);
            Assert.Equal(10, logs.Count);
            DeleteTestJSON(path); // delete file for next test
        }
        [Fact]
        public async Task TestErrorLog_ReadJSONContent_StatusCode()
        {
            DateTime date = DateTime.Now;
            DateOnly dateOnly = new DateOnly(date.Year, date.Month, date.Day);
            string path = @$"Backend/Data/ErrorLogs/{dateOnly}.json";
            Error test = GetTestErrorObject(); ;

            DeleteTestJSON(path); // to make sure file does not exist before hand
            for (int i = 0; i < 2; i++)
            {
                test = GetTestErrorObject(); // generate 2 Errors
            }
            JArray logs = GetTestJSONLogs(path);
            foreach (var log in logs)
            {
                Assert.Equal(test.StatusCode, (int)log["statusCode"]);
            }
            DeleteTestJSON(path); // delete file for next test
        }
        [Fact]
        public async Task TestErrorLog_ReadJSONContent_Url()
        {
            DateTime date = DateTime.Now;
            DateOnly dateOnly = new DateOnly(date.Year, date.Month, date.Day);
            string path = @$"Backend/Data/ErrorLogs/{dateOnly}.json";
            Error test = GetTestErrorObject(); ;

            DeleteTestJSON(path); // to make sure file does not exist before hand
            for (int i = 0; i < 10; i++)
            {
                test = GetTestErrorObject(); // generate 10 Errors
            }
            JArray logs = GetTestJSONLogs(path);
            foreach (var log in logs)
            {
                Assert.Equal(test.Url, (string)log["url"]);
            }
            DeleteTestJSON(path); // delete file for next test
        }
        [Fact]
        public async Task TestErrorLog_ReadJSONContent_Message()
        {
            DateTime date = DateTime.Now;
            DateOnly dateOnly = new DateOnly(date.Year, date.Month, date.Day);
            string path = @$"Backend/Data/ErrorLogs/{dateOnly}.json";
            Error test = GetTestErrorObject();;

            DeleteTestJSON(path); // to make sure file does not exist before hand
            for (int i = 0; i < 10; i++)
            {
                test = GetTestErrorObject(); // generate 10 Errors
            }
            JArray logs = GetTestJSONLogs(path);
            foreach (var log in logs)
            {
                Assert.Equal(test.Message, (string)log["message"]);
            }
            DeleteTestJSON(path); // delete file for next test
        }
    }
}