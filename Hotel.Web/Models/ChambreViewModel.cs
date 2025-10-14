using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Hotel.Models
{
    public class ChambreViewModel
    {
        public IEnumerable<Chambre> Chambre { get; set; }
        public string? SearchString { get; set; }
    }
}
