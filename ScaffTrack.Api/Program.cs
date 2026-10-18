using System.Text.Json.Serialization;
using Microsoft.EntityFrameworkCore;
using ScaffTrack.Api.Data;

var builder = WebApplication.CreateBuilder(args);

// 1. Configuration SQL Server avec Entity Framework Core
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? "Server=localhost;Database=ScaffTrackDb;Trusted_Connection=True;TrustServerCertificate=True;MultipleActiveResultSets=true";

builder.Services.AddDbContext<ScaffTrackDbContext>(options =>
    options.UseSqlServer(connectionString));

// 2. Contrôleurs & Gestion des cycles JSON
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
        options.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
    });

// 3. Configuration CORS (pour autoriser l'app mobile MAUI et tests locaux)
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

// 4. OpenAPI / Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddOpenApi();

var app = builder.Build();

// 5. Initialisation automatique de la Base de Données SQL Server
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var context = services.GetRequiredService<ScaffTrackDbContext>();
        await DbInitializer.SeedAsync(context);
        Console.WriteLine("--> [ScaffTrackDb] Base de données SQL Server synchronisée et initialisée avec succès !");
    }
    catch (Exception ex)
    {
        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.WriteLine($"--> [ScaffTrackDb] Note SQL Server : {ex.Message}");
        Console.ResetColor();
    }
}

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseCors("AllowAll");

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
