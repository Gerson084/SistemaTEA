using System.ComponentModel.DataAnnotations;

namespace SistemaTEA.Models
{
    public class AreaADIR
    {
        [Key]
        public int AreaID { get; set; }

        [Required, StringLength(100)]
        public string NombreArea { get; set; }

        [StringLength(200)]
        public string Descripcion { get; set; }

        public bool EsActivo { get; set; } = true;
    }

}
