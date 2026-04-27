using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using ProyectoAnalisis.Models;
[Table("pedido")]
public class Pedido
{
    [Key]
    [Column("id_pedido")]
    public int IdPedido { get; set; }

    [Column("id_usuario")]
    public int IdUsuario { get; set; }

    [Column("fecha")]
    public DateTime Fecha { get; set; }

    [Column("total")]
    public decimal Total { get; set; }

    [Column("estado")]
    public string Estado { get; set; }

    [ForeignKey("IdUsuario")]
    public Usuario Usuario { get; set; }

    public ICollection<DetallePedido> Detalles { get; set; }
}