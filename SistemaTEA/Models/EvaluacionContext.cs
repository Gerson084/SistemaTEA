using Microsoft.EntityFrameworkCore;
using System.Data;
using System.Diagnostics;

namespace SistemaTEA.Models
{
    public class EvaluacionContext : DbContext
    {
        public EvaluacionContext(DbContextOptions<EvaluacionContext> options)
            : base(options)
        {
        }

        public DbSet<Rol> Roles { get; set; }
        public DbSet<Usuario> Usuarios { get; set; }
        public DbSet<Paciente> Pacientes { get; set; }
        public DbSet<TipoTest> TiposTest { get; set; }
        public DbSet<ModuloADOS2> ModulosADOS2 { get; set; }
        public DbSet<AreaADIR> AreasADIR { get; set; }
        public DbSet<PreguntaMCHAT> PreguntasMCHAT { get; set; }
        public DbSet<PreguntaADOS2> PreguntasADOS2 { get; set; }
        public DbSet<PreguntaADIR> PreguntasADIR { get; set; }
        public DbSet<Evaluacion> Evaluaciones { get; set; }
        public DbSet<RespuestaMCHAT> RespuestasMCHAT { get; set; }
        public DbSet<RespuestaADOS2> RespuestasADOS2 { get; set; }
        public DbSet<RespuestaADIR> RespuestasADIR { get; set; }

    }
}
