using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Hotel.Models
{
    public class Mouvoir
    {
        [ForeignKey("MouvementId")]
        public int MouvementId { get; set; }
        public Mouvement Mouvement { get; set; }
        [ForeignKey("AlimentId")]
        public int AlimentId { get; set; }
        public Aliment Aliment { get; set; }

        [Required(ErrorMessage = "Quantité mouvement requis")]
        [Display(Name = "Quantité mouvement")]
        public int QteMouv { get; set; }
    }
}
