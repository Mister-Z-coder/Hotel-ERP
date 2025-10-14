using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Hotel.Models
{
    public class Chambre
    {

        public int ChambreId { get; set; }

        [Required(ErrorMessage = "Numéro chambre requis")]
        [Display(Name = "Numéro chambre")]
        public int NumeroChambre { get; set; }

        [Required(ErrorMessage = "Type de chambre requis")]
        [Display(Name="Type de chambre")]
        public string TypeChambre { get; set; }

        [Required(ErrorMessage = "Prix nuit chambre requis")]
        [Display(Name = "Prix nuit chambre ($)")]
        public double PrixNuit { get; set; }

        [Required(ErrorMessage = "Prix heure chambre requis")]
        [Display(Name = "Prix heure chambre ($)")]
        public double PrixHeure { get; set; }

        [Required(ErrorMessage = "Statut chambre requis")]
        [Display(Name = "Statut chambre")]
        public string StatutChambre { get; set; } = "Libre|Sale";

        [Required(ErrorMessage = "Capacité maximale chambre requis")]
        [Display(Name = "Capacité maximale chambre")]
        public int CapaciteMaxChambre { get; set; }
    }
}
