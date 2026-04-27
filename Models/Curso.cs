using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProyectoAnalisis.Models
{
    [Table("curso")] // opcional si usas minúsculas en PostgreSQL
    public class Curso
    {
        [Key]
        [Column("id_curso")]
        public int IdCurso { get; set; }

        [Column("nombre_curso")]
        public string NombreCurso { get; set; } = null!;

        [Column("descripcion")]
        public string Descripcion { get; set; } = null!;

        [Column("duracion_horas")]
        public int DuracionHoras { get; set; }

        [Column("modalidad")]
        public string Modalidad { get; set; } = null!;

        [Column("cupo_maximo")]
        public int CupoMaximo { get; set; }

        [Column("id_docente")]
        public int IdDocente { get; set; }

        [Column("fecha_inicio")]
        public DateTime FechaInicio { get; set; }

        [Column("precio_curso")]
        public decimal PrecioCurso { get; set; }
        public ICollection<Inscripcion> Inscripciones { get; set; }
    }
}