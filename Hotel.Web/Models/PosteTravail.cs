using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Hotel.Models
{
    public class PosteTravail
    {
        public int PosteTravailId { get; set; }

        [Required(ErrorMessage = "Titre poste de travail requis")]
        [Display(Name = "Titre poste de travail")]
        public string TitrePosteTravail { get; set; }
    }
}
