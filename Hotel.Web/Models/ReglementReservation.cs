using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Hotel.Models
{
    public class ReglementReservation : Reglement
    {
        [ForeignKey("ReservationId")]
        public int ReservationId { get; set; }

    }
}
