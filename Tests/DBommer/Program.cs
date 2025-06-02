
using System.Text;
using NBomber.CSharp;
using NBomber.Http;
using Newtonsoft.Json;
using ProjectD.Models;
namespace ProjectD;

public class Program
{
    public static void Main(string[] args)
    {
        // !!!WARNING!!! Make sure you back up and replace SeriLog and ErrorLogs afther bomming because if there is an Error response it will bloat these files and the server.
        // <-- Actually run the stress test
        var token = StressTest.GetJwtToken();
        Console.WriteLine(token.Result);
        List<string> paths = new()
        {
            "http://localhost:5165/api/Touchpoint/page/1",
            "http://localhost:5165/api/Touchpoint/page/2",
            "http://localhost:5165/api/Touchpoint/page/3",
            "http://localhost:5165/api/Touchpoint/page/4",
            "http://localhost:5165/api/Touchpoint/page/5"
        };
        if (token != null)
        {
            foreach (string path in paths)
            {
                StressTest.Run(token.Result.ToString(), path);
            }
        }
    }
}

public static class StressTest
{
    public static async Task<string> GetJwtToken()
    {
        var httpClient = new HttpClient();

        var loginData = new
        {
            username = "Admin1",
            password = "password123"
        };

        var json = JsonConvert.SerializeObject(loginData);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        var response = await httpClient.PostAsync("http://localhost:5165/api/auth/login", content);
        var responseBody = await response.Content.ReadAsStringAsync();

        // Assume token is returned as: { "token": "abc.xyz.123", ... }
        dynamic result = JsonConvert.DeserializeObject(responseBody);

        return result.token;
    }
    public static void Run(string token, string path)
    {
        var httpClient = new HttpClient();
        httpClient.DefaultRequestHeaders.Authorization =
            new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
            var scenario = Scenario.Create("login", async context =>
            {
                await Task.Delay(0); // sleep at each attempt

                var response = await httpClient.GetAsync(path); // you can put your URL here to for performance test, by bomming this URL

                if (response.IsSuccessStatusCode)
                    return Response.Ok();
                else
                    return Response.Fail(response.StatusCode);
            }).WithoutWarmUp();

        NBomberRunner
            .RegisterScenarios(scenario)
            .Run();
    }
}


