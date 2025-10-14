using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Hotel.Models
{
    public class Concerner
    {
        [ForeignKey("AlimentId")]
        public int AlimentId { get; set; }
        public Aliment Aliment { get; set; }

        [ForeignKey("CommandeId")]
        public int CommandeId { get; set; }
        public Commande Commande { get; set; }

        public int QuantiteCom { get; set; }


    }
}
