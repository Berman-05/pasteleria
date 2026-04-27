namespace ProyectoAnalisis.DTOs
{
    public class RegisterDto
    {
        public string NombreCompleto { get; set; }
        public DateTime FechaNacimiento { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
    }
}