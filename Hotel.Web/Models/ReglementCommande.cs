using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Hotel.Models
{
    public class ReglementCommande
    {
        [ForeignKey("CommandeId")]
        public int CommandeId { get; set; }

        [ForeignKey("ReglementId")]
        public int ReglementId { get; set; }
        public decimal MontantAffecte { get; set; }

        public Reglement Reglement { get; set; }
        public Commande Commande { get; set; }

    }
}
