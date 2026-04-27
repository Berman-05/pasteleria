using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProyectoAnalisis.Data;
using ProyectoAnalisis.Data;

namespace ProyectoAnalisis.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UsuariosController : ControllerBase
    {
        private readonly AppDbContext _context;

        public UsuariosController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> GetUsuarios()
        {
            var usuarios = await _context.Usuarios.ToListAsync();
            return Ok(usuarios);
        }
    }
}