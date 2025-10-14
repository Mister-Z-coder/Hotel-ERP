using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Hotel.Models
{
    public class Procurer
    {
        [ForeignKey("FournisseurId")]
        public int FournisseurId { get; set; }
        public Fournisseur Fournisseur { get; set; }
        [ForeignKey("BoissonId")]
        public int BoissonId { get; set; }
        public Boisson Boisson { get; set; }

    }
}
