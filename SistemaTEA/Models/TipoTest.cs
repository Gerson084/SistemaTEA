using System.ComponentModel.DataAnnotations;

namespace SistemaTEA.Models
{
    public class TipoTest
    {
        [Key]
        public int TipoTestID { get; set; }

        [Required, StringLength(50)]
        public string NombreTest { get; set; }

        [StringLength(200)]
        public string Descripcion { get; set; }

        public bool EsActivo { get; set; } = true;
    }

}
