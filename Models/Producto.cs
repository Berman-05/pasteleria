using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

[Table("producto")]
public class Producto
{
    [Key]
    [Column("id_producto")]
    public int IdProducto { get; set; }

    [Column("nombre")]
    public string Nombre { get; set; }

    [Column("descripcion")]
    public string Descripcion { get; set; }

    [Column("precio")]
    public decimal Precio { get; set; }

    [Column("stock")]
    public int Stock { get; set; }
}