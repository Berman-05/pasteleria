using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProyectoAnalisis.Data;

[ApiController]
[Route("api/[controller]")]
public class PedidoController : ControllerBase
{
    private readonly AppDbContext _context;

    public PedidoController(AppDbContext context)
    {
        _context = context;
    }
    [Authorize]
    [HttpPost]
    public async Task<IActionResult> CrearPedido(CrearPedidoDto dto)
    {
        var userId = int.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)!.Value);

        if (dto.Items == null || !dto.Items.Any())
            return BadRequest("El pedido está vacío");

        var pedido = new Pedido
        {
            IdUsuario = userId,
            Fecha = DateTime.UtcNow,
            Estado = "Pendiente",
            Total = 0
        };

        _context.Pedidos.Add(pedido);
        await _context.SaveChangesAsync();

        decimal total = 0;

        foreach (var item in dto.Items)
        {
            var producto = await _context.Productos.FindAsync(item.IdProducto);

            if (producto == null)
                return BadRequest($"Producto {item.IdProducto} no existe");

            if (producto.Stock < item.Cantidad)
                return BadRequest($"Stock insuficiente para {producto.Nombre}");

            var detalle = new DetallePedido
            {
                IdPedido = pedido.IdPedido,
                IdProducto = item.IdProducto,
                Cantidad = item.Cantidad,
                PrecioUnitario = producto.Precio
            };

            total += item.Cantidad * producto.Precio;

            // 🔥 reducir stock
            producto.Stock -= item.Cantidad;

            _context.DetallePedidos.Add(detalle);
        }

        pedido.Total = total;

        await _context.SaveChangesAsync();

        return Ok(new
        {
            mensaje = "Pedido creado",
            pedidoId = pedido.IdPedido,
            total = total
        });
    }
    [Authorize]
    [HttpGet("mis-pedidos")]
    public async Task<IActionResult> MisPedidos()
    {
        var userId = int.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)!.Value);

        var pedidos = await _context.Pedidos
            .Where(p => p.IdUsuario == userId)
            .Include(p => p.Detalles)
            .ThenInclude(d => d.Producto)
            .Select(p => new
            {
                p.IdPedido,
                p.Fecha,
                p.Estado,
                p.Total,
                Detalles = p.Detalles.Select(d => new
                {
                    d.IdProducto,
                    Producto = d.Producto.Nombre,
                    d.Cantidad,
                    d.PrecioUnitario
                })
            })
            .ToListAsync();

        return Ok(pedidos);
    }
    [Authorize(Roles = "Administrador")]
    [HttpPut("cambiar-estado/{id}")]
    public async Task<IActionResult> CambiarEstado(int id, [FromBody] string estado)
    {
        var pedido = await _context.Pedidos.FindAsync(id);

        var estadosValidos = new[] { "Pendiente", "Confirmado", "Entregado" };

        if (!estadosValidos.Contains(estado))
            return BadRequest("Estado inválido");

        if (pedido == null)
            return NotFound("Pedido no encontrado");

        pedido.Estado = estado;

        await _context.SaveChangesAsync();

        return Ok("Estado actualizado");
    }
}
