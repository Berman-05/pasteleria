using Microsoft.EntityFrameworkCore;
using ProyectoAnalisis.Data;

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

// 🔹 3. CORS (DESPUÉS de build, antes de endpoints)
app.UseCors("AllowAll");

// 🔹 middleware normal
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthorization();

app.MapControllers();

// 🔹 4. PUERTO (al final)
app.Run();