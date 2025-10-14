using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Hotel.Models
{
    public class Demande
    {
        public int DemandeId { get; set; }

        [Required(ErrorMessage = "Agent requis")]
        [Display(Name = "Agent")]
        [ForeignKey("AgentId")]
        public int AgentId { get; set; }
        public virtual Agent Agent { get; set; }

        [Display(Name = "Date demande")]
        public DateTime DateDemande { get; set; }


        [Required(ErrorMessage = "Type demande requis")]
        [Display(Name = "Type demande")]
        public string TypeDemande { get; set; }
        [NotMapped]
        [Display(Name = "Etat demande")]

        public string EtatDemande{get;set;}
        public ICollection<Comprendre> Comprendres { get; set; }
        public ICollection<Comporter> Comporters { get; set; }
        public Demande()
        {
            DateDemande = DateTime.Today; // Initialiser à la date du jour
        }


    }
}
