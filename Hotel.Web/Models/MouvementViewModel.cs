using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Hotel.Models
{
    public class MouvementViewModel
    {
        public Mouvement Mouvements { get; set; }
        public IEnumerable<Mouvement> Mouvement { get; set; }
        public string? SearchType { get; set; }
        public string? SearchDate { get; set; }
        public string? SearchNature { get; set; }
        public string? SearchAgent { get; set; }
        public IEnumerable<SelectListItem> AgentList { get; set; }

        public IEnumerable<SelectListItem> TypeMouvementList { get; set; }

        public IEnumerable<SelectListItem> BougerList { get; set; }
        public IEnumerable<SelectListItem> MouvoirList { get; set; }
        public IEnumerable<Bouger> Bouger { get; set; }
        public IEnumerable<Mouvoir> Mouvoir { get; set; }
        public IEnumerable<Boisson> Boisson { get; set; }
        public IEnumerable<Aliment> Aliment { get; set; }
    }
}
