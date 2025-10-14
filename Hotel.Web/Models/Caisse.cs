using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Hotel.Models
{
    public class Caisse:PosteTravail
    {
        [Required(ErrorMessage = "Titre caisse requis")]
        [Display(Name = "Titre caisse")]
        public string TitreCaisse { get; set; }

        [Required(ErrorMessage = "Bar requis")]
        [Display(Name = "Bar")]
        public string BarCaisse { get; set; }
    }
}
