using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProyectoAnalisis.Models
{
    [Table("inscripcion")]
    public class Inscripcion
    {
        [Key]
        [Column("id_inscripcion")]
        public int IdInscripcion { get; set; }

        [Column("id_usuario")]
        public int IdUsuario { get; set; }

        [Column("id_curso")]
        public int IdCurso { get; set; }

        [Column("fecha_inscripcion")]
        public DateTime FechaInscripcion { get; set; }

        [Column("estado_pago")]
        public string EstadoPago { get; set; } = "Pendiente";

        [Column("nota_final")]
        [ForeignKey("IdUsuario")]
        public Usuario Usuario { get; set; }
        public decimal NotaFinal { get; set; }
        [ForeignKey("IdCurso")]
        public Curso Curso { get; set; }
    }
}