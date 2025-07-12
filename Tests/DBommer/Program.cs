
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

        // List<string> paths = new();
        // for (int i = 1; i < 1001; i++)
        // {
        //     paths.Add($"http://localhost:5165/api/Touchpoint/page/{i}");
        // }
            
        if (token != null)
        {
            // var scenarios = paths.Select(path => StressTest.RunAll(token.Result.ToString(), path)).ToArray();
            // StressTest.RunSingle(token.Result.ToString(), paths);
            var scenarios = StressTest.ChacheTest(token.Result.ToString(), "http://localhost:5165/api/Touchpoint/page/3");

            NBomberRunner
                .RegisterScenarios(scenarios)
                .Run();
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
    public static void RunSingle(string token, List<string> ActualPath)
    {
        var path = ActualPath[0]; // graps first in list
        var httpClient = new HttpClient();
        httpClient.DefaultRequestHeaders.Authorization =
            new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

        var scenario = Scenario.Create($"bombin {path}", async context =>
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

    public static NBomber.Contracts.ScenarioProps RunAll(string token, string path)
    {
        var httpClient = new HttpClient();
        httpClient.DefaultRequestHeaders.Authorization =
            new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

        return Scenario.Create($"bombin {path}", async context =>
        {
            await Task.Delay(0);

            var response = await httpClient.GetAsync(path);

            if (response.IsSuccessStatusCode)
                return Response.Ok();
            else
                return Response.Fail(response.StatusCode);

        }).WithoutWarmUp();
    }

    public static NBomber.Contracts.ScenarioProps ChacheTest(string token, string path)
    {
        var httpClient = new HttpClient();
        httpClient.DefaultRequestHeaders.Authorization =
            new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
        return Scenario.Create("cache-test", async context =>
        {
            var response = await httpClient.GetAsync(path);

            return response.IsSuccessStatusCode
                ? Response.Ok()
                : Response.Fail(response.StatusCode);
        })
        .WithLoadSimulations(Simulation.KeepConstant(copies: 1000, during: TimeSpan.FromSeconds(60)));
    }
}

