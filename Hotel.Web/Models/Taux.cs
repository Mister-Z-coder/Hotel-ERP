using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Hotel.Models
{
    public class Taux
    {
        public int TauxId { get; set; }

        [ForeignKey("DeviseId")]
        [Required(ErrorMessage = "Devise du taux requis")]
        [Display(Name = "Devise")]
        public int DeviseId { get; set; }
        public Devise Devise { get; set; }

        [Required(ErrorMessage = "Date du taux requis")]
        [Display(Name = "Date taux")]
        public DateTime DateTaux { get; set; }

        [Required(ErrorMessage = "Valeur taux requis")]
        [Display(Name = "Valeur taux")]
        public float ValeurTaux { get; set; }
    }
}
