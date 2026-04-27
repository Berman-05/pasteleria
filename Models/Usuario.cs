using System.ComponentModel.DataAnnotations.Schema;

namespace ProyectoAnalisis.Models
{
    public class Usuario
    {
        public int IdUsuario { get; set; }
        public string NombreCompleto { get; set; }
        public DateTime FechaNacimiento { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public string PasswordHash { get; set; }
        public int IdRol { get; set; }

        public Rol Rol { get; set; }
    }
}