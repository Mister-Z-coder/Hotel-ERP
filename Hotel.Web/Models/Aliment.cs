using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Hotel.Models
{
    public class Aliment
    {
        public int AlimentId { get; set; }


        [Required(ErrorMessage = "Titre aliment requis")]
        [Display(Name = "Titre aliment reglement")]
        public string TitreAliment { get; set; }


        [Required(ErrorMessage = "Unité aliment requis")]
        [Display(Name = "Unité aliment")]
        public string UniteAliment { get; set; }

        [Required(ErrorMessage = "Prix unitaire requis")]
        [Display(Name = "Prix unitaire ($)")]
        public float PrixUnitAliment { get; set; }


        [Required(ErrorMessage = "Quantité stock requis")]
        [Display(Name = "Quantité stock")]
        public int QuantiteStock { get; set; }

        [NotMapped]
        [Display(Name = "Photo de l'aliment")]
        public IFormFile PhotoAliment { get; set; }

        // Propriété pour stocker le chemin de la photo après l'upload
        public string PhotoPath { get; set; }
        public ICollection<HPrixUnitAliment> PrixAliments { get; set; } = new List<HPrixUnitAliment>();
        public ICollection<Fournir> Fournirs { get; set; }
        public ICollection<Comporter> Comporters { get; set; }
        public ICollection<Mouvoir> Mouvoirs { get; set; }


    }
}
