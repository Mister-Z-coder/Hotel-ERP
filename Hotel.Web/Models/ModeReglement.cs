using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Hotel.Models
{
    public class ModeReglement
    {
        public int ModeReglementId{ get; set; }

        [Required(ErrorMessage = "Titre mode reglement requis")]
        [Display(Name = "Titre mode reglement")]
        public string TitreModeReglement { get; set; }
    }
}
