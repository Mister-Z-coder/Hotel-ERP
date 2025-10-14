using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Hotel.Models
{
    public class Inclure
    {
        [ForeignKey("ReservationId")]
        public int ReservationId { get; set; }
        public Reservation Reservation { get; set; }
        [ForeignKey("ServiceId")]
        public int ServiceId { get; set; }
        public Service Service { get; set; }

    }
}
