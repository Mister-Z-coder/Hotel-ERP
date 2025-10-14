using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Hotel.Models
{
    public class FournirViewModel
    {
        public IEnumerable<SelectListItem> FournirList { get; set; }
        public IEnumerable<SelectListItem> ProcurerList { get; set; }
        public IEnumerable<Procurer> Procurer { get; set; }
        public IEnumerable<Fournir> Fournir { get; set; }
        public Fournisseur Fournisseur { get; set; }
        public IEnumerable<Boisson> Boisson { get; set; }
        public IEnumerable<Aliment> Aliment { get; set; }


    }
}
