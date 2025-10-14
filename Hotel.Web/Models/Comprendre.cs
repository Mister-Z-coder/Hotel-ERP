using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Hotel.Models
{
    public class Comprendre
    {
        [ForeignKey("DemandeId")]
        public int DemandeId { get; set; }
        public Demande Demande { get; set; }
        [ForeignKey("BoissonId")]
        public int BoissonId { get; set; }
        public Boisson Boisson { get; set; }

        [Required(ErrorMessage = "Quantité demandée requis")]
        [Display(Name = "Quantité demandée")]
        public int QteDemande { get; set; }
    }
}
