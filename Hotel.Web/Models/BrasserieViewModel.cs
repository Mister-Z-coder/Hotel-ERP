using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Hotel.Models
{
    public class BrasserieViewModel
    {
        public IEnumerable<Brasserie> Brasserie { get; set; }
        public string? SearchString { get; set; }
    }
}
