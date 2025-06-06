using System.ComponentModel.DataAnnotations;

namespace SistemaTEA.Models
{
    public class ModuloADOS2
    {
        [Key]
        public int ModuloID { get; set; }

        [Required, StringLength(50)]
        public string NombreModulo { get; set; }

        [StringLength(200)]
        public string Descripcion { get; set; }

        public int EdadMinima { get; set; }

        public int EdadMaxima { get; set; }

        public bool EsActivo { get; set; } = true;
    }

}
