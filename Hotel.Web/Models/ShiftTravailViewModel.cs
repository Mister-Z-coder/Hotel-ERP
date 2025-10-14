using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Hotel.Models
{
    public class ShiftTravailViewModel
    {
        public ShiftTravail ShiftTravail { get; set; }
        public FinShiftTravail FinShiftTravail { get; set; }
        public bool IsMultishift { get; set; }
        public IEnumerable<SelectListItem> AgentList { get; set; }
        public IEnumerable<SelectListItem> PosteTravailList { get; set; }
        public IEnumerable<FinShiftTravail> FinShiftTravails { get; set; }

        public string SearchDate { get; set; }
        public string SearchAgent { get; set; }
        public string SearchPoste { get; set; }
    }
}
