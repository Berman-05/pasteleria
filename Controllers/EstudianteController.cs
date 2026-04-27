using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProyectoAnalisis.Data;
using ProyectoAnalisis.DTOs;
using ProyectoAnalisis.Models;
using System.Security.Claims;

[Authorize]
[Route("api/estudiante")]
[ApiController]
public class EstudianteController : ControllerBase
{
    private readonly AppDbContext _context;

    public EstudianteController(AppDbContext context)
    {
        _context = context;
    }

    [HttpPost("inscribirse")]
    public async Task<IActionResult> Inscribirse(InscripcionDTO dto)
    {
        // 🟦 1. Obtener usuario desde el token
        var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

        // 🟦 2. Verificar que el curso existe
        var curso = await _context.Cursos.FindAsync(dto.IdCurso);
        if (curso == null)
            return NotFound("Curso no existe");

        // 🟦 3. Verificar si ya está inscrito
        var yaInscrito = await _context.Inscripciones
            .AnyAsync(i => i.IdUsuario == userId && i.IdCurso == dto.IdCurso);

        if (yaInscrito)
            return BadRequest("Ya estás inscrito en este curso");

        // 🟦 4. Verificar cupo disponible
        var inscritos = await _context.Inscripciones
            .CountAsync(i => i.IdCurso == dto.IdCurso);

        if (inscritos >= curso.CupoMaximo)
            return BadRequest("Cupo lleno");

        // 🟦 5. Crear inscripción
        var inscripcion = new Inscripcion
        {
            IdUsuario = userId,
            IdCurso = dto.IdCurso,
            FechaInscripcion = DateTime.UtcNow,
            EstadoPago = "Pendiente"
        };

        _context.Inscripciones.Add(inscripcion);
        await _context.SaveChangesAsync();

        return Ok(new { mensaje = "Inscripción realizada correctamente" });
    }
    [Authorize]
    [HttpGet("mis-cursos")]
    public async Task<IActionResult> MisCursos()
    {
        var userIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier);
        if (userIdClaim == null)
            return Unauthorized();

        var userId = int.Parse(userIdClaim.Value);

        var cursos = await _context.Inscripciones
            .Where(i => i.IdUsuario == userId)
            .Include(i => i.Curso)
            .Select(i => new
            {
                i.IdCurso,
                i.Curso.NombreCurso,
                i.Curso.Descripcion,
                i.Curso.Modalidad,
                i.Curso.FechaInicio
            })
            .ToListAsync();

        return Ok(cursos);
    }
    [Authorize]
    [HttpGet("mis-notas")]
    public async Task<IActionResult> MisNotas()
    {
        var userId = int.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)!.Value);

        var notas = await _context.Inscripciones
            .Where(i => i.IdUsuario == userId)
            .Include(i => i.Curso)
            .Select(i => new
            {
                i.IdCurso,
                i.Curso.NombreCurso,
                i.NotaFinal
            })
            .ToListAsync();

        return Ok(notas);
    }
    [Authorize]
    [HttpGet("mi-asistencia")]
    public async Task<IActionResult> MiAsistencia()
    {
        var userId = int.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)!.Value);

        var asistencias = await _context.Asistencias
            .Include(a => a.Inscripcion)
            .Where(a => a.Inscripcion.IdUsuario == userId)
            .GroupBy(a => a.Inscripcion.IdCurso)
            .Select(g => new
            {
                IdCurso = g.Key,
                TotalClases = g.Count(),
                Asistencias = g.Count(a => a.Presente)
            })
            .ToListAsync();

        return Ok(asistencias);
    }
    [Authorize]
    [HttpGet("mi-diploma/{idCurso}")]
    public async Task<IActionResult> MiDiploma(int idCurso)
    {
        var userId = int.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)!.Value);

        var diploma = await _context.Diplomas
            .Include(d => d.Inscripcion)
            .Where(d =>
                d.Inscripcion.IdUsuario == userId &&
                d.Inscripcion.IdCurso == idCurso)
            .Select(d => new
            {
                d.FechaEmision,
                d.CodigoVerificacion
            })
            .FirstOrDefaultAsync();

        if (diploma == null)
            return NotFound("No tienes diploma");

        return Ok(diploma);
    }
}