
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
        // <-- Actually run the stress test
        var something = StressTest.GetJwtToken();
        Console.WriteLine(something.Result);
        if (something != null)
        {
            StressTest.Run();
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
    public static void Run()
    {
        var httpClient = new HttpClient();

        var scenario = Scenario.Create("login", async context =>
        {
            await Task.Delay(500);

            var loginData = new
            {
                username = "Admin1",       // Replace with valid user
                password = "password123"     // Replace with valid password
            };

            var json = JsonConvert.SerializeObject(loginData);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await httpClient.GetAsync("http://localhost:5165/api/Touchpoint/page/1");

            return Response.Ok();
        }
        ).WithoutWarmUp();

        NBomberRunner
            .RegisterScenarios(scenario)
            .Run();
    }
}
