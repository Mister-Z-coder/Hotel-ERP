using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Hotel.Models
{
    public class Mouvement
    {
        public int MouvementId { get; set; }

        [Required(ErrorMessage = "Agent requis")]
        [Display(Name = "Agent")]
        [ForeignKey("AgentId")]
        public int AgentId { get; set; }
        public virtual Agent Agent { get; set; }


        [Required(ErrorMessage = "TypeMouvement requis")]
        [Display(Name = "TypeMouvement")]
        [ForeignKey("TypeMouvementId")]
        public int TypeMouvementId { get; set; }
        public virtual TypeMouvement TypeMouvement { get; set; }

        [Display(Name = "Date mouvement")]
        public DateTime DateMouvement { get; set; }

        [Display(Name = "Heure mouvement")]
        public DateTime HeureMouvement { get; set; }


        [Required(ErrorMessage = "Nature mouvement requis")]
        [Display(Name = "Nature mouvement")]
        public string NatureMouvement { get; set; }

        //[NotMapped]
        //[Display(Name = "Etat demande")]

        //public string EtatDemande{get;set;}
        //public ICollection<Comprendre> Comprendres { get; set; }
        //public ICollection<Comporter> Comporters { get; set; }

        public ICollection<Bouger> Bougers { get; set; }
        public ICollection<Mouvoir> Mouvoirs { get; set; }
        public Mouvement()
        {
            DateMouvement = DateTime.Today; // Initialiser à la date du jourjour
            HeureMouvement = DateTime.Now; // Initialiser à la date du jourjour
        }


    }
}
