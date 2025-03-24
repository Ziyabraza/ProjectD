using Microsoft.EntityFrameworkCore;
using ProjectD;

var builder = WebApplication.CreateBuilder(args);

// Voeg de FlightDBContext toe aan Dependency Injection en verbind met PostgreSQL
builder.Services.AddDbContext<FlightDBContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

// Voeg controllers toe (ASP.NET Core Web API)
builder.Services.AddControllers();

// Voeg CORS toe als je frontend en backend vanaf verschillende domeinen gebruikt (optioneel)
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll",
        builder => builder.AllowAnyOrigin()
                          .AllowAnyMethod()
                          .AllowAnyHeader());
});

var app = builder.Build();

// Gebruik CORS (indien nodig)
app.UseCors("AllowAll");

app.UseRouting();

app.UseAuthorization();

app.MapControllers();

app.Run();