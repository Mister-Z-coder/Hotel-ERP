using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Hotel.Models
{
    public class Comporter
    {
        [ForeignKey("DemandeId")]
        public int DemandeId { get; set; }
        public Demande Demande { get; set; }
        [ForeignKey("AlimentId")]
        public int AlimentId { get; set; }
        public Aliment Aliment { get; set; }

        [Required(ErrorMessage = "Quantité ordonnée requis")]
        [Display(Name = "Quantité ordonnée")]
        public int QteOrdone { get; set; }
    }
}
