var builder = WebApplication.CreateBuilder(args);

// Voeg CORS toe
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll",
        builder => builder.AllowAnyOrigin()
                          .AllowAnyMethod()
                          .AllowAnyHeader());
});

builder.Services.AddControllers();

var app = builder.Build();

// Gebruik CORS policy
app.UseCors("AllowAll");

app.UseRouting();
app.UseEndpoints(endpoints =>
{
    endpoints.MapControllers();
});

app.Run();
