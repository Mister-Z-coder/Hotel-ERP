using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Hotel.Models
{
    public class FinShiftTravail : ShiftTravail
    {

        //[Required(ErrorMessage = "Date fin shift requis")]
        [Display(Name = "Date fin shift")]
        public DateTime? DateFinShift { get; set; }

        //[Required(ErrorMessage = "Heure fin shift requis")]
        [Display(Name = "Heure fin shift")]
        public DateTime? HeureFinShift { get; set; }

        //[Required(ErrorMessage = "Observation fin shift requis")]
        [Display(Name = "Observation fin shift")]
        public string ObservationFinShift { get; set; }
    }
}
