using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Hotel.Models
{
    public class Fournir
    {
        [ForeignKey("FournisseurId")]
        public int FournisseurId { get; set; }
        public Fournisseur Fournisseur { get; set; }
        [ForeignKey("AlimentId")]
        public int AlimentId { get; set; }
        public Aliment Aliment { get; set; }

    }
}
