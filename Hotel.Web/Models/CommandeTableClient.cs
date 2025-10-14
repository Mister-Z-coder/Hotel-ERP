using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Hotel.Models
{
    public class CommandeTableClient:Commande
    {
        [ForeignKey("TableClientId")]
        public int TableClientId { get; set; }

        public DateTime DateOccupation { get; set; }
        public DateTime HeureOccupation { get; set; }

    }
}
