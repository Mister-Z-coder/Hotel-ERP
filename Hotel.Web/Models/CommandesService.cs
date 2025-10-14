using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Hotel.Models
{
    public class CommandesService:Commande
    {
        [ForeignKey("ServiceId")]
        public int ServiceId { get; set; }

    }
}
