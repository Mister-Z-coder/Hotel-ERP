using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Hotel.Models
{
    public class TypeReglement
    {
        public int TypeReglementId{ get; set; }

        [Required(ErrorMessage = "Titre type reglement requis")]
        [Display(Name ="Titre type reglement")]
        public string TitreTypeReglement{ get; set; }
    }
}
