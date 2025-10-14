using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Hotel.Models
{
    public class Client
    {
        public int ClientId { get; set; }

        [MaxLength(50, ErrorMessage = "Le Nom ne doit pas depasser 50 caractères.")]
        [Required(ErrorMessage ="Nom du client requis")]
        [Display(Name="Nom complet du client")]
        public string NomClient { get; set; }

        [MaxLength(1)]
        [Required(ErrorMessage = "Sexe du client requis")]
        [Display(Name = "Sexe du client")]
        public string SexeClient { get; set; }

        [Required(ErrorMessage = "Type du client requis")]
        [Display(Name = "Type de client")]
        public string TypeClient { get; set; }
    }
}
