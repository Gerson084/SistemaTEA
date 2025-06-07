using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace SistemaTEA.Models
{
    [Table("Pacientes")]
    public class Paciente
    {
        [Key]
        public int PacienteID { get; set; }

        [Required(ErrorMessage = "El nombre es requerido")]
        [StringLength(100, ErrorMessage = "El nombre no puede exceder 100 caracteres")]
        [Display(Name = "Nombre")]
        public string Nombre { get; set; } = string.Empty;

        [Required(ErrorMessage = "El apellido es requerido")]
        [StringLength(100, ErrorMessage = "El apellido no puede exceder 100 caracteres")]
        [Display(Name = "Apellido")]
        public string Apellido { get; set; } = string.Empty;

        [Required(ErrorMessage = "La fecha de nacimiento es requerida")]
        [DataType(DataType.Date)]
        [Display(Name = "Fecha de Nacimiento")]
        public DateTime FechaNacimiento { get; set; }

        [Required(ErrorMessage = "El género es requerido")]
        [StringLength(1)]
        [RegularExpression("^[MF]$", ErrorMessage = "El género debe ser M o F")]
        [Display(Name = "Género")]
        public string Genero { get; set; } = string.Empty;

        [Column("PadreID")]
        public int PadreID { get; set; }

        [Column("PsicologoAsignadoID")]
        public int? PsicologoAsignadoID { get; set; }

        [Column("FechaRegistro")]
        public DateTime FechaRegistro { get; set; } = DateTime.Now;

        [StringLength(int.MaxValue)]
        [Display(Name = "Observaciones")]
        public string? Observaciones { get; set; }

        // Propiedades de navegación
        [ForeignKey("PadreID")]
        public virtual Usuario? Padre { get; set; }

        [ForeignKey("PsicologoAsignadoID")]
        public virtual Usuario? PsicologoAsignado { get; set; }

        [Display(Name = "Consentimiento Otorgado")]
        public bool? ConsentimientoOtorgado { get; set; }

        [Display(Name = "Fecha de Consentimiento")]
        public DateTime? FechaConsentimiento { get; set; }

        [StringLength(45)]
        [Display(Name = "IP de Consentimiento")]
        public string? IPConsentimiento { get; set; }
    }
}