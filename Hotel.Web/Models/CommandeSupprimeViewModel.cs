using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Hotel.Models
{
    public class CommandeSupprimeViewModel
    {
        public HistoriqueReglement HistoriqueReglement { get; set; }

        public IEnumerable<HistoriqueReglement> HistoriqueReglements { get; set; }
        public string? SearchAgent { get; set; }
        public string? SearchClient { get; set; }
        public string? SearchTable { get; set; }
        public string? SearchType { get; set; }
        public string? SearchNumCmd { get; set; }
        public List<CommandeJour> CommandeJours { get; set; }

    }
}
