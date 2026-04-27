using Microsoft.EntityFrameworkCore;
using ProyectoAnalisis.Data;
using ProyectoAnalisis.Models;

var builder = WebApplication.CreateBuilder(args);

// 🔹 1. DB
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(connectionString));

// 🔹 2. CORS (ANTES de build)
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll",
        policy => policy
            .AllowAnyOrigin()
            .AllowAnyHeader()
            .AllowAnyMethod());
});

// 🔹 otros servicios
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();
using (var scope = app.Services.CreateScope())
{
    try
    {
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        db.Database.Migrate();

        if (!db.Roles.Any())
        {
            db.Roles.AddRange(
                new Rol { NombreRol = "Cliente" },
                new Rol { NombreRol = "Estudiante" },
                new Rol { NombreRol = "Admin" }
            );
            db.SaveChanges();
        }
    }
    catch (Exception ex)
    {
        Console.WriteLine("ERROR DB INIT: " + ex.Message);
    }
}


// 🔹 3. CORS (DESPUÉS de build, antes de endpoints)
app.UseCors("AllowAll");

// 🔹 middleware normal


    app.UseSwagger();
    app.UseSwaggerUI();


app.UseAuthorization();

app.MapControllers();

// 🔹 4. PUERTO (al final)
var port = Environment.GetEnvironmentVariable("PORT") ?? "8080";
app.Run($"http://0.0.0.0:{port}");
