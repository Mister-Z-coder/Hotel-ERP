using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Hotel.Models
{
    public class Boisson
    {
        public int BoissonId { get; set; }

        [ForeignKey("BrasserieId")]
        [Required(ErrorMessage = "Brasserie boisson requis")]
        [Display(Name = "Brasserie boisson")]
        public int BrasserieId { get; set; }
        public Brasserie Brasserie { get; set; }

        [ForeignKey("CategorieBoissonId")]
        [Required(ErrorMessage = "Categorie boisson requis")]
        [Display(Name = "Categorie boisson")]
        public int CategorieBoissonId { get; set; }
        public CategorieBoisson CategorieBoisson { get; set; }


        [Required(ErrorMessage = "Titre boisson requis")]
        [Display(Name = "Titre boisson")]
        public string TitreBoisson { get; set; }


        [Required(ErrorMessage = "Unité boisson requis")]
        [Display(Name = "Unité boisson")]
        public string UniteBoisson { get; set; }

        [Required(ErrorMessage = "Prix unitaire SB requis")]
        [Display(Name = "Prix unitaire SB ($)")]
        public float PrixUnitSB { get; set; }

        [Required(ErrorMessage = "Prix unitaire LB requis")]
        [Display(Name = "Prix unitaire LB ($)")]
        public float PrixUnitLB { get; set; }


        [Required(ErrorMessage = "Quantité stock requis")]
        [Display(Name = "Quantité stock")]
        public int QuantiteStock { get; set; }

        
        [NotMapped]
        [Display(Name = "Photo boisson")]
        public IFormFile PhotoBoisson { get; set; }

        // Propriété pour stocker le chemin de la photo après l'upload
        public string PhotoPath { get; set; }
        public ICollection<HPrixUnitLBBoisson> PrixLBs { get; set; } = new List<HPrixUnitLBBoisson>();
        public ICollection<HPrixUnitSBBoisson> PrixSBs { get; set; } = new List<HPrixUnitSBBoisson>();
        public ICollection<Procurer> Procurers { get; set; }
        public ICollection<Comprendre> Comprendres { get; set; }
        public ICollection<Bouger> Bougers { get; set; }




    }
}
