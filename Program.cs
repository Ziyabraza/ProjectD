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
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

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
builder.Services.AddSwaggerGen(options => {
    options.SwaggerDoc("v1", new() { Title = "Your API", Version = "v1" });

    //JWT authenticatie voor Swagger
    options.AddSecurityDefinition("Bearer", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = Microsoft.OpenApi.Models.SecuritySchemeType.ApiKey,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = Microsoft.OpenApi.Models.ParameterLocation.Header,
        Description = "Enter your JWT token like: **Bearer your_token_here**"
    });

    options.AddSecurityRequirement(new Microsoft.OpenApi.Models.OpenApiSecurityRequirement
    {
        {
            new Microsoft.OpenApi.Models.OpenApiSecurityScheme
            {
                Reference = new Microsoft.OpenApi.Models.OpenApiReference
                {
                    Type = Microsoft.OpenApi.Models.ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new string[] {}
        }
    });
});


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
    // Note2: 30/03/2025: it partialy works not, but it logs unecessary data that are mostly null in the fields and dont found a workaround for that yet.
    // Note3: 31/03/2025: Should now properly log in PostpreSQL make sure to create table manualy!
    .WriteTo.PostgreSQL(connectionString, "logs2", columnOptions, restrictedToMinimumLevel: Serilog.Events.LogEventLevel.Error, needAutoCreateTable: true, needAutoCreateSchema: true) // Log to PostgreSQL, 
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
//JWT Authentication met Key:
var key = Encoding.UTF8.GetBytes("!!H0g3ScH00LR0tt3RdAm@2025-04-22??"); 



builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.RequireHttpsMetadata = true; // Set true in production!
    options.SaveToken = true;
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = builder.Configuration["JwtSettings:Issuer"],
        ValidAudience = builder.Configuration["JwtSettings:Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(key)
    };
});

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

app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run(); 
