using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using Npgsql;
using NpgsqlTypes;
using OpenTelemetry;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using ProjectD;
using Serilog;
using Serilog.Sinks.PostgreSQL;
using Serilog.Sinks.PostgreSQL.ColumnWriters;
using System.Collections.Generic;

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
    { "status_code", new SinglePropertyColumnWriter("status_code", PropertyWriteMethod.Raw, NpgsqlDbType.Integer) }, // Integer
    { "details", new SinglePropertyColumnWriter("details", PropertyWriteMethod.Raw, NpgsqlDbType.Text) }, // String
    { "message", new SinglePropertyColumnWriter("message", PropertyWriteMethod.Raw, NpgsqlDbType.Text) }, // String
    { "url", new SinglePropertyColumnWriter("url", PropertyWriteMethod.Raw, NpgsqlDbType.Text) }, // String
    { "date", new TimestampColumnWriter() } // Timestamp
};

Serilog.Log.Logger = new LoggerConfiguration()
    .WriteTo.Console() // Log to console
    // Note1: WriteTo.PostgreSQL is not working at the moment, if someone knows a fix it would be apreaciated.
    // Made workaround in Error.cs that logs errors in a JSON file.
    // Note2 30/03/2025: It partialy works not, but it logs unecessary data that are mostly null in the fields and dont found a workaround for that yet.
    .WriteTo.PostgreSQL(connectionString, "logs", columnOptions, restrictedToMinimumLevel: Serilog.Events.LogEventLevel.Error) // Log to PostgreSQL, 
    .CreateLogger();


// this checks if the connection to the database is working
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

app.Run(); 
