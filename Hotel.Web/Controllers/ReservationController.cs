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
    public class ReservationController : Controller
    {
        private readonly ApplicationDbContext _db;
        public ReservationController(ApplicationDbContext db)
        {
            _db = db;
        }

        public IActionResult FactureCaution()
        {
            return View();
        }
        public IActionResult FactureReservation()
        {
            return View();
        }
        public IActionResult Index(int page = 1, int size = 20, string searchAgent="", string searchChambre = "", string searchResident = "", string searchDate="")
        {
            DateTime parsedDate;
            bool isDateValid = DateTime.TryParse(searchDate, out parsedDate);
            int totalreservations = _db.Reservations.Count();
            int skipValue = (page - 1) * size;
            int totalPages = (int)Math.Ceiling((double)totalreservations / size);
            var reservations = _db.Reservations
              .Include(r => r.Agent)
              .Include(r => r.Chambre)
              .Include(r => r.Resident)
              .Where(r =>
                  (string.IsNullOrEmpty(searchAgent) || r.Agent.NomAgent.ToUpper().Contains(searchAgent.ToUpper())) &&
                  (string.IsNullOrEmpty(searchChambre) || r.Chambre.NumeroChambre.ToString().ToUpper().Contains(searchChambre.ToUpper())) &&
                  (string.IsNullOrEmpty(searchResident) || r.Resident.NomClient.ToUpper().Contains(searchResident.ToUpper())) &&
                  (string.IsNullOrEmpty(searchDate) || r.DateReservation == Convert.ToDateTime(parsedDate.Date))
              )
              .OrderByDescending(r => r.ReservationId)               // Ensuite, trier par date de réservation
              .ThenBy(r => r.StatutReservation == "Confirme" ? 0 : 1) // Les "Confirmé" en premier
              .Skip(skipValue)
              .Take(size)
              .ToList();
            var depart = (totalPages - 1) * skipValue + 1;
            ViewBag.NumAff = depart;
            ViewBag.CurrentPage = page;
            ViewBag.PageSize = size;
            ViewBag.TotalPage = totalPages;

            ViewBag.SearchAgent = searchAgent;
            ViewBag.SearchChambre = searchChambre;
            ViewBag.SearchResident = searchResident;
            
            ReservationViewModel reservationViewModel = new()
            {
                Reservation = reservations
            };
            return View(reservationViewModel);
        }
        public IActionResult Create()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            int userId = userIdClaim != null ? int.Parse(userIdClaim.Value) : 0;
            Agent agent = _db.Agents.FirstOrDefault(a => a.AgentId == userId);


            ReservationViewModel ReservationVM = new()
            {
                AgentList = _db.Agents.ToList().Where(a => a.AgentId == userId).Select(a => new SelectListItem
                {
                    Text = $"{a.NomAgent} | Prénom : {a.PrenomAgent} | Sexe : {a.SexeAgent} | Nationalité : {a.Nationalite_Agent}",
                    Value = a.AgentId.ToString()
                }),

                ChambreList = _db.Chambres.Where(c => c.StatutChambre == "Libre|Propre").ToList().Select(c => new SelectListItem
                {
                    Text = $"{c.NumeroChambre.ToString()} | Type : {c.TypeChambre} | Statut : {c.StatutChambre} | Prix-heure : {c.PrixHeure} | Prix-nuit : {c.PrixNuit} | Capacité : {c.CapaciteMaxChambre}",
                    Value = c.ChambreId.ToString()
                }).ToList(),

            ResidentList = _db.Residents.ToList().Select(r => new SelectListItem
                {
                    Text = $"{r.NomClient} | Téléphone : {r.PhoneClient} | Sexe : {r.SexeClient}",
                    Value = r.ClientId.ToString()
                }).ToList(),

                //ServicesList = _db.Services.ToList().Select(s => new SelectListItem
                //{
                //    Text = $"{s.NomService} | Prix : {s.PrixService}",
                //    Value = s.ServiceId.ToString()
                //}).ToList(),
                ModeReglementList = _db.ModeReglements.ToList().Select(m => new SelectListItem
                {
                    Text = $"{m.TitreModeReglement}",
                    Value = m.ModeReglementId.ToString()
                }).ToList(),

                //DeviseList = (from devises in _db.Devises
                //               join taux in _db.Tauxes on devises.DeviseId equals taux.DeviseId
                //               select new Devise
                //               {
                //                   DeviseId = devises.DeviseId,
                //                   SigleDevise = devises.SigleDevise,
                //                   TitreDevise = devises.TitreDevise
                //               })
                //               .Select(d => new SelectListItem
                //                {
                //                    Text = $"{d.SigleDevise} : {d.TitreDevise}",
                //                    Value = d.DeviseId.ToString()
                //                }).ToList(),
                DeviseList = (from devises in _db.Devises
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
                                           }).ToList(),

                TypeReglementList = _db.TypeReglements.ToList().Select(t => new SelectListItem
                {
                    Text = $"{t.TitreTypeReglement.ToString()}",
                    Value = t.TypeReglementId.ToString()
                }).ToList(),

                Reservations = new Reservation{
                    AgentId = userId
                }
            };

            return View(ReservationVM);
        }
        private void ValidationDate(DateTime date, string fieldName, string errorMessage)
        {
            if (date < DateTime.Today)
            {
                ModelState.AddModelError(fieldName, errorMessage);
            }
        }

        public IActionResult Details(int reservationId)
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
            //Recupérer reservation non payé
            var reservationpaye = _db.Reservations
                                .Include(r => r.Agent)
                                .Include(r => r.Chambre)
                                .Include(r => r.Resident)
                                .Where(r => !_db.ReglementReservations.Any(rr => rr.ReservationId == r.ReservationId &&
                                                                                 rr.MotifReglement == "Reservation") && r.StatutReservation != "Annule")
                                .OrderBy(r => r.DateReservation)
                                .FirstOrDefault(r=>r.ReservationId == reservation.ReservationId);
            if (reservationpaye is not null)
            {
                reservationViewModel.reservationpaye = false;
            }
            else
            {
                reservationViewModel.reservationpaye = true;

            }

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
            
            return View(reservationViewModel);
        }

        

        [HttpPost]
        public IActionResult Create(ReservationViewModel reservationViewModel)
        {
            ModelState.Remove("Reservations.DateReservation");
            ModelState.Remove("Reservations.HeureReservation");
            ModelState.Remove("Reservations.StatutReservation");
            int caissId = 0;
            var agentIdClaim = User.Claims.FirstOrDefault(c => c.Type == "AgentId");
            var agentId = Convert.ToInt32(agentIdClaim.Value);

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
                TempData["error"] = "Agent non affecté à ce poste de travail.";
                ModelState.AddModelError("Reservations.AgentId", "Cet agent n'est pas affecté au poste ce poste de travail requis pour cette opération.");
            }
            //DateTime dtreservation = reservationViewModel.Reservation.DateReservation;
            reservationViewModel.Reservations.DateReservation = DateTime.Today;
            DateTime dtdebutoccupation = reservationViewModel.Reservations.DateDebutOccupation;
            DateTime dtfinoccupation = reservationViewModel.Reservations.DateFinOccupation;
            double caution = reservationViewModel.Reservations.CautionReservation;
            //DateTime hrreservation = reservationViewModel.Reservation.HeureReservation;
            reservationViewModel.Reservations.HeureReservation = DateTime.Now;
            DateTime hrdebutoccupation = reservationViewModel.Reservations.HeureDebutOccupation;
            DateTime hrfinoccupation = reservationViewModel.Reservations.HeureFinOccupation;
            int duree = 0;
            DateTime debutComplet = dtdebutoccupation.Date + hrdebutoccupation.TimeOfDay;
            DateTime finComplet = dtfinoccupation.Date + hrfinoccupation.TimeOfDay;
            string typereservation="";
            reservationViewModel.Reservations.StatutReservation = "Confirme";
            DateTime datheureres = reservationViewModel.Reservations.DateReservation.Date + reservationViewModel.Reservations.HeureReservation.TimeOfDay;
            ///bool IsDirect = reservationViewModel.IsDirect;

            duree = Convert.ToInt32((finComplet - debutComplet).TotalHours);
            if (ModelState.IsValid)
            {

                //Verification à la date du jour
                ValidationDate(dtdebutoccupation, "Reservations.DateDebutOccupation",
                             "La date de début d'occupation ne peut pas être antérieure à la date du jour.");
                ValidationDate(dtfinoccupation, "Reservations.DateFinOccupation",
                             "La date de fin d'occupation ne peut pas être antérieure à la date du jour.");



                //Verification de la caution
                if (caution <= 0)
                {
                    ModelState.AddModelError("Reservations.CautionReservation", "La caution est requis.");
                }

                if (hrdebutoccupation.Hour == 0)
                {
                    ModelState.AddModelError("Reservations.HeureDebutOccupation", "Les heures de debut d\'occupation doivent être comprisent entre 1h et 23h.");
                }
                if (hrfinoccupation.Hour == 0)
                {
                    ModelState.AddModelError("Reservations.HeureFinOccupation", "Les heures de fin d\'occupation doivent être comprisent entre 1h et 23h.");
                }

                //Verification date de debut doit être <= à la date de fin occupation
                if (dtdebutoccupation > dtfinoccupation)
                {
                    ModelState.AddModelError("Reservations.DateDebutOccupation", "La date de début d'occupation doit être inférieure ou égale à la date de fin d'occupation.");
                }

                //Verification minimum 1 heure de reservation
                if (dtdebutoccupation == dtfinoccupation)
                {
                    //Verification heure debut doit être < à heure de fin si la date de debut et de fin occupation sont égaux

                    if (hrdebutoccupation > hrfinoccupation)
                    {
                        ModelState.AddModelError("Reservations.HeureDebutOccupation", "L\' heure de debut d\'occupation ne peut pas être supérieure à l\' heure de fin d\'occupation.");
                    }

                    //Associer les dates et heures pour avoir un resultat sûr
                    debutComplet = dtdebutoccupation.Date + hrdebutoccupation.TimeOfDay;
                    finComplet = dtfinoccupation.Date + hrfinoccupation.TimeOfDay;


                    //Verifier la durée
                    TimeSpan dureeH = finComplet - debutComplet;
                    duree = Convert.ToInt32(dureeH.TotalHours);
                    if (dureeH.TotalHours < 1)
                    {
                        ModelState.AddModelError("Reservations.HeureFinOccupation", "Une reservation doit être au minimum d'une heure.");

                    }

                }
                if(datheureres > debutComplet && datheureres > finComplet)
                {
                    ModelState.AddModelError("Reservations.DateDebutOccupation", "Les différentes dates et heures ne doivent pas être inférieures à la date du jour.");
                }

                if (ModelState.IsValid)
                {
                    Chambre chambre = _db.Chambres.SingleOrDefault(c => c.ChambreId == reservationViewModel.Reservations.ChambreId);
                    if (chambre is not null)
                    {
                        
                        _db.Reservations.Add(reservationViewModel.Reservations);
                        _db.SaveChanges();

                        if (chambre.StatutChambre.Contains("Nettoyage"))
                        {
                            chambre.StatutChambre = "Occupe|Nettoyage";

                        }
                        else if (chambre.StatutChambre.Contains("Propre"))
                        {
                            chambre.StatutChambre = "Occupe|Propre";
                        }
                        else if (chambre.StatutChambre.Contains("Sale"))
                        {
                            chambre.StatutChambre = "Occupe|Sale";
                        }
                        //nouvellechambre.StatutChambre = "Occupe";
                        //chambre.StatutChambre = "Occupe";
                        _db.Chambres.Update(chambre);
                        _db.SaveChanges();
                        var titrecaisse = _db.Caisses.Where(c => c.PosteTravailId == caissId).Select(c=> c.TitreCaisse).FirstOrDefault();

                        ReglementReservation reglementReservation = new()
                        {

                            ReservationId = reservationViewModel.Reservations.ReservationId,
                            ModeReglementId = reservationViewModel.ModeReglementId,
                            DeviseId = reservationViewModel.DeviseId,
                            TypeReglementId = reservationViewModel.TypeReglementId,
                            AgentId = reservationViewModel.Reservations.AgentId,
                            CaisseId = caissId,
                            DateReglement = reservationViewModel.Reservations.DateReservation,
                            HeureReglement = reservationViewModel.Reservations.HeureReservation,
                            MontantReglement = Convert.ToDecimal(reservationViewModel.Reservations.CautionReservation),
                            MotifReglement = $"Reservation-caution({titrecaisse})",
                        };

                        //if(IsDirect is true)
                        //{
                        //    reglementReservation.MotifReglement = $"Reservation({titrecaisse})";
                        //}
                        _db.Reglements.Add(reglementReservation);
                        _db.SaveChanges();
                        //Envoyer Print
                        //EnvoyerPrint(reglementReservation.ReglementId);
                        TempData["success"] = "Reservation effectuée.";
                        // 4. Stockage de l'ID pour impression après redirection
                        TempData["LastReglementId"] = reglementReservation.ReglementId;
                        TempData["success"] = "Réservation effectuée avec succès";
                        DetailsFactureCaution detailsFactureCaution = new()
                        {
                            Numres = reservationViewModel.Reservations.ReservationId,
                            Numfact = reglementReservation.ReglementId,
                            Datefact = DateTime.Today,
                            Heurefact = DateTime.Now,
                            Debut = reservationViewModel.Reservations.DateDebutOccupation.Date + reservationViewModel.Reservations.HeureDebutOccupation.TimeOfDay,
                            Fin = reservationViewModel.Reservations.DateFinOccupation.Date + reservationViewModel.Reservations.HeureFinOccupation.TimeOfDay,
                            Caissier = reservationViewModel.Reservations.AgentId,
                            Caisse = caissId,
                            Chambre = reservationViewModel.Reservations.Chambre.NumeroChambre.ToString(),
                            Duree = duree,
                            Montant = (decimal)reservationViewModel.Reservations.CautionReservation,
                            ModeReglement = _db.ModeReglements.Where(m => m.ModeReglementId == reglementReservation.ModeReglementId).Select(m => m.TitreModeReglement).FirstOrDefault().ToString(),
                            TypeReglement = _db.TypeReglements.Where(t => t.TypeReglementId == reglementReservation.TypeReglementId).Select(t => t.TitreTypeReglement).FirstOrDefault().ToString(),
                            Devise = _db.Devises.Where(d => d.DeviseId == reglementReservation.DeviseId).Select(d => d.SigleDevise).FirstOrDefault().ToString()
                        };
                        _db.DetailsFactureCautions.Add(detailsFactureCaution);
                        _db.SaveChanges();
                        //if(IsDirect is true)
                        //{
                        //    return RedirectToAction("FactureReservation");
                        //}
                        return RedirectToAction("FactureCaution");
                    }

                }

            }
            reservationViewModel.AgentList = _db.Agents.ToList().Select(a => new SelectListItem
            {
                Text = $"{a.NomAgent} | Prénom : {a.PrenomAgent} | Sexe : {a.SexeAgent} | Nationalité : {a.Nationalite_Agent}",
                Value = a.AgentId.ToString()
            }).ToList();
            reservationViewModel.ChambreList = _db.Chambres.Where(c => c.StatutChambre == "Libre|Propre").ToList().Select(c => new SelectListItem
            {
                Text = $"{c.NumeroChambre.ToString()} | Type : {c.TypeChambre} | Statut : {c.StatutChambre} | Prix-heure : {c.PrixHeure} | Prix-nuit : {c.PrixNuit} | Capacité : {c.CapaciteMaxChambre}",
                Value = c.ChambreId.ToString()
            }).ToList();
            reservationViewModel.ResidentList = _db.Residents.ToList().Select(r => new SelectListItem
            {
                Text = $"{r.NomClient} | Téléphone : {r.PhoneClient} | Sexe : {r.SexeClient}",
                Value = r.ClientId.ToString()
            }).ToList();
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
            return View(reservationViewModel);
        }

        private IActionResult EnvoyerPrint(int reglementId)
        {
            try
            {
                // Retourner un script JavaScript dans la réponse
                var script = $@"
            <script type='text/javascript'>
                console.log('Impression lancée pour le règlement ID: {reglementId}');
                window.open(`/Report/PrintPayementReservation?id=${reglementId}`, '_blank'); // Lancer l'impression
            </script>";

                return Content(script, "text/html");
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    success = false,
                    reglementId,
                    error = ex.Message
                });
            }
        }

        public IActionResult Update(int reservationId)
        {
            Reservation? reservation = _db.Reservations.FirstOrDefault(r => r.ReservationId == reservationId);
            ReservationViewModel reservationViewModel = new ReservationViewModel();
            //Reservation? reservation = _db.Reservations
            //.Select(r => new Reservation {
            //    ReservationId = r.ReservationId,
            //    DateReservation = r.DateReservation,
            //    HeureReservation = r.HeureReservation.Date + r.HeureReservation.TimeOfDay,
            //    DateDebutOccupation = r.DateDebutOccupation,
            //    HeureDebutOccupation = r.HeureDebutOccupation,
            //    DateFinOccupation = r.DateFinOccupation,
            //    HeureFinOccupation = r.HeureFinOccupation,
            //    StatutReservation = r.StatutReservation,
            //    CautionReservation = r.CautionReservation,
            //    AgentId = r.AgentId,
            //    ChambreId = r.ChambreId,
            //    ResidentId = r.ResidentId
            //}).FirstOrDefault(r => r.ReservationId == reservationId);

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
            return View(reservationViewModel);
        }
        
        [HttpPost]
        public IActionResult Update(ReservationViewModel reservationViewModel)
        {
            Reservation? reservation = _db.Reservations.FirstOrDefault(a => a.ReservationId == reservationViewModel.Reservations.ReservationId);
            //Chambre chambreoccupe = _db.Chambres.FirstOrDefault(c => c.ChambreId == reservationViewModel.Reservations.ChambreId);
            ModelState.Remove("Reservations.DateReservation");
            ModelState.Remove("Reservations.HeureReservation");
            ModelState.Remove("Reservations.StatutReservation");
            int caissId = 0;
            var agentIdClaim = User.Claims.FirstOrDefault(c => c.Type == "AgentId");
            var agentId = Convert.ToInt32(agentIdClaim.Value);

            var caisseDetails = (
            from fst in _db.FinShiftTravails
            join a in _db.Agents on fst.AgentId equals a.AgentId
            join c in _db.Caisses on fst.PosteTravailId equals c.PosteTravailId
            where fst.AgentId == agentId /*reservationViewModel.Reservations.AgentId*/ && fst.DateFinShift == null && c.TitreCaisse == "CAISSE ACCEUIL"
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
                ModelState.AddModelError("Reservations.AgentId", "L'agent connecté n'est pas affecté au poste de travail requis pour cette opération.");
            }
            //DateTime dtreservation = reservationViewModel.Reservation.DateReservation;
            //reservationViewModel.Reservations.DateReservation = DateTime.Today;
            DateTime dtdebutoccupation = reservationViewModel.Reservations.DateDebutOccupation;
            DateTime dtfinoccupation = reservationViewModel.Reservations.DateFinOccupation;
            double caution = reservationViewModel.Reservations.CautionReservation;
            //DateTime hrreservation = reservationViewModel.Reservation.HeureReservation;
            DateTime hrdebutoccupation = reservationViewModel.Reservations.HeureDebutOccupation;
            DateTime hrfinoccupation = reservationViewModel.Reservations.HeureFinOccupation;
            int duree=0;
            DateTime debutComplet = dtdebutoccupation.Date + hrdebutoccupation.TimeOfDay;
            DateTime finComplet = dtfinoccupation.Date + hrfinoccupation.TimeOfDay;
            string typereservation = "";
            reservationViewModel.Reservations.StatutReservation = "Confirme";
            DateTime datheureres = reservationViewModel.Reservations.DateReservation.Date + reservationViewModel.Reservations.HeureReservation.TimeOfDay;
            duree = Convert.ToInt32((finComplet - debutComplet).TotalHours);

            if (ModelState.IsValid)
            {

                //Verification de la caution
                if (caution <= 0)
                {
                    ModelState.AddModelError("Reservations.CautionReservation", "La caution est requis.");
                }
                
                if (hrdebutoccupation.Hour == 0)
                {
                    ModelState.AddModelError("Reservations.HeureDebutOccupation", "Les heures de debut d\'occupation sont comprisent entre 1h et 23h.");
                }
                if (hrfinoccupation.Hour == 0)
                {
                    ModelState.AddModelError("Reservations.HeureFinOccupation", "Les heures de fin d\'occupation sont comprisent entre 1h et 23h.");
                }

                //Verification date de debut doit être <= à la date de fin occupation
                if (dtdebutoccupation > dtfinoccupation)
                {
                    ModelState.AddModelError("Reservations.DateDebutOccupation", "La date de début d'occupation doit être inférieure ou égale à la date de fin d'occupation.");
                }

                //Verification minimum 1 heure de reservation
                if (dtdebutoccupation == dtfinoccupation)
                {
                    //Verification heure debut doit être < à heure de fin si la date de debut et de fin occupation sont égaux

                    if (hrdebutoccupation > hrfinoccupation)
                    {
                        ModelState.AddModelError("Reservations.HeureDebutOccupation", "L\' heure de debut d\'occupation ne peut pas être supérieure à l\' heure de fin d\'occupation.");
                    }

                    //Associer les dates et heures pour avoir un resultat sûr
                    debutComplet = dtdebutoccupation.Date + hrdebutoccupation.TimeOfDay;
                    finComplet = dtfinoccupation.Date + hrfinoccupation.TimeOfDay;


                    //Verifier la durée
                    TimeSpan dureeH = finComplet - debutComplet;
                    duree = Convert.ToInt32(dureeH.TotalHours);
                    if (dureeH.TotalHours < 1)
                    {
                        ModelState.AddModelError("Reservations.HeureFinOccupation", "Une reservation doit être au minimum d'une heure.");
                    }

                }
                if (datheureres > debutComplet && datheureres > finComplet)
                {
                    ModelState.AddModelError("Reservations.DateDebutOccupation", "Les différentes dates et heures ne doivent pas être inférieures à la date du jour.");
                }
                if (ModelState.IsValid)
                {
                    if (reservation.ChambreId == reservationViewModel.Reservations.ChambreId)
                    {
                        Chambre anciennechambre = _db.Chambres.FirstOrDefault(c => c.ChambreId == reservation.ChambreId);
                        if (anciennechambre is not null)
                        {
                            //var chambre = _db.Chambres.FirstOrDefault(c => c.ChambreId == finMaintenanceChambre.ChambreId);
                            if (anciennechambre.StatutChambre.Contains("Nettoyage"))
                            {
                                anciennechambre.StatutChambre = "Libre|Nettoyage";

                            }
                            else if (anciennechambre.StatutChambre.Contains("Propre"))
                            {
                                anciennechambre.StatutChambre = "Libre|Propre";
                            }
                            else if (anciennechambre.StatutChambre.Contains("Sale"))
                            {
                                anciennechambre.StatutChambre = "Libre|Sale";
                            }
                            //anciennechambre.StatutChambre = "Libre";
                            _db.Chambres.Update(anciennechambre);

                            _db.SaveChanges();
                        }
                        Chambre nouvellechambre = _db.Chambres.FirstOrDefault(c => c.ChambreId == reservationViewModel.Reservations.ChambreId);
                        if (nouvellechambre is not null)
                        {
                            if (nouvellechambre.StatutChambre.Contains("Nettoyage"))
                            {
                                nouvellechambre.StatutChambre = "Occupe|Nettoyage";

                            }
                            else if (nouvellechambre.StatutChambre.Contains("Propre"))
                            {
                                nouvellechambre.StatutChambre = "Occupe|Propre";
                            }
                            else if (nouvellechambre.StatutChambre.Contains("Sale"))
                            {
                                nouvellechambre.StatutChambre = "Occupe|Sale";
                            }
                            //nouvellechambre.StatutChambre = "Occupe";
                            _db.Chambres.Update(nouvellechambre);
                            _db.SaveChanges();
                        }
                        reservationViewModel.Reservations.AgentId = agentId;
                        _db.Entry(reservation).CurrentValues.SetValues(reservationViewModel.Reservations);
                        _db.SaveChanges();
                        var reglement = _db.ReglementReservations.FirstOrDefault(rgl => rgl.ReservationId == reservationViewModel.Reservations.ReservationId);
                        var detfactcaut = _db.DetailsFactureCautions.FirstOrDefault(df => df.Numres == reservationViewModel.Reservations.ReservationId);

                        detfactcaut.Debut = reservationViewModel.Reservations.DateDebutOccupation.Date + reservationViewModel.Reservations.HeureDebutOccupation.TimeOfDay;
                        detfactcaut.Fin = reservationViewModel.Reservations.DateFinOccupation.Date + reservationViewModel.Reservations.HeureFinOccupation.TimeOfDay;
                        //detfactcaut.Caissier = reservationViewModel.Reservations.AgentId;
                        detfactcaut.Caissier = agentId;
                        detfactcaut.Caisse = caissId;
                        detfactcaut.Chambre = reservation.Chambre.NumeroChambre.ToString();
                        detfactcaut.Duree = duree;
                        _db.Update(detfactcaut);
                        //_db.Update(detailsFactureCaution);
                        _db.SaveChanges();

                        //Envoyer Print
                        //EnvoyerPrint(reglementReservation.ReglementId);
                        // 4. Stockage de l'ID pour impression après redirection
                        TempData["success"] = "Reservation mise à jour.";
                        TempData["LastReglementId"] = detfactcaut.Numfact;
                        return RedirectToAction("FactureCaution");

                    }
                    
                    
                }
            }
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
            return View(reservationViewModel);
        }
        public IActionResult Delete(int reservationId)
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

            if (reservation is not null)
            {
                reservationViewModel.Reservations = reservation;
            }
            else
            {
                return RedirectToAction("Error", "Home");
            }
            return View(reservationViewModel);
        }

        [HttpPost]
        public IActionResult Delete(ReservationViewModel reservationViewModel)
        {
            Reservation? reservation = _db.Reservations.FirstOrDefault(a => a.ReservationId == reservationViewModel.Reservations.ReservationId);
            int caissId = 0;
            var agentIdClaim = User.Claims.FirstOrDefault(c => c.Type == "AgentId");
            var agentId = Convert.ToInt32(agentIdClaim.Value);

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
            if (reservation is not null)
            {
                Chambre anciennechambre = _db.Chambres.SingleOrDefault(c => c.ChambreId == reservation.ChambreId);
                if (anciennechambre is not null)
                {
                    if (anciennechambre.StatutChambre.Contains("Nettoyage"))
                    {
                        anciennechambre.StatutChambre = "Libre|Nettoyage";

                    }
                    else if (anciennechambre.StatutChambre.Contains("Propre"))
                    {
                        anciennechambre.StatutChambre = "Libre|Propre";
                    }
                    else if (anciennechambre.StatutChambre.Contains("Sale"))
                    {
                        anciennechambre.StatutChambre = "Libre|Sale";
                    }
                    //anciennechambre.StatutChambre = "Libre";
                    _db.Chambres.Update(anciennechambre);
                    _db.SaveChanges();

                    reservation.StatutReservation = "Annule";
                    _db.Reservations.Update(reservation);
                    _db.SaveChanges();
                    TempData["success"] = "Reservation annulée.";
                    return RedirectToAction("Index");
                }
            }

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
            return View(reservationViewModel);
        }


        public IActionResult Terminer(int reservationId)
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

            if (reservation is not null)
            {
                reservationViewModel.Reservations = reservation;
            }
            else
            {
                return RedirectToAction("Error", "Home");
            }
            return View(reservationViewModel);
        }

        [HttpPost]
        public IActionResult Terminer(ReservationViewModel reservationViewModel)
        {
            Reservation? reservation = _db.Reservations.FirstOrDefault(a => a.ReservationId == reservationViewModel.Reservations.ReservationId);
            int caissId = 0;
            var agentIdClaim = User.Claims.FirstOrDefault(c => c.Type == "AgentId");
            var agentId = Convert.ToInt32(agentIdClaim.Value);

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
            if (reservation is not null)
            {
                Chambre anciennechambre = _db.Chambres.SingleOrDefault(c => c.ChambreId == reservation.ChambreId);
                if (anciennechambre is not null)
                {
                    if (anciennechambre.StatutChambre.Contains("Nettoyage"))
                    {
                        anciennechambre.StatutChambre = "Libre|Nettoyage";

                    }
                    else if (anciennechambre.StatutChambre.Contains("Propre"))
                    {
                        anciennechambre.StatutChambre = "Libre|Propre";
                    }
                    else if (anciennechambre.StatutChambre.Contains("Sale"))
                    {
                        anciennechambre.StatutChambre = "Libre|Sale";
                    }
                    //anciennechambre.StatutChambre = "Libre";
                    _db.Chambres.Update(anciennechambre);
                    _db.SaveChanges();

                    reservation.StatutReservation = "Termine";
                    _db.Reservations.Update(reservation);
                    _db.SaveChanges();
                    TempData["success"] = "Reservation terminée.";
                    return RedirectToAction("Index");
                }
            }

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
            return View(reservationViewModel);
        }

        public class RetraitService
        {
            public int serviceId { get; set; }
            public int reservationId { get; set; }
        }
        [HttpPost]
        public IActionResult RetirerService([FromBody] RetraitService data)
        {
            try
            {
                Inclure inclure =  _db.Inclures
                    .FirstOrDefault(i => i.ServiceId == data.serviceId && i.ReservationId == data.reservationId);

                if (inclure == null)
                {
                    return Json(new { success = false, message = "Inclusion introuvable" });
                }

                _db.Inclures.Remove(inclure);
                _db.SaveChanges();

                return Json(new { success = true, message = "Service retiré avec succès" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Erreur : " + ex.Message });
            }
        }
        public class InsererService
        {
            public List<int> ServiceIds { get; set; }
        }

        [HttpPost]
        public IActionResult InsertionServices([FromBody] InsererService data, int reservationId)
        {
            try
            {
                // Traitement pour les services
                if (data.ServiceIds != null && data.ServiceIds.Any())
                {
                    foreach (var service in data.ServiceIds)
                    {
                        Inclure inclure = new()
                        {
                            ServiceId = service,
                            ReservationId = reservationId
                        };
                        _db.Inclures.Add(inclure);
                    }
                }

                _db.SaveChanges();


                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, error = ex.Message });
            }
        }
    }
}
