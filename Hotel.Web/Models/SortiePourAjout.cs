using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Hotel.Models
{
    public class SortiePourAjout:Mouvement
    {
        [Required(ErrorMessage = "Ajout requis")]
        [Display(Name = "Ajout")]
        [ForeignKey("AjoutId")]
        public int AjoutId { get; set; }
        public virtual Ajout Ajout { get; set; }

        [Required(ErrorMessage = "Motif ajout requis")]
        [Display(Name = "Motif ajout")]
        public string MotifAjout { get; set; }


    }
}
