using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Hotel.Models
{
    public class ReglementViewModel
    {
        public Reglement Reglement { get; set; }

        public IEnumerable<Reglement> Reglements { get; set; }
        public string? SearchAgent { get; set; }
        public string? SearchCaisse { get; set; }
        public string? SearchMotif { get; set; }
        public string? SearchDatedebut { get; set; }
        public string? SearchDatefin { get; set; }
        public IEnumerable<SelectListItem> AgentList { get; set; }
        public IEnumerable<SelectListItem> ModeReglementList { get; set; }
        public IEnumerable<SelectListItem> DeviseList { get; set; }
        public IEnumerable<SelectListItem> TypeReglementList { get; set; }
        public IEnumerable<SelectListItem> CaisseList { get; set; }
        public IEnumerable<SelectListItem> ReservationList { get; set; }
    }
}
