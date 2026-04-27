namespace ProyectoAnalisis.DTOs
{
    public class CrearCursoDto
    {
        public string NombreCurso { get; set; } = null!;
        public string Descripcion { get; set; } = null!;
        public int DuracionHoras { get; set; }
        public string Modalidad { get; set; } = null!;
        public int CupoMaximo { get; set; }
        public int IdDocente { get; set; }
        public DateTime FechaInicio { get; set; }
        public decimal PrecioCurso { get; set; }
    }
}
