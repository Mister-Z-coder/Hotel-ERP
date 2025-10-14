using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Hotel.Models
{
    public class CategorieBoissonViewModel
    {
        public IEnumerable<CategorieBoisson> CategorieBoisson { get; set; }
        public string? SearchString { get; set; }
    }
}
