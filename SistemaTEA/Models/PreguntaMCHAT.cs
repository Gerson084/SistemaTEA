using System.ComponentModel.DataAnnotations;

namespace SistemaTEA.Models
{
    public class PreguntaMCHAT
    {
        [Key]
        public int PreguntaID { get; set; }

        public int NumeroPregunta { get; set; }

        [Required]
        public string TextoPregunta { get; set; }

        public bool EsActiva { get; set; } = true;
    }

}
