using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Hotel.Models
{
    public class TypeMouvement
    {
        public int TypeMouvementId { get; set; }

        [Required(ErrorMessage = "Titre type mouvement requis")]
        [Display(Name ="Titre type mouvement")]
        public string TitreTypeMouvement { get; set; }
    }
}
