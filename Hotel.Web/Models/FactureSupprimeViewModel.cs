using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Hotel.Models
{
    public class FactureSupprimeViewModel
    {
        public HistoriqueReglement HistoriqueReglement { get; set; }

        public IEnumerable<HistoriqueReglement> HistoriqueReglements { get; set; }
        public string? SearchAgent { get; set; }
        public string? SearchClient { get; set; }
        public string? SearchDevise { get; set; }
        public string? SearchDateRegl { get; set; }
        public string? SearchMontant { get; set; }
        public string? SearchNumfact { get; set; }
        public IEnumerable<SelectListItem> AgentList { get; set; }
        public IEnumerable<SelectListItem> ModeReglementList { get; set; }
        public IEnumerable<SelectListItem> DeviseList { get; set; }
        public IEnumerable<SelectListItem> TypeReglementList { get; set; }
        public IEnumerable<SelectListItem> CaisseList { get; set; }
        public IEnumerable<SelectListItem> ReservationList { get; set; }
    }
}
