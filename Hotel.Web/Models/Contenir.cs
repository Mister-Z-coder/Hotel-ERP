using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Hotel.Models
{
    public class Contenir
    {
        [ForeignKey("BoissonId")]
        public int BoissonId { get; set; }
        public Boisson Boisson { get; set; }

        [ForeignKey("CommandeId")]
        public int CommandeId { get; set; }
        public Commande Commande { get; set; }

        public int QuantiteCom { get; set; }


    }
}
