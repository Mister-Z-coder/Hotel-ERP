using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Hotel.Models
{
    public class Livraison : Mouvement
    {
        [Required(ErrorMessage = "Approvisionnement requis")]
        [Display(Name = "Approvisionnement")]
        [ForeignKey("ApprovisionnementId")]
        public int ApprovisionnementId { get; set; }
        public virtual Approvisionnement Approvisionnement { get; set; }

        [Required(ErrorMessage = "Fournisseur requis")]
        [Display(Name = "Fournisseur")]
        [ForeignKey("FournisseurId")]
        public int FournisseurId { get; set; }
        public virtual Fournisseur Fournisseur { get; set; }


        [Required(ErrorMessage = "Montant livraison requis")]
        [Display(Name = "Montant livraison ($)")]
        public decimal MontantLivraison { get; set; }

        [Display(Name = "Date livraison")]
        public DateTime DateLivraison { get; set; }

        [Display(Name = "Heure livraison")]
        public DateTime HeureLivraison { get; set; }
        public Livraison()
        {
            DateLivraison = DateTime.Today; // Initialiser à la date du jourjour
            HeureLivraison = DateTime.Now; // Initialiser à la date du jourjour
        }

    }
}
