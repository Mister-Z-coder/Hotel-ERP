using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Hotel.Models
{
    public class Agent
    {
        public int AgentId { get; set; }

        [MaxLength(30, ErrorMessage ="Le Nom ne doit pas depasser 30 caractères.")]
        [Required(ErrorMessage ="Nom de l\'agent requis")]
        [Display(Name="Nom de l\'agent ")]
        public string NomAgent { get; set; }

        [MaxLength(15, ErrorMessage = "Le Prénom ne doit pas depasser 15 caractères.")]
        [Required(ErrorMessage = "Prénom de l\'agent requis")]
        [Display(Name = "Prénom de l\'agent ")]
        public string PrenomAgent { get; set; }

        [MaxLength(1)]
        [Required(ErrorMessage = "Sexe de l\'agent requis")]
        [Display(Name = "Sexe de l\'agent")]
        public string SexeAgent { get; set; }

        [Required(ErrorMessage = "Lieu de naissance de l\'agent requis")]
        [Display(Name = "Lieu de naissance de l\'agent ")]
        public string Lieu_nais_Agent { get; set; }

        [Required(ErrorMessage = "Date de naissance de l\'agent requis")]
        [Display(Name = "Date de naissance de l\'agent ")]
        public DateTime Date_nais_Agent { get; set; }

        [Required(ErrorMessage = "Nationalité de l\'agent requis")]
        [Display(Name = "Nationalité de l\'agent ")]
        public string Nationalite_Agent { get; set; }

        [Required(ErrorMessage = "Fonction de l\'agent requis")]
        [Display(Name = "Fonction de l\'agent ")]
        public string Fonction_Agent { get; set; }

        [NotMapped]
        [Display(Name = "Photo de l'agent")]
        public IFormFile PhotoAgent { get; set; }

        // Propriété pour stocker le chemin de la photo après l'upload
        public string PhotoPath { get; set; }
    }
}
