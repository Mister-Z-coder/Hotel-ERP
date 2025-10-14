using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Hotel.Models
{
    public class Reservation
    {

        public int ReservationId { get; set; }

        [Required(ErrorMessage = "Agent requis")]
        [Display(Name = "Agent")]
        [ForeignKey("AgentId")]
        public int AgentId { get; set; }
        public virtual Agent Agent { get; set; }

        [Required(ErrorMessage = "Chambre requis")]
        [Display(Name = "Chambre")]
        [ForeignKey("ChambreId")]
        public int ChambreId { get; set; }
        public virtual Chambre Chambre { get; set; }

        [Required(ErrorMessage = "Resident requis")]
        [Display(Name = "Resident")]
        [ForeignKey("ResidentId")]
        public int ResidentId { get; set; }
        public virtual Resident Resident { get; set; }


        [Required(ErrorMessage = "Date de reservation requis")]
        [Display(Name = "Date de reservation")]
        public DateTime DateReservation { get; set; }

        [Required(ErrorMessage = "Heure de reservation requis")]
        [Display(Name = "Heure de reservation")]
        public DateTime HeureReservation { get; set; }

        [Required(ErrorMessage = "Date de début occupation requis")]
        [Display(Name = "Date de début occupation")]
        public DateTime DateDebutOccupation { get; set; }

        [Required(ErrorMessage = "Heure de début occupation requis")]
        [Display(Name = "Heure de début occupation")]
        public DateTime HeureDebutOccupation { get; set; }

        [Required(ErrorMessage = "Date de fin occupation requis")]
        [Display(Name = "Date de fin occupation")]
        public DateTime DateFinOccupation { get; set; }

        [Required(ErrorMessage = "Heure de fin occupation requis")]
        [Display(Name = "Heure de fin occupation")]
        public DateTime HeureFinOccupation { get; set; }

        [Required(ErrorMessage = "Statut de reservation requis")]
        [Display(Name = "Statut de reservation")]
        public string StatutReservation { get; set; }

        [Required(ErrorMessage = "Caution de reservation requis")]
        [Display(Name = "Caution de reservation $")]
        public double CautionReservation { get; set; }
        public ICollection<Inclure> Inclures { get; set; }

    }
}
