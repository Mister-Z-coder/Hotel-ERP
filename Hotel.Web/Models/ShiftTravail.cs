using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Hotel.Models
{
    public class ShiftTravail
    {
        public int ShiftTravailId { get; set; }

        [ForeignKey("AgentId")]
        [Required(ErrorMessage = "Agent requis")]
        [Display(Name = "Agent")]
        public int AgentId { get; set; }
        public Agent Agent { get; set; }

        [ForeignKey("PosteTravailId")]
        [Required(ErrorMessage = "Poste Travail requis")]
        [Display(Name = "Poste Travail")]
        public int PosteTravailId { get; set; }
        public PosteTravail PosteTravail { get; set; }

        [Required(ErrorMessage = "Date debut shift requis")]
        [Display(Name = "Date debut shift")]
        public DateTime? DateDebutShift { get; set; }

        [Required(ErrorMessage = "Heure debut shiftt requis")]
        [Display(Name = "Heure debut shift")]
        public DateTime? HeureDebutShift { get; set; }

        [Required(ErrorMessage = "Observation debut shift requis")]
        [Display(Name = "Observation debut shift")]
        public string ObservationDebutShift { get; set; }



    }
}
