namespace ProyectoAnalisis.Controllers
{
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.EntityFrameworkCore;
    using ProyectoAnalisis.Data;
    using ProyectoAnalisis.DTOs;
    using ProyectoAnalisis.Models;

    [Authorize(Roles = "Administrador")]
    [Route("api/admin")]
    [ApiController]
    public class AdminController : ControllerBase
    {
        private readonly AppDbContext _context;

        public AdminController(AppDbContext context)
        {
            _context = context;
        }

        // 🟦 CREAR CURSO
        [HttpPost("curso")]
        public async Task<IActionResult> CrearCurso(CrearCursoDto dto)
        {
            var curso = new Curso
            {
                NombreCurso = dto.NombreCurso,
                Descripcion = dto.Descripcion,
                DuracionHoras = dto.DuracionHoras,
                Modalidad = dto.Modalidad,
                CupoMaximo = dto.CupoMaximo,
                IdDocente = dto.IdDocente,
                FechaInicio = dto.FechaInicio.ToUniversalTime(),
                PrecioCurso = dto.PrecioCurso
            };

            _context.Cursos.Add(curso);
            await _context.SaveChangesAsync();

            return Ok(new { mensaje = "Curso creado correctamente" });
        }

        [Authorize(Roles = "Administrador,Docente")]
        [HttpPost("subir-nota")]
        public async Task<IActionResult> SubirNota(SubirNotaDto dto)
        {
            if (dto.IdCurso <= 0)
                return BadRequest("Curso inválido");

            if (dto.IdUsuario <= 0)
                return BadRequest("Usuario inválido");

            if (dto.Nota < 0 || dto.Nota > 100)
                return BadRequest("Nota inválida");

            var inscripcion = await _context.Inscripciones
                .FirstOrDefaultAsync(i =>
                    i.IdUsuario == dto.IdUsuario &&
                    i.IdCurso == dto.IdCurso);

            if (inscripcion == null)
                return NotFound("El estudiante no está inscrito en ese curso");

            inscripcion.NotaFinal = dto.Nota;

            await _context.SaveChangesAsync();

            return Ok("Nota actualizada");
        }
        [Authorize(Roles = "Administrador,Docente")]
        [HttpPost("registrar-asistencia")]
        public async Task<IActionResult> RegistrarAsistencia(RegistrarAsistenciaDto dto)
        {
      
            var existe = await _context.Inscripciones
                .AnyAsync(i => i.IdInscripcion == dto.IdInscripcion);

            if (!existe)
                return BadRequest("Inscripción no existe");

            var hoy = DateTime.UtcNow.Date;

            var yaExiste = await _context.Asistencias.AnyAsync(a =>
                a.IdInscripcion == dto.IdInscripcion &&
                a.Fecha >= hoy &&
                a.Fecha < hoy.AddDays(1));

            if (yaExiste)
                return BadRequest("Ya se registró asistencia hoy");


            var asistencia = new Asistencia
            {
                IdInscripcion = dto.IdInscripcion,
                Fecha = DateTime.UtcNow,
                Presente = dto.Presente
            };

            _context.Asistencias.Add(asistencia);
            await _context.SaveChangesAsync();

            return Ok("Asistencia registrada");
        }
        [Authorize(Roles = "Administrador,Docente")]
        [HttpPost("generar-diploma")]
        public async Task<IActionResult> GenerarDiploma(int idInscripcion)
        {
            var inscripcion = await _context.Inscripciones
                .FirstOrDefaultAsync(i => i.IdInscripcion == idInscripcion);

            if (inscripcion == null)
                return BadRequest("Inscripción no existe");

            if (inscripcion.NotaFinal < 61)
                return BadRequest("El estudiante no aprobó");

            // 🔥 evitar duplicados
            var existe = await _context.Diplomas
                .AnyAsync(d => d.IdInscripcion == idInscripcion);

            if (existe)
                return BadRequest("Ya existe diploma");

            // 🔥 generar código único
            var codigo = Guid.NewGuid().ToString().Substring(0, 8).ToUpper();

            var diploma = new Diploma
            {
                IdInscripcion = idInscripcion,
                FechaEmision = DateTime.UtcNow,
                CodigoVerificacion = codigo
            };

            _context.Diplomas.Add(diploma);
            await _context.SaveChangesAsync();

            return Ok(new
            {
                mensaje = "Diploma generado",
                codigo = codigo
            });
        }
        [Authorize(Roles = "Administrador")]
        [HttpPost("crear-producto")]
        public async Task<IActionResult> CrearProducto(CrearProductoDto dto)
        {
            if (dto.Precio <= 0)
                return BadRequest("Precio inválido");

            if (dto.Stock < 0)
                return BadRequest("Stock inválido");

            var producto = new Producto
            {
                Nombre = dto.Nombre,
                Descripcion = dto.Descripcion,
                Precio = dto.Precio,
                Stock = dto.Stock
            };

            _context.Productos.Add(producto);
            await _context.SaveChangesAsync();

            return Ok("Producto creado");
        }
    }
}
