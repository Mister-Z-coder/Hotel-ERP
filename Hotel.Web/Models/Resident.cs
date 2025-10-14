using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Hotel.Models
{
    public class Resident : Client
    {
        [MaxLength(10,ErrorMessage = "Le Téléphone ne doit pas depasser 10 caractères.")]
        [Required(ErrorMessage ="Téléphone du client requis")]
        [Display(Name="Téléphone du client")]
        public string PhoneClient { get; set; }

        [Required(ErrorMessage = "Numéro pièce du client requis")]
        [Display(Name = "Numéro pièce du client")]
        public string NumpieceId { get; set; }

        [Required(ErrorMessage = "Type document du client requis")]
        [Display(Name = "Type de document")]
        public string TypedocId { get; set; }
    }
}
