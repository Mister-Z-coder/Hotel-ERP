using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Hotel.Models
{
    public class FournisseurViewModel
    {
        public IEnumerable<Fournisseur> Fournisseur { get; set; }
        public string? SearchString { get; set; }
    }
}
