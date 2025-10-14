using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Hotel.Models
{
    public class CategorieBoisson
    {
        public int CategorieBoissonId { get; set; }

        [Required(ErrorMessage = "Titre catégorie requis")]
        [Display(Name = "Titre catégorie")]
        public string TitreCategorieBoisson { get; set; }
    }
}
