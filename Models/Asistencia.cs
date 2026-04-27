using ProyectoAnalisis.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

[Table("asistencia")]
public class Asistencia
{
    [Key]
    [Column("id_asistencia")]
    public int IdAsistencia { get; set; }

    [Column("id_inscripcion")]
    public int IdInscripcion { get; set; }

    [Column("fecha")]
    public DateTime Fecha { get; set; }

    [Column("presente")]
    public bool Presente { get; set; }

   
    [ForeignKey("IdInscripcion")]
    public Inscripcion Inscripcion { get; set; }
}