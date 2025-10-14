using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Hotel.Models
{
    public class Service
    {
        public int ServiceId { get; set; }


        [Required(ErrorMessage = "Nom service requis")]
        [Display(Name = "Nom service")]
        public string NomService { get; set; }


        [Required(ErrorMessage = "Description service requis")]
        [Display(Name = "Description service")]
        public string DescriptionService { get; set; }

        [Required(ErrorMessage = "Prix service requis")]
        [Display(Name = "Prix service  ($)")]
        public float PrixService { get; set; }

        [NotMapped]
        [Display(Name = "Photo service")]
        public IFormFile PhotoService { get; set; }

        // Propriété pour stocker le chemin de la photo après l'upload
        public string PhotoPath { get; set; }
        public ICollection<Inclure> Inclures { get; set; }


    }
}
