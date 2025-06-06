using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace SistemaTEA.Models
{
    public class Usuario
    {
        [Key]
        public int UsuarioID { get; set; }

        [Required, StringLength(100)]
        public string Nombre { get; set; }

        [Required, StringLength(100)]
        public string Apellido { get; set; }

        [Required, StringLength(150), EmailAddress]
        public string Email { get; set; }

        [StringLength(20)]
        public string Telefono { get; set; }

        [Required, StringLength(255)]
        public string Contraseña { get; set; }

        public int RolID { get; set; }

        public bool EsActivo { get; set; } = false;

        public DateTime FechaRegistro { get; set; } = DateTime.Now;

        public DateTime? FechaAprobacion { get; set; }

        public int? AprobadoPor { get; set; }

        [ForeignKey("RolID")]
        public virtual Rol Rol { get; set; }

        [ForeignKey("AprobadoPor")]
        public virtual Usuario Aprobador { get; set; }
    }

}
