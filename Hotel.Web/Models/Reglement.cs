using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Hotel.Models
{
    public class Reglement
    {
        public int ReglementId { get; set; }

        [ForeignKey("ModeReglementId")]
        [Required(ErrorMessage = "Mode de reglement requis")]
        [Display(Name = "Mode de reglement")]
        public int ModeReglementId { get; set; }
        public ModeReglement ModeReglement { get; set; }

        [ForeignKey("DeviseId")]
        [Required(ErrorMessage = "Devise requis")]
        [Display(Name = "Devise")]
        public int DeviseId { get; set; }
        public Devise Devise { get; set; }

        [ForeignKey("TypeReglementId")]
        [Required(ErrorMessage = "Type reglement requis")]
        [Display(Name = "Type reglement")]
        public int TypeReglementId { get; set; }
        public TypeReglement TypeReglement { get; set; }

        [ForeignKey("AgentId")]
        [Required(ErrorMessage = "Agent requis")]
        [Display(Name = "Agent")]
        public int AgentId {get;set;}
        public Agent Agent{ get; set; }

        [ForeignKey("PosteTravailId")]
        [Required(ErrorMessage = "Caisse requis")]
        [Display(Name = "Caisse")]
        public int CaisseId { get; set; }
        public Caisse Caisse { get; set; }

        [Required(ErrorMessage = "Date reglement requis")]
        [Display(Name = "Date reglement")]
        public DateTime DateReglement { get; set; }

        [Required(ErrorMessage = "Heure reglement requis")]
        [Display(Name = "Heure reglement")]
        public DateTime HeureReglement { get; set; }

        [Required(ErrorMessage = "Montant reglement requis")]
        [Display(Name = "Montant reglement")]
        public decimal MontantReglement { get; set; }

        [Required(ErrorMessage = "Motif reglement requis")]
        [Display(Name = "Motif reglement")]
        public string MotifReglement { get; set; }

        public ICollection<ReglementCommande> Commandes { get; set; }
    }
}
