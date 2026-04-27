using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProyectoAnalisis.Data;

[ApiController]
[Route("api/[controller]")]
public class CateringController : ControllerBase
{
    private readonly AppDbContext _context;

    public CateringController(AppDbContext context)
    {
        _context = context;
    }
    [Authorize]
    [HttpPost]
    public async Task<IActionResult> CrearSolicitud(CrearCateringDto dto)
    {
        var userId = int.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)!.Value);

        if (dto.CantidadPersonas <= 0)
            return BadRequest("Cantidad inválida");

        var solicitud = new Catering
        {
            IdUsuario = userId,
            FechaEvento = dto.FechaEvento,
            CantidadPersonas = dto.CantidadPersonas,
            Descripcion = dto.Descripcion,
            Estado = "Pendiente"
        };

        _context.Caterings.Add(solicitud);
        await _context.SaveChangesAsync();

        return Ok("Solicitud enviada");
    }
    [Authorize]
    [HttpGet("mis-solicitudes")]
    public async Task<IActionResult> MisSolicitudes()
    {
        var userId = int.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)!.Value);

        var solicitudes = await _context.Caterings
            .Where(c => c.IdUsuario == userId)
            .ToListAsync();

        return Ok(solicitudes);
    }
    [Authorize(Roles = "Administrador")]
    [HttpGet]
    public async Task<IActionResult> Todas()
    {
        var solicitudes = await _context.Caterings
            .Include(c => c.Usuario)
            .ToListAsync();

        return Ok(solicitudes);
    }
    [Authorize(Roles = "Administrador")]
    [HttpPut("cambiar-estado/{id}")]
    public async Task<IActionResult> CambiarEstado(int id, [FromBody] string estado)
    {
        var solicitud = await _context.Caterings.FindAsync(id);

        if (solicitud == null)
            return NotFound("Solicitud no existe");

        solicitud.Estado = estado;

        await _context.SaveChangesAsync();

        return Ok("Estado actualizado");
    }

}