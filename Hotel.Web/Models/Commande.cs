using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Hotel.Models
{
    public class Commande
    {
        

        public int CommandeId { get; set; }

        [Required(ErrorMessage = "Serveur de la commande requis")]
        [Display(Name = "Serveur de la commande")]
        [ForeignKey("AgentId")]
        public int AgentId { get; set; }
        public virtual Agent Agent { get; set; }

        [Required(ErrorMessage = "Client de la commande requis")]
        [Display(Name = "Client de la commande")]
        [ForeignKey("ClientId")]
        public int ClientId { get; set; }
        public virtual Client Client { get; set; }

        [Required(ErrorMessage = "Date de la commande requis")]
        [Display(Name = "Date de la commande")]
        public DateTime DateCommande { get; set; }

        [Required(ErrorMessage = "Heure de la commande requis")]
        [Display(Name = "Heure de la commande")]
        public DateTime HeureCommande { get; set; }


        [Required(ErrorMessage = "Type commande requis")]
        [Display(Name = "Type commande")]
        public string TypeCommande { get; set;}
        public decimal MontantTotal { get; set; }

        public ICollection<ReglementCommande> Reglements { get; set; }
        public Commande()
        {
            DateCommande = DateTime.Today; // Initialiser à la date du jour
            HeureCommande = DateTime.Now;   // Initialiser à l'heure actuelle
        }

    }
}
