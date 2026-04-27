using Microsoft.EntityFrameworkCore;
using ProyectoAnalisis.Models;

namespace ProyectoAnalisis.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<Usuario> Usuarios { get; set; }
        public DbSet<Rol> Roles { get; set; }
        public DbSet<Curso> Cursos { get; set; }
        public DbSet<Inscripcion> Inscripciones { get; set; }
        public DbSet<Asistencia> Asistencias { get; set; }
        public DbSet<Diploma> Diplomas { get; set; }
        public DbSet<Producto> Productos { get; set; }
        public DbSet<Pedido> Pedidos { get; set; }
        public DbSet<DetallePedido> DetallePedidos { get; set; }
        public DbSet<Catering> Caterings { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // TABLA ROL
            modelBuilder.Entity<Rol>(entity =>
            {
                entity.ToTable("rol");
                entity.HasKey(r => r.IdRol);

                entity.Property(r => r.IdRol).HasColumnName("id_rol");
                entity.Property(r => r.NombreRol).HasColumnName("nombre_rol");
            });

            // TABLA USUARIO
            modelBuilder.Entity<Usuario>(entity =>
            {
                entity.ToTable("usuario");
                entity.HasKey(u => u.IdUsuario);

                entity.Property(u => u.IdUsuario).HasColumnName("id_usuario");
                entity.Property(u => u.NombreCompleto).HasColumnName("nombre_completo");
                entity.Property(u => u.FechaNacimiento)
                    .HasColumnName("fecha_nacimiento")
                    .HasColumnType("date");
                entity.Property(u => u.Username).HasColumnName("username");
                entity.Property(u => u.Email).HasColumnName("email");
                entity.Property(u => u.PasswordHash).HasColumnName("password_hash");
                entity.Property(u => u.IdRol).HasColumnName("id_rol");

                entity.HasOne(u => u.Rol)
                      .WithMany(r => r.Usuarios)
                      .HasForeignKey(u => u.IdRol);
            });
        }
    }
}