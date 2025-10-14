using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Hotel.Models
{
    public class MaintenanceChambreViewModel
    {
        public MaintenanceChambre MaintenanceChambre { get; set; }
        public FinMaintenanceChambre FinMaintenanceChambre { get; set; }
        public IEnumerable<FinMaintenanceChambre> FinMaintenanceChambres { get;set;}
        public bool IsMultimaintenance { get; set; }
        public IEnumerable<SelectListItem> AgentList { get; set; }
        public IEnumerable<SelectListItem> ChambreList { get; set; }
        public string SearchDate { get; set; }
        public string SearchAgent { get; set; }
        public string SearchChambre { get; set; }

    }
}
