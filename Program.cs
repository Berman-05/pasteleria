using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using ProyectoAnalisis.Data;
using ProyectoAnalisis.Models;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddCors(options =>
{
    options.AddPolicy("MiPoliticaCORS", policy =>
    {
        policy.AllowAnyOrigin() // Permite cualquier origen (frontend)
              .AllowAnyMethod() // Permite GET, POST, PUT, DELETE, etc.
              .AllowAnyHeader(); // Permite cualquier cabecera
    });
});

builder.Services.AddControllers();

// Conexión a PostgreSQL
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

// JWT
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,

            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]!))
        };
    });

builder.Services.AddAuthorization();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

// Swagger + JWT
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "ProyectoAnalisis API",
        Version = "v1"
    });

    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Ingresa el token así: Bearer {tu token}"
    });

    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new string[] {}
        }
    });
});

var app = builder.Build();
app.UseCors("MiPoliticaCORS");

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
Console.WriteLine("CONNECTION STRING:");
Console.WriteLine(builder.Configuration.GetConnectionString("DefaultConnection"));
app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();

    // Imprimimos un log visible en Railway
    Console.WriteLine(">>> DEBUG: Iniciando lógica de base de datos...");

    try
    {
        // Usaremos EnsureCreated() porque es más agresivo y fiable para despliegues rápidos
        // EnsureCreated() crea la DB y tablas si no existen, sin depender del historial de migraciones
        bool created = dbContext.Database.EnsureCreated();

        if (created)
        {
            Console.WriteLine(">>> DEBUG: ¡Base de datos creada exitosamente!");
        }
        else
        {
            Console.WriteLine(">>> DEBUG: La base de datos ya existía (o no fue necesario crearla).");
        }
    }
    catch (Exception ex)
    {
        Console.WriteLine($">>> DEBUG FATAL: Error en base de datos: {ex.Message}");
    }
}

app.Run();