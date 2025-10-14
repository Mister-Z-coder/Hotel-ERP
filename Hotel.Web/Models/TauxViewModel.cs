using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Hotel.Models
{
    public class TauxViewModel
    {
        public Taux Taux { get; set; }
        public IEnumerable<SelectListItem> DeviseList { get; set; }
    }
}
