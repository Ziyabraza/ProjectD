
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
        StressTest.Run(); // <-- Actually run the stress test
    }
}

public static class StressTest
{
    public static void Run()
    {
        var httpClient = new HttpClient();

        var scenario = Scenario.Create("login", async context =>
        {
            await Task.Delay(500);

            var loginData = new
            {
                username = "testuser",       // Replace with valid user
                password = "password123"     // Replace with valid password
            };

            var json = JsonConvert.SerializeObject(loginData);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await httpClient.PostAsync("http://localhost:5165/api/auth/login", content);

            return Response.Ok();
        }
        ).WithoutWarmUp();

        NBomberRunner
            .RegisterScenarios(scenario)
            .Run();
    }
}
