using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace SistemaTEA.Models
{
    public class Paciente
    {
        [Key]
        public int PacienteID { get; set; }

        [Required, StringLength(100)]
        public string Nombre { get; set; }

        [Required, StringLength(100)]
        public string Apellido { get; set; }

        [Required]
        public DateTime FechaNacimiento { get; set; }

        [Required, StringLength(1)]
        public string Genero { get; set; } // 'M' o 'F'

        public int PadreID { get; set; }

        public int? PsicologoAsignadoID { get; set; }

        public DateTime FechaRegistro { get; set; } = DateTime.Now;

        public string Observaciones { get; set; }

        [ForeignKey("PadreID")]
        public virtual Usuario Padre { get; set; }

        [ForeignKey("PsicologoAsignadoID")]
        public virtual Usuario PsicologoAsignado { get; set; }
    }

}
