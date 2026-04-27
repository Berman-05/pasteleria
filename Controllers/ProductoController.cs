using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProyectoAnalisis.Data;

[ApiController]
[Route("api/[controller]")]
public class ProductoController : ControllerBase
{
    private readonly AppDbContext _context;

    public ProductoController(AppDbContext context)
    {
        _context = context;
    }

    [HttpGet("productos")]
    public async Task<IActionResult> ObtenerProductos()
    {
        var productos = await _context.Productos.ToListAsync();
        return Ok(productos);
    }
    [Authorize(Roles = "Administrador")]
    [HttpPut("{id}")]
    public async Task<IActionResult> EditarProducto(int id, CrearProductoDto dto)
    {
        var producto = await _context.Productos.FindAsync(id);

        if (producto == null)
            return NotFound("Producto no encontrado");

        // 🔥 validaciones
        if (dto.Precio <= 0)
            return BadRequest("Precio inválido");

        if (dto.Stock < 0)
            return BadRequest("Stock inválido");

        // ✏️ actualizar campos
        producto.Nombre = dto.Nombre;
        producto.Descripcion = dto.Descripcion;
        producto.Precio = dto.Precio;
        producto.Stock = dto.Stock;

        await _context.SaveChangesAsync();

        return Ok(producto);
    }
    [Authorize(Roles = "Administrador")]
    [HttpDelete("{id}")]
    public async Task<IActionResult> EliminarProducto(int id)
    {
        var producto = await _context.Productos.FindAsync(id);

        if (producto == null)
            return NotFound("Producto no encontrado");

        _context.Productos.Remove(producto);
        await _context.SaveChangesAsync();

        return Ok("Producto eliminado");
    }
}