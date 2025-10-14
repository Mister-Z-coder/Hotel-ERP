using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Hotel.Models
{
    public class PosteTravailViewModel
    {
        public PosteTravail PosteTravail { get; set; }
        public Caisse? Caisse { get; set; }
        [Display(Name ="Type poste")]
        [Required(ErrorMessage ="Type poste requis")]
        public string TypePoste { get; set; }

    }
}
