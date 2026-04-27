using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

[Table("detalle_pedido")]
public class DetallePedido
{
    [Key]
    [Column("id_detalle")]
    public int IdDetalle { get; set; }

    [Column("id_pedido")]
    public int IdPedido { get; set; }

    [Column("id_producto")]
    public int IdProducto { get; set; }

    [Column("cantidad")]
    public int Cantidad { get; set; }

    [Column("precio_unitario")]
    public decimal PrecioUnitario { get; set; }

    [ForeignKey("IdPedido")]
    public Pedido Pedido { get; set; }

    [ForeignKey("IdProducto")]
    public Producto Producto { get; set; }
}