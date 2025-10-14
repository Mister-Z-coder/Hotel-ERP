using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Hotel.Models
{
    public class Fournisseur
    {
        public int FournisseurId { get; set; }

        [MaxLength(30, ErrorMessage ="Le Nom ne doit pas depasser 30 caractères.")]
        [Required(ErrorMessage ="Nom du fourniseur requis")]
        [Display(Name="Nom du fournisseur ")]
        public string NomFournisseur { get; set; }

        [MaxLength(10, ErrorMessage = "Le Téléphone ne doit pas depasser 10 caractères.")]
        [Required(ErrorMessage = "Téléphone du fournisseur requis")]
        [Display(Name = "Téléphone du fournisseur")]
        public string PhoneFournisseur { get; set; }

        [Required(ErrorMessage = "Adresse du fournisseur requis")]
        [Display(Name = "Adressee du fournisseur")]
        public string AdresseFournisseur { get; set; }

        [Required(ErrorMessage = "Domaine d\'activité du fournisseur requis")]
        [Display(Name = "Domaine d\'activité du fournisseur")]
        public string DomaineActivite{ get; set; }

        public ICollection<Fournir> Fournirs { get; set; }
        public ICollection<Procurer> Procurers { get; set; }
    }
}
