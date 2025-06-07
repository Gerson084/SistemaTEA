using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace SistemaTEA.Models
{
    public class PreguntaADIR
    {
        [Key]
        public int PreguntaID { get; set; }

        public int AreaID { get; set; }

        public int NumeroPregunta { get; set; }

        [Required]
        public string TextoPregunta { get; set; }

        public string TipoPregunta { get; set; }

        public bool EsActiva { get; set; } = true;

    }

}
