﻿using Newtonsoft.Json.Linq;
using Moq;

namespace ProjectD
{
    public class ErrorTest : IClassFixture<ErrorTestSetup>
    {
        private readonly ErrorTestSetup _setup;
        public ErrorTest(ErrorTestSetup setup)
        {
            _setup = setup; // give ErrorTest a delay to prefent JSON content conflicts with assyncs.
        }
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
        private string GetPath()
        {
            DateTime date = DateTime.Now;
            DateOnly dateOnly = new DateOnly(date.Year, date.Month, date.Day);
            string fileName = dateOnly.ToString("dd-MM-yyyy"); // of "yyyy-MM-dd"

            return @$"Backend/Data/ErrorLogs/{fileName}.json"; // should be excluded from async tests
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
        public void TestNoMessageErrorObject_Message()
        {
            Error error = GetStatus404NoMessage(); // Message not filled
            Assert.Equal(ErrorDefaultMessage(), error.Message);
        }
        [Fact]
        public void TestNoMessageErrorObject_Details()
        {
            Error error = GetStatus404NoMessage(); // Message not filled
            Assert.Equal("Not Found - The resource could not be found.", error.Details);
        }
        [Fact]
        public void TestNoMessageErrorObject_StatusCode()
        {
            Error error = GetStatus404NoMessage(); // Message not filled
            Assert.Equal(404, error.StatusCode);
        }
        [Fact]
        public void TestNoMessageErrorObject_Url()
        {
            Error error = GetStatus404NoMessage(); // Message not filled
            Assert.Equal("https://example.com/notfound", error.Url);
        }
        [Fact]
        public void TestWithMessageErrorObject_Message()
        {
            Error error = GetStatus401WithMessage();
            Assert.Equal("User not authenticated.", error.Message);
        }
        [Fact]
        public void TestWithMessageErrorObject_Details()
        {
            Error error = GetStatus401WithMessage();
            Assert.Equal("Unauthorized - Authentication is required.", error.Details);
        }
        [Fact]
        public void TestWithMessageErrorObject_StatusCode()
        {
            Error error = GetStatus401WithMessage();
            Assert.Equal(401, error.StatusCode);
        }
        [Fact]
        public void TestWithMessageErrorObject_Url()
        {
            Error error = GetStatus401WithMessage();
            Assert.Equal("https://example.com/protected", error.Url);
        }
        [Fact]
        public void NullOrEmpty_Url()
        {
            Error error1 = new Error(500, "");
            Error error2 = new Error(500, null);
            Assert.Equal("UNKNOWN", error1.Url);
            Assert.Equal("UNKNOWN", error2.Url);
        }
        [Fact]
        public void TestErrorLog_Location()
        {
            // prepare path
            string path = GetPath();
            DeleteTestJSON(path); // to make sure file does not exist before hand

            // Act
            ErrorDefaultMessage(); // to generate error

            Assert.True(File.Exists(path));
            DeleteTestJSON(path); // delete file for next test
        }

        [Fact]
        public void TestErrorLog_JSONFileCount()
        {
            string path = GetPath();
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
        public void TestErrorLog_ReadJSONContent_StatusCode()
        {
            string path = GetPath();
            DeleteTestJSON(path); // to make sure file does not exist before hand
            Error test = null;
            for (int i = 0; i < 2; i++)
            {
                test = GetTestErrorObject(); // generate 2 Errors
            }

            JArray logs = GetTestJSONLogs(path);
            foreach (var log in logs)
            {
                Assert.Equal(test.StatusCode, (int)log["StatusCode"]);
            }
            DeleteTestJSON(path); // delete file for next test
        }
        [Fact]
        public void TestErrorLog_ReadJSONContent_Url()
        {
            string path = GetPath();
            DeleteTestJSON(path); // to make sure file does not exist before hand
            Error test = null;
            for (int i = 0; i < 10; i++)
            {
                test = GetTestErrorObject(); // generate 10 Errors
            }
            JArray logs = GetTestJSONLogs(path);
            foreach (var log in logs)
            {
                Assert.Equal(test.Url, (string)log["Url"]);
            }
            DeleteTestJSON(path); // delete file for next test
        }
        [Fact]
        public void TestErrorLog_ReadJSONContent_Message()
        {
            string path = GetPath();
            DeleteTestJSON(path); // to make sure file does not exist before hand
            Error test = null;
            for (int i = 0; i < 10; i++)
            {
                test = GetTestErrorObject(); // generate 10 Errors
            }
            JArray logs = GetTestJSONLogs(path);
            foreach (var log in logs)
            {
                Assert.Equal(test.Message, (string)log["Message"]);
            }
            DeleteTestJSON(path); // delete file for next test
        }
    }
}

public class ErrorTestSetup : IDisposable
{
    public ErrorTestSetup()
    {
        Thread.Sleep(5000);  // Assures prefention JSON file conflict when executing assync methods from other tests
    }

    public void Dispose()
    {
        // Runs once after all tests complete
    }
}