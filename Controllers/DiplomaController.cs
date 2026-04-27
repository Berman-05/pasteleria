using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProyectoAnalisis.Data;

[ApiController]
[Route("api/[controller]")]
public class DiplomaController : ControllerBase
{
    private readonly AppDbContext _context;

    public DiplomaController(AppDbContext context)
    {
        _context = context;
    }

    [HttpGet("validar/{codigo}")]
    public async Task<IActionResult> ValidarDiploma(string codigo)
    {
        var diploma = await _context.Diplomas
            .Include(d => d.Inscripcion)
            .ThenInclude(i => i.Curso)
            .Include(d => d.Inscripcion)
            .ThenInclude(i => i.Usuario)
            .FirstOrDefaultAsync(d => d.CodigoVerificacion == codigo);

        if (diploma == null)
            return NotFound("Diploma inválido");

        return Ok(new
        {
            estudiante = diploma.Inscripcion.Usuario.NombreCompleto,
            curso = diploma.Inscripcion.Curso.NombreCurso,
            fecha = diploma.FechaEmision
        });
    }
}
