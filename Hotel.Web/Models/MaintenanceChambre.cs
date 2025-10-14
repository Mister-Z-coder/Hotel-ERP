using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Hotel.Models
{
    public class MaintenanceChambre
    {
        public int MaintenanceChambreId { get; set; }

        [ForeignKey("AgentId")]
        [Required(ErrorMessage = "Agent requis")]
        [Display(Name = "Agent")]
        public int AgentId { get; set; }
        public Agent Agent { get; set; }

        [ForeignKey("ChambreId")]
        [Required(ErrorMessage = "Chambre requis")]
        [Display(Name = "Chambre")]
        public int ChambreId { get; set; }
        public Chambre Chambre { get; set; }

        [Required(ErrorMessage = "Date debut maintenance requis")]
        [Display(Name = "Date debut maintenance")]
        public DateTime? DateDebutMaintenance { get; set; }

        [Required(ErrorMessage = "Heure debut maintenance requis")]
        [Display(Name = "Heure debut maintenance")]
        public DateTime? HeureDebutMaintenance { get; set; }

        [Required(ErrorMessage = "Observation maintenance requis")]
        [Display(Name = "Observation maintenance")]
        public string ObservationDebutMaintenance { get; set; }



    }
}
