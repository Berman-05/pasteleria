using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using ProyectoAnalisis.Data;
using ProyectoAnalisis.DTOs;
using ProyectoAnalisis.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace ProyectoAnalisis.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IConfiguration _configuration;

        public AuthController(AppDbContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterDto dto)
        {
            // Validar si username ya existe
            if (await _context.Usuarios.AnyAsync(u => u.Username == dto.Username))
            {
                return BadRequest("El nombre de usuario ya está en uso.");
            }

            // Validar si email ya existe
            if (await _context.Usuarios.AnyAsync(u => u.Email == dto.Email))
            {
                return BadRequest("El correo ya está registrado.");
            }

            // Buscar rol "Cliente" o "Estudiante"
            var rol = await _context.Roles.FirstOrDefaultAsync(r => r.NombreRol == "Cliente");

            if (rol == null)
            {
                return BadRequest("No existe el rol Cliente en la base de datos.");
            }

            var usuario = new Usuario
            {
                NombreCompleto = dto.NombreCompleto,
                FechaNacimiento = dto.FechaNacimiento,
                Username = dto.Username,
                Email = dto.Email,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password),
                IdRol = rol.IdRol
            };

            _context.Usuarios.Add(usuario);
            await _context.SaveChangesAsync();

            return Ok("Usuario registrado correctamente.");
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginDto dto)
        {
            var usuario = await _context.Usuarios
                .Include(u => u.Rol)
                .FirstOrDefaultAsync(u => u.Email == dto.Email);

            if (usuario == null)
            {
                return Unauthorized("Credenciales inválidas.");
            }

            bool passwordValida = BCrypt.Net.BCrypt.Verify(dto.Password, usuario.PasswordHash);

            if (!passwordValida)
            {
                return Unauthorized("Credenciales inválidas.");
            }

            var token = GenerarToken(usuario);

            var response = new AuthResponseDto
            {
                Token = token,
                Username = usuario.Username,
                Rol = usuario.Rol.NombreRol
            };

            return Ok(response);
        }

        private string GenerarToken(Usuario usuario)
        {
            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, usuario.IdUsuario.ToString()),
                new Claim(ClaimTypes.Name, usuario.Username),
                new Claim(ClaimTypes.Email, usuario.Email),
                new Claim(ClaimTypes.Role, usuario.Rol.NombreRol)
            };

            var key = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));

            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: DateTime.Now.AddHours(2),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}