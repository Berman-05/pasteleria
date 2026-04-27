using ProyectoAnalisis.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

[Table("diploma")]
public class Diploma
{
    [Key]
    [Column("id_diploma")]
    public int IdDiploma { get; set; }

    [Column("id_inscripcion")]
    public int IdInscripcion { get; set; }

    [Column("fecha_emision")]
    public DateTime FechaEmision { get; set; }

    [Column("codigo_verificacion")]
    public string CodigoVerificacion { get; set; }

    [ForeignKey("IdInscripcion")]
    public Inscripcion Inscripcion { get; set; }
}