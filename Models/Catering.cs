using ProyectoAnalisis.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

[Table("catering")]
public class Catering
{
    [Key]
    [Column("id_catering")]
    public int IdCatering { get; set; }

    [Column("id_usuario")]
    public int IdUsuario { get; set; }

    [Column("fecha_evento")]
    public DateTime FechaEvento { get; set; }

    [Column("cantidad_personas")]
    public int CantidadPersonas { get; set; }

    [Column("descripcion")]
    public string Descripcion { get; set; }

    [Column("estado")]
    public string Estado { get; set; }

    [ForeignKey("IdUsuario")]
    public Usuario Usuario { get; set; }
}