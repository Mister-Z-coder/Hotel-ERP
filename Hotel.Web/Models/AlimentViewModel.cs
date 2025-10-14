using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Hotel.Models
{
    public class AlimentViewModel
    {
        public Aliment Aliment { get; set; }
        public IEnumerable<Aliment> Aliments { get; set; }
        public ICollection<HPrixUnitAliment> HPrixUnitAlimentList { get; set; }
        public string? SearchString { get; set; }

    }
}
