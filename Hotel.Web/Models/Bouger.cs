using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Hotel.Models
{
    public class Bouger
    {
        [ForeignKey("MouvementId")]
        public int MouvementId { get; set; }
        public Mouvement Mouvement { get; set; }
        [ForeignKey("BoissonId")]
        public int BoissonId { get; set; }
        public Boisson Boisson { get; set; }

        [Required(ErrorMessage = "Quantité bouger requis")]
        [Display(Name = "Quantité bouger")]
        public int QteBouger { get; set; }
    }
}
