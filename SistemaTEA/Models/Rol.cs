using System.ComponentModel.DataAnnotations;

namespace SistemaTEA.Models
{
    public class Rol
    {
        [Key]
        public int RolID { get; set; }

        [Required, StringLength(50)]
        public string NombreRol { get; set; }

        [StringLength(200)]
        public string Descripcion { get; set; }
    }

}
