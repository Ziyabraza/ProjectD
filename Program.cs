using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using Npgsql;
using OpenTelemetry;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using ProjectD;
using Serilog;
using Serilog.Sinks.PostgreSQL;
using Serilog.Sinks.PostgreSQL.ColumnWriters;

var builder = WebApplication.CreateBuilder(new WebApplicationOptions
{
    Args = args,
    EnvironmentName = Environments.Development
});

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

var columnOptions = new Dictionary<string, ColumnWriterBase>
{
    { "timestamp", new TimestampColumnWriter() },
    { "log_level", new LevelColumnWriter() },
    { "message", new RenderedMessageColumnWriter() },
    { "source", new SinglePropertyColumnWriter("REST_API") }
};

Serilog.Log.Logger = new LoggerConfiguration()
    .WriteTo.Console() // Log to console
    // Note: WriteTo.PostgreSQL is not working at the moment, if someone knows a fix it would be apreaciated.
    // Made workaround in Error.cs that logs errors in a JSON file.
    .WriteTo.PostgreSQL(connectionString, "public.logs", columnOptions, needAutoCreateTable: true) // Log to PostgreSQL, 
    .CreateLogger();

Serilog.Log.Information("Test log message 2");

using (var conn = new NpgsqlConnection(builder.Configuration.GetConnectionString("DefaultConnection")))
{
    try
    {
        conn.Open();
        Console.WriteLine("Database connection successful!");
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Database connection failed: {ex.Message}");
    }
}

builder.Logging.ClearProviders();  // Clear default providers to prevent duplication
builder.Logging.AddSerilog(); 


builder.Services.AddDbContext<FlightDBContext>(options => options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Services.AddScoped<ExcelImportService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{

    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthorization();
app.MapControllers();

app.Use(async (context, next) =>
{
    try
    {
        // Proceed with the request
        await next();

        // Log the status after the request has been processed
        Log.Information($"Request: {context.Request.Path}, Status: {context.Response.StatusCode}");

        // If the status code is 4xx or 5xx (indicating errors), log that too
        if (context.Response.StatusCode >= 400)
        {
            // Console.WriteLine("an error occurred");
            Log.Error($"Request to {context.Request.Path} returned status {context.Response.StatusCode}");
        }
    }
    catch (Exception ex)
    {
        // Catch any unhandled exceptions and log as error
        Log.Error(ex, "An unhandled error occurred while processing the request.");
        throw; // Rethrow the exception after logging
    }
});

app.Run(); 
