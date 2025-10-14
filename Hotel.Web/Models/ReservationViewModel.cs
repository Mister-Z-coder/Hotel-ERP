using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Hotel.Models
{
    public class ReservationViewModel
    {
        public Reservation Reservations { get; set; }
        public IEnumerable<Reservation> Reservation { get; set; }
        public string? SearchAgent { get; set; }
        public string? SearchChambre { get; set; }
        public string? SearchResident { get; set; }
        public string? SearchDate { get; set; }
        public bool IsDirect { get; set; }


        [Required(ErrorMessage = "Devise requis")]
        [Display(Name = "Devise")]
        public int DeviseId { get; set; }

        [Required(ErrorMessage = "Mode reglement requis")]
        [Display(Name = "Mode reglement")]
        public int ModeReglementId { get; set; }

        [Required(ErrorMessage = "Type reglement requis")]
        [Display(Name = "Type reglement")]
        public int TypeReglementId { get; set; }
        public IEnumerable<SelectListItem> AgentList { get; set; }
        public IEnumerable<SelectListItem> ChambreList { get; set; }
        public IEnumerable<SelectListItem> ResidentList { get; set; }
        public IEnumerable<SelectListItem> ServicesList { get; set; }
        public IEnumerable<SelectListItem> ModeReglementList { get; set; }
        public IEnumerable<SelectListItem> TypeReglementList { get; set; }
        public IEnumerable<SelectListItem> DeviseList { get; set; }
        public IEnumerable<SelectListItem> InclureList { get; set; }
        public IEnumerable<Inclure> Inclures { get; set; }
        public IEnumerable<Service> Services { get; set; }
        public bool? reservationpaye { get; set; }


    }
}
