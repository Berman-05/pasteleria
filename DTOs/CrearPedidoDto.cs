public class CrearPedidoDto
{
    public List<ItemPedidoDto> Items { get; set; }
}

public class ItemPedidoDto
{
    public int IdProducto { get; set; }
    public int Cantidad { get; set; }
}