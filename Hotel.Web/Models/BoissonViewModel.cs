using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Hotel.Models
{
    public class BoissonViewModel
    {
        public Boisson Boisson { get; set; }
        public IEnumerable<Boisson> Boissons { get; set; }

        public IEnumerable<SelectListItem> BrasserieList { get; set; }
        public IEnumerable<SelectListItem> CategorieBoissonList { get; set; }
        public ICollection<HPrixUnitLBBoisson> HPrixUnitLBBoissonList { get; set; }
        public ICollection<HPrixUnitSBBoisson> HPrixUnitSBBoissonList { get; set; }
        public string? SearchString { get; set; }

    }
}
