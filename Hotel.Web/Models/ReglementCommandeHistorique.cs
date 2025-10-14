using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Hotel.Models
{
    public class ReglementCommandeHistorique
    {
        public int CommandeId { get; set; }

        public int ReglementId { get; set; }
        public decimal MontantAffecte { get; set; }


    }
}
