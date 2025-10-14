using Hotel.Data;
using Hotel.Models;
using Hotel.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Hotel.Controllers
{
    [Authorize]
    [Authorize(Roles = "ADMIN,SUPERVISEUR,RECEPTIONNISTE")]
    [CaisseRedirectFilter]
    public class PayementController : Controller
    {
        private readonly ApplicationDbContext _db;
        public PayementController(ApplicationDbContext db)
        {
            _db = db;
        }

        public IActionResult FactureCaution()
        {
            return View();
        }
        public IActionResult Index(int page = 1, int size = 20, string searchAgent = "", string searchChambre = "", string searchResident = "", string searchDate = "")
        {

            DateTime parsedDate;
            bool isDateValid = DateTime.TryParse(searchDate, out parsedDate);
            int skipValue = (page - 1) * size;
            var reservations = _db.Reservations
                                .Include(r => r.Agent)
                                .Include(r => r.Chambre)
                                .Include(r => r.Resident)
                                .Where(r => !_db.ReglementReservations.Any(rr => rr.ReservationId == r.ReservationId &&
                                                                                 rr.MotifReglement == "Reservation(CAISSE ACCEUIL)") && r.StatutReservation != "Annule" 
                                                                                 && (string.IsNullOrEmpty(searchAgent) || r.Agent.NomAgent.ToUpper().Contains(searchAgent.ToUpper())) &&
                  (string.IsNullOrEmpty(searchChambre) || r.Chambre.NumeroChambre.ToString().ToUpper().Contains(searchChambre.ToUpper())) &&
                  (string.IsNullOrEmpty(searchResident) || r.Resident.NomClient.ToUpper().Contains(searchResident.ToUpper())) &&
                  (string.IsNullOrEmpty(searchDate) || r.DateReservation == Convert.ToDateTime(parsedDate.Date)))
                                .OrderByDescending(r => r.ReservationId)
                                .Skip(skipValue)
                                .Take(size)
                                .ToList();
            int totalreservations = reservations.Count();
            int totalPages = (int)Math.Ceiling((double)totalreservations / size);
            var depart = (totalPages - 1) * skipValue + 1;
            ViewBag.NumAff = depart;
            ViewBag.CurrentPage = page;
            ViewBag.PageSize = size;
            ViewBag.TotalPage = totalPages;

           
            ReservationViewModel reservationViewModel = new()
            {
                Reservation = reservations
            };
            return View(reservationViewModel);
        }
        
        private void ValidationDate(DateTime date, string fieldName, string errorMessage)
        {
            if (date < DateTime.Today)
            {
                ModelState.AddModelError(fieldName, errorMessage);
            }
        }
        private decimal CalculerMontant(DateTime debut, DateTime fin, Chambre chambre, double monantservices)
        {
            string TypeReservation = "";
            //Verifier la durée
            TimeSpan duree = fin - debut;
            if (duree.TotalHours < 12 && debut.Date == fin.Date)
            {
                TypeReservation = "ALaHeure";
            }
            else
            {
                TypeReservation = "ALaNuit";
            }

            if (TypeReservation == "ALaNuit")
            {
                int nombreNuits = (int)Math.Round((fin - debut).TotalDays);
                return ((decimal)chambre.PrixNuit * nombreNuits) + ((decimal) monantservices);
            }
            else
            {
                int nombreHeures = (int)Math.Round((fin - debut).TotalHours);
                return ((decimal)chambre.PrixHeure * nombreHeures) + ((decimal)monantservices);
            }
        }
        public IActionResult Payer(int reservationId)
        {
            Reservation? reservation = _db.Reservations.FirstOrDefault(r => r.ReservationId == reservationId);
            ReservationViewModel reservationViewModel = new ReservationViewModel();
            

            reservationViewModel.AgentList = _db.Agents.ToList().Select(a => new SelectListItem
            {
                Text = $"{a.NomAgent} | Prénom : {a.PrenomAgent} | Sexe : {a.SexeAgent} | Nationalité : {a.Nationalite_Agent}",
                Value = a.AgentId.ToString()
            }).ToList();
            reservationViewModel.ChambreList = _db.Chambres.ToList().Select(c => new SelectListItem
            {
                Text = $"{c.NumeroChambre.ToString()} | Type : {c.TypeChambre} | Statut : {c.StatutChambre} | Prix-heure : {c.PrixHeure} | Prix-nuit : {c.PrixNuit} | Capacité : {c.CapaciteMaxChambre}",
                Value = c.ChambreId.ToString()
            }).ToList();
            reservationViewModel.ResidentList = _db.Residents.ToList().Select(r => new SelectListItem
            {
                Text = $"{r.NomClient} | Téléphone : {r.PhoneClient} | Sexe : {r.SexeClient}",
                Value = r.ClientId.ToString()
            }).ToList();

            reservationViewModel.Services = _db.Services
                .ToList()
                .Where(s => !_db.Inclures
                .Any(i => i.ServiceId == s.ServiceId && i.ReservationId == reservationId));

            reservationViewModel.Inclures = _db.Inclures
                .Include(s => s.Service)
                .Include(r => r.Reservation)
                .ToList()
                .Where(r => r.ReservationId == reservationId);

            reservationViewModel.ModeReglementList = _db.ModeReglements.ToList().Select(m => new SelectListItem
            {
                Text = $"{m.TitreModeReglement}",
                Value = m.ModeReglementId.ToString()
            }).ToList();

            reservationViewModel.DeviseList = (from devises in _db.Devises
                                               join taux in _db.Tauxes on devises.DeviseId equals taux.DeviseId
                                               orderby taux.TauxId descending
                                               group devises by new
                                               {
                                                   devises.DeviseId,
                                                   devises.SigleDevise,
                                                   devises.TitreDevise
                                               } into g
                                               select new Devise
                                               {
                                                   DeviseId = g.Key.DeviseId,
                                                   SigleDevise = g.Key.SigleDevise,
                                                   TitreDevise = g.Key.TitreDevise
                                               })
                                           .Select(d => new SelectListItem
                                           {
                                               Text = $"{d.SigleDevise} : {d.TitreDevise}",
                                               Value = d.DeviseId.ToString()
                                           }).ToList();


            reservationViewModel.TypeReglementList = _db.TypeReglements.ToList().Select(t => new SelectListItem
                {
                    Text = $"{t.TitreTypeReglement.ToString()}",
                    Value = t.TypeReglementId.ToString()
                }).ToList();
            
            int caissId = 0;
            var agentIdClaim = User.Claims.FirstOrDefault(c => c.Type == "AgentId");
            var agentId = Convert.ToInt32(agentIdClaim.Value);

            var caisseDetails = (
            from fst in _db.FinShiftTravails
            join a in _db.Agents on fst.AgentId equals a.AgentId
            join c in _db.Caisses on fst.PosteTravailId equals c.PosteTravailId
            where fst.AgentId == agentId && fst.DateFinShift == null
            orderby fst.DateDebutShift descending // Trier par date de début décroissante
            select new CaisseDetail // Utilisez la classe CaisseDetail
            {
                CaisseId = c.PosteTravailId, // Inclut l'id de la caisse
                TitreCaisse = c.TitreCaisse,  // Inclut le titre de la caisse
                BarCaisse = c.BarCaisse       // Inclut le bar de la caisse
            }
            ).ToList();
            if (caisseDetails is not null && caisseDetails.Count > 0)
            {
                foreach (var caisse in caisseDetails)
                {
                    caissId = caisse.CaisseId;
                }
            }
            else
            {
                TempData["error"] = "Poste de travail non reconnu.";
                ViewBag.TitreCaisse = "";
                ViewBag.BarCaisse = "";
                ModelState.AddModelError("Reservations.AgentId", "Cet agent n'est pas affecté au poste ce poste de travail requis pour cette opération.");
            }

            ViewData["ModeReglementList"] = _db.ModeReglements.ToList();
            ViewData["DeviseList"] = (from devises in _db.Devises
                                      join taux in _db.Tauxes on devises.DeviseId equals taux.DeviseId
                                      orderby taux.TauxId descending
                                      group devises by new
                                      {
                                          devises.DeviseId,
                                          devises.SigleDevise,
                                          devises.TitreDevise
                                      } into g
                                      select new Devise
                                      {
                                          DeviseId = g.Key.DeviseId,
                                          SigleDevise = g.Key.SigleDevise,
                                          TitreDevise = g.Key.TitreDevise
                                      }).ToList();
            ViewData["TypeReglementList"] = _db.TypeReglements.ToList();
            ViewData["Caisse"] = caisseDetails;
            if (reservation is not null)
            {
                reservationViewModel.Reservations = reservation;
                //if (reservation.StatutReservation == "Annule")
                //{
                //    return RedirectToAction("Details", "Reservation");
                //}
            }
            else
            {
                return RedirectToAction("Error", "Home");
            }

            DateTime debutComplet = reservationViewModel.Reservations.DateDebutOccupation.Date + reservationViewModel.Reservations.HeureDebutOccupation.TimeOfDay;
            DateTime finComplet = reservationViewModel.Reservations.DateFinOccupation.Date + reservationViewModel.Reservations.HeureFinOccupation.TimeOfDay;
            //Recupérer caution
            double caution = reservationViewModel.Reservations.CautionReservation;
            //Recupérer montant services inclus
            double monantservices = 0;
            foreach (var services in reservationViewModel.Inclures)
            {
                monantservices =+ services.Service.PrixService;
            }
            //Recuérer montant reservation
            decimal prixchambre = CalculerMontant(debutComplet, finComplet, reservationViewModel.Reservations.Chambre, monantservices);

            //Recupérer montant total caution + prix chambre - montant services
            //
            decimal montanttot = (decimal)prixchambre - (decimal)caution;

            if (montanttot < 0)
            {
                TempData["error"] = "La reservation a déjà été reglé.";

            }
            else
            {
                decimal montanttotal = Math.Abs((decimal)prixchambre - (decimal)caution);
                ViewBag.PrixTotal = montanttotal;
            }
            return View(reservationViewModel);
        }
        public class service
        {
            public int id { get; set; }
            public string nom { get; set; }
            public string prix { get; set; }
        }
        public class ReglementData
        {
            public string ReservationId { get; set; }      // Identifiant de la commande
            public string ModeReglementId { get; set; }      // Identifiant du mode de règlement
            public string DeviseId { get; set; }              // Identifiant de la devise
            public string TypeReglementId { get; set; }       // Identifiant du type de règlement
            public string PosteTravailId { get; set; }        // Identifiant du poste de travail
            public string MontantReglement { get; set; }   // Montant du règlement
            public string MotifReglement { get; set; }
            public List<service> ServicesArray { get; set; }
        }
        [HttpPost]
        [Route("ValiderReglementReservation")]
        public IActionResult ValiderReglementReservation([FromBody] ReglementData data)
        {
            try
            {
                // Valider le règlement ici (par exemple, enregistrer dans la base de données)
                // ...
                

                var reglementpayexist = _db.ReglementReservations.FirstOrDefault(rr => rr.ReservationId == int.Parse(data.ReservationId) && rr.MotifReglement == "Reservation(CAISSE ACCEUIL)");
                if (reglementpayexist is not null)
                {
                    return Json(new { success = false, message = "Cette commande a déjà été reglée." });
                }

                var agentIdClaim = User.Claims.FirstOrDefault(c => c.Type == "AgentId");
                if (agentIdClaim == null || !int.TryParse(agentIdClaim.Value, out int agentId))
                {
                    return Json(new { success = false, message = "Agent non identifié." });
                }
                
                
                int caissId = 0;

                var caisseDetails = (
                from fst in _db.FinShiftTravails
                join a in _db.Agents on fst.AgentId equals a.AgentId
                join c in _db.Caisses on fst.PosteTravailId equals c.PosteTravailId
                where fst.AgentId == agentId && fst.DateFinShift == null && c.TitreCaisse == "CAISSE ACCEUIL"
                orderby fst.DateDebutShift descending // Trier par date de début décroissante
                select new CaisseDetail // Utilisez la classe CaisseDetail
                {
                    CaisseId = c.PosteTravailId, // Inclut l'id de la caisse
                }
                ).ToList();

                if (caisseDetails is not null && caisseDetails.Count > 0)
                {
                    foreach (var caisse in caisseDetails)
                    {
                        caissId = caisse.CaisseId;
                    }
                }
                else
                {
                    TempData["error"] = "Poste de travail non reconnu.";
                    ModelState.AddModelError("Reservations.AgentId", "Cet agent n'est pas affecté au poste ce poste de travail requis pour cette opération.");
                }
                //var titrecaisse = _db.Caisses.FirstOrDefault(c => c.PosteTravailId == int.Parse(data.PosteTravailId));
                var titrecaisse = _db.Caisses.Where(c => c.PosteTravailId == int.Parse(data.PosteTravailId)).Select(c => c.TitreCaisse).FirstOrDefault();

                ReglementReservation reglementReservation = new()
                {
                    ReservationId = int.Parse(data.ReservationId),
                    ModeReglementId = int.Parse(data.ModeReglementId),
                    DeviseId = int.Parse(data.DeviseId),
                    TypeReglementId = int.Parse(data.TypeReglementId),
                    AgentId = agentId,
                    CaisseId = int.Parse(data.PosteTravailId),
                    DateReglement = DateTime.Today,
                    HeureReglement = DateTime.Now,
                    MontantReglement = decimal.Parse(data.MontantReglement),
                    MotifReglement = $"Reservation({titrecaisse})"
                };
                _db.Reglements.Add(reglementReservation);
                _db.SaveChanges();

                var detailsReservation = _db.Reservations.Where(r => r.ReservationId == reglementReservation.ReservationId).FirstOrDefault();
                

                //var detailsFactures = _db.DetailsFactures.Where(d => d.Numcmd == reglementcommande.CommandeId).ToList();
                if (detailsReservation is not null)
                {
                    DateTime debut = detailsReservation.DateDebutOccupation.Date + detailsReservation.HeureDebutOccupation.TimeOfDay;
                    DateTime fin = detailsReservation.DateFinOccupation.Date + detailsReservation.HeureFinOccupation.TimeOfDay;
                    TimeSpan duree = fin - debut;
                    if (data.ServicesArray.Count > 0)
                    {
                        foreach (var services in data.ServicesArray)
                        {
                            DetailsFactureReservation detailsFactureReservation = new()
                            {
                                Numres = detailsReservation.ReservationId,
                                Numfact = reglementReservation.ReglementId,
                                Datefact = DateTime.Today,
                                Heurefact = DateTime.Now,
                                Debut = detailsReservation.DateDebutOccupation.Date + detailsReservation.HeureDebutOccupation.TimeOfDay,
                                Fin = detailsReservation.DateFinOccupation.Date + detailsReservation.HeureFinOccupation.TimeOfDay,
                                Caissier = detailsReservation.AgentId,
                                Caisse = reglementReservation.CaisseId,
                                Chambre = _db.Chambres.Where(c => c.ChambreId == detailsReservation.ChambreId).Select(c => c.NumeroChambre).FirstOrDefault().ToString(),
                                Duree = Convert.ToInt32(duree.TotalHours),
                                Service = services.nom,
                                Montant = (decimal)reglementReservation.MontantReglement,
                                ModeReglement = _db.ModeReglements.Where(m => m.ModeReglementId == reglementReservation.ModeReglementId).Select(m => m.TitreModeReglement).FirstOrDefault().ToString(),
                                TypeReglement = _db.TypeReglements.Where(t => t.TypeReglementId == reglementReservation.TypeReglementId).Select(t => t.TitreTypeReglement).FirstOrDefault().ToString(),
                                Devise = _db.Devises.Where(d => d.DeviseId == reglementReservation.DeviseId).Select(d => d.SigleDevise).FirstOrDefault().ToString()
                            };
                            _db.DetailsFactureReservations.Add(detailsFactureReservation);
                            _db.SaveChanges();
                        }
                    }
                    else
                    {
                        DetailsFactureReservation detailsFactureReservation = new()
                        {
                            Numres = detailsReservation.ReservationId,
                            Numfact = reglementReservation.ReglementId,
                            Datefact = DateTime.Today,
                            Heurefact = DateTime.Now,
                            Debut = detailsReservation.DateDebutOccupation.Date + detailsReservation.HeureDebutOccupation.TimeOfDay,
                            Fin = detailsReservation.DateFinOccupation.Date + detailsReservation.HeureFinOccupation.TimeOfDay,
                            Caissier = detailsReservation.AgentId,
                            Caisse = reglementReservation.CaisseId,
                            Chambre = _db.Chambres.Where(c => c.ChambreId == detailsReservation.ChambreId).Select(c => c.NumeroChambre).FirstOrDefault().ToString(),
                            Duree = Convert.ToInt32(duree.TotalHours),
                            Montant = (decimal)reglementReservation.MontantReglement,
                            ModeReglement = _db.ModeReglements.Where(m => m.ModeReglementId == reglementReservation.ModeReglementId).Select(m => m.TitreModeReglement).FirstOrDefault().ToString(),
                            TypeReglement = _db.TypeReglements.Where(t => t.TypeReglementId == reglementReservation.TypeReglementId).Select(t => t.TitreTypeReglement).FirstOrDefault().ToString(),
                            Devise = _db.Devises.Where(d => d.DeviseId == reglementReservation.DeviseId).Select(d => d.SigleDevise).FirstOrDefault().ToString()
                        };
                        _db.DetailsFactureReservations.Add(detailsFactureReservation);
                        _db.SaveChanges();
                    }
                    

                }
                else
                {
                    Console.WriteLine($"Erreur lors de la validation du reglement");
                    TempData["error"] = "Impossible de valider ce reglement.";
                    return RedirectToAction("Index");
                }
                //await insererDetailFactureAsync(reglementcommande.CommandeId);
                TempData["success"] = "Reglement validé avec succès.";
                return Json(new { success = true, id = reglementReservation.ReglementId });
                return RedirectToAction("Index");

            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

    }
}
