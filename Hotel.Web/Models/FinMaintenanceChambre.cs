using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Hotel.Models
{
    public class FinMaintenanceChambre : MaintenanceChambre
    {

        //[Required(ErrorMessage = "Date fin shift requis")]
        [Display(Name = "Date fin maintenance")]
        public DateTime? DateFinMaintenance { get; set; }

        //[Required(ErrorMessage = "Heure fin shift requis")]
        [Display(Name = "Heure fin maintenance")]
        public DateTime? HeureFinMaintenance { get; set; }

        //[Required(ErrorMessage = "Observation fin shift requis")]
        [Display(Name = "Observation fin maintenance")]
        public string ObservationFinMaintenance { get; set; }
    }
}
