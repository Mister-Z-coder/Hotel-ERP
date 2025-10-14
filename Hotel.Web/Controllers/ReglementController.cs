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
    [Authorize(Roles = "ADMIN,ADMINISTRATEUR")]
    [CaisseRedirectFilter]
    public class ReglementController : Controller
    {
        private readonly ApplicationDbContext _db;

        public ReglementController(ApplicationDbContext db)
        {
            _db = db;
        }

        public IActionResult Index(int page = 1, int size = 20, string searchAgent = "", string searchCaisse = "", string searchMotif = "", string searchDatedebut = "", string searchDatefin = "")
        {
            DateTime parsedDatedebut;
            bool isDateValid = DateTime.TryParse(searchDatedebut, out parsedDatedebut);

            DateTime parsedDatefin;
            isDateValid = DateTime.TryParse(searchDatefin, out parsedDatefin);

            int totalreglements = _db.Reglements.Count();
            int skipValue = (page - 1) * size;
            int totalPages = (int)Math.Ceiling((double)totalreglements / size);
            var reglements = _db.Reglements
                .Include(r =>r.Agent)
                .Include(r => r.ModeReglement)
                .Include(r => r.Devise)
                .Include(r => r.TypeReglement)
                .Include(r => r.Caisse)
                .Include(r => r.Devise)
                .OrderByDescending(r=>r.ReglementId)
                .Where(r =>
                  (string.IsNullOrEmpty(searchAgent) || r.Agent.NomAgent.ToUpper().Contains(searchAgent.ToUpper())) &&
                  (string.IsNullOrEmpty(searchCaisse) || r.Caisse.TitrePosteTravail.ToUpper().Contains(searchCaisse.ToUpper())) &&
                  (string.IsNullOrEmpty(searchMotif) || r.MotifReglement.ToUpper().Contains(searchMotif.ToUpper())) &&
                  (string.IsNullOrEmpty(searchDatedebut) || r.DateReglement == Convert.ToDateTime(parsedDatedebut.Date)) &&
                  (string.IsNullOrEmpty(searchDatefin) || (r.DateReglement >= Convert.ToDateTime(parsedDatefin.Date) && r.DateReglement <= Convert.ToDateTime(parsedDatefin.Date)))
              )
               
                .ToList();
            var depart = (totalPages - 1) * skipValue + 1;
            ViewBag.NumAff = depart;
            ViewBag.CurrentPage = page;
            ViewBag.PageSize = size;
            ViewBag.TotalPage = totalPages;
            
            ReglementViewModel reglementViewModel = new()
            {
                Reglements = reglements
            };
            return View(reglementViewModel);
        }

        [Authorize(Roles = "ADMIN,SUPERVISEUR")]
        public IActionResult Create()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            int userId = userIdClaim != null ? int.Parse(userIdClaim.Value) : 0;
            Agent agent = _db.Agents.FirstOrDefault(a => a.AgentId == userId);


            ReglementViewModel ReglementVM = new()
            {
                AgentList = _db.Agents.ToList().Where(a => a.AgentId == userId).Select(a => new SelectListItem
                {
                    Text = $"{a.NomAgent} | Prénom : {a.PrenomAgent} | Sexe : {a.SexeAgent} | Nationalité : {a.Nationalite_Agent}",
                    Value = a.AgentId.ToString()
                }),
                
                ModeReglementList = _db.ModeReglements.ToList().Select(m => new SelectListItem
                {
                    Text = $"{m.TitreModeReglement.ToString()}",
                    Value = m.ModeReglementId.ToString()
                }).ToList(),

                TypeReglementList = _db.TypeReglements.ToList().Select(t => new SelectListItem
                {
                    Text = $"{t.TitreTypeReglement.ToString()}",
                    Value = t.TypeReglementId.ToString()
                }).ToList(),

                DeviseList = _db.Devises.ToList().Select(d => new SelectListItem
                {
                    Text = $"{d.SigleDevise} : {d.TitreDevise}",
                    Value = d.DeviseId.ToString()
                }).ToList(),
                CaisseList = _db.Caisses.ToList().Select(d => new SelectListItem
                {
                    Text = $"{d.TitreCaisse}",
                    Value = d.PosteTravailId.ToString()
                }).ToList(),

                Reglement = new Reglement{
                    AgentId = userId
                }

            };
            return View(ReglementVM);
        }
        private void ValidationDate(DateTime date, string fieldName, string errorMessage)
        {
            if (date < DateTime.Today)
            {
                ModelState.AddModelError(fieldName, errorMessage);
            }
        }

        [HttpPost]
        [Authorize(Roles = "ADMIN,SUPERVISEUR")]
        public IActionResult Create(ReglementViewModel reglementViewModel)
        {
            DateTime dtreglement = reglementViewModel.Reglement.DateReglement;
            DateTime hrreglement = reglementViewModel.Reglement.HeureReglement;
            decimal montant = reglementViewModel.Reglement.MontantReglement;
            //ModelState.Remove("Reservation.StatutReservation");
            //reglementViewModel.Reservation.StatutReservation = "Confirme";

            if (ModelState.IsValid)
            {

                ValidationDate(dtreglement, "Reglement.DateReglement",
                         "La date de réservation ne peut pas être antérieure à la date du jour.");


                if (montant <= 0)
                {
                    ModelState.AddModelError("Reglement.MontantReglement", "Le montant de reglement est requis.");
                }
                if (dtreglement > DateTime.Today)
                {
                    ModelState.AddModelError("Reglement.DateReservation", "La date de réservation ne peut pas être supérieur à la date du jour.");
                }
                if (ModelState.IsValid)
                {
                    Agent agent = _db.Agents.SingleOrDefault(a => a.AgentId == reglementViewModel.Reglement.AgentId);
                    Devise devise = _db.Devises.SingleOrDefault(d => d.DeviseId == reglementViewModel.Reglement.DeviseId);
                    ModeReglement modeReglement = _db.ModeReglements.SingleOrDefault(m => m.ModeReglementId == reglementViewModel.Reglement.ModeReglementId);
                    TypeReglement typeReglement = _db.TypeReglements.SingleOrDefault(t => t.TypeReglementId == reglementViewModel.Reglement.TypeReglementId);
                    Caisse caisse = _db.Caisses.SingleOrDefault(c => c.PosteTravailId == reglementViewModel.Reglement.CaisseId);
                    if (agent is not null && devise is not null && modeReglement is not null && typeReglement is not null && caisse is not null)
                    {
                        _db.Reglements.Add(reglementViewModel.Reglement);
                        _db.SaveChanges();


                        TempData["success"] = "Reglement effectuée.";
                        return RedirectToAction("Index");
                    }

                }

            }


            reglementViewModel.AgentList = _db.Agents.ToList().Select(a => new SelectListItem
            {
                Text = $"{a.NomAgent} | Prénom : {a.PrenomAgent} | Sexe : {a.SexeAgent} | Nationalité : {a.Nationalite_Agent}",
                Value = a.AgentId.ToString()
            });

            reglementViewModel.ModeReglementList = _db.ModeReglements.ToList().Select(m => new SelectListItem
            {
                Text = $"{m.TitreModeReglement.ToString()}",
                Value = m.ModeReglementId.ToString()
            }).ToList();

            reglementViewModel.TypeReglementList = _db.TypeReglements.ToList().Select(t => new SelectListItem
            {
                Text = $"{t.TitreTypeReglement.ToString()}",
                Value = t.TypeReglementId.ToString()
            }).ToList();

            reglementViewModel.DeviseList = _db.Devises.ToList().Select(d => new SelectListItem
            {
                Text = $"{d.SigleDevise} : {d.TitreDevise}",
                Value = d.DeviseId.ToString()
            }).ToList();
            reglementViewModel.CaisseList = _db.Caisses.ToList().Select(d => new SelectListItem
            {
                Text = $"{d.TitreCaisse}",
                Value = d.PosteTravailId.ToString()
            }).ToList();
            
            return View(reglementViewModel);
        }

        [Authorize(Roles = "ADMIN,SUPERVISEUR")]
        public IActionResult Update(int reglementId)
        {
            //var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            //int userId = userIdClaim != null ? int.Parse(userIdClaim.Value) : 0;
            //Agent agent = _db.Agents.FirstOrDefault(a => a.AgentId == userId);
            Reglement reglement = _db.Reglements.FirstOrDefault(r=>r.ReglementId== reglementId);

            ReglementViewModel reglementViewModel = new()
            {
                AgentList = _db.Agents.ToList().Select(a => new SelectListItem
                {
                    Text = $"{a.NomAgent} | Prénom : {a.PrenomAgent} | Sexe : {a.SexeAgent} | Nationalité : {a.Nationalite_Agent}",
                    Value = a.AgentId.ToString()
                }),

                ModeReglementList = _db.ModeReglements.ToList().Select(m => new SelectListItem
                {
                    Text = $"{m.TitreModeReglement.ToString()}",
                    Value = m.ModeReglementId.ToString()
                }).ToList(),

                TypeReglementList = _db.TypeReglements.ToList().Select(t => new SelectListItem
                {
                    Text = $"{t.TitreTypeReglement.ToString()}",
                    Value = t.TypeReglementId.ToString()
                }).ToList(),

                DeviseList = _db.Devises.ToList().Select(d => new SelectListItem
                {
                    Text = $"{d.SigleDevise} : {d.TitreDevise}",
                    Value = d.DeviseId.ToString()
                }).ToList(),
                CaisseList = _db.Caisses.ToList().Select(d => new SelectListItem
                {
                    Text = $"{d.TitreCaisse}",
                    Value = d.PosteTravailId.ToString()
                }).ToList(),

                //Reglement = new Reglement
                //{
                //    AgentId = userId
                //}
            };

            if (reglement is not null)
            {
                reglement.MontantReglement = Convert.ToInt32(reglement.MontantReglement);
                reglement.HeureReglement = DateTime.Parse(reglement.HeureReglement.ToShortTimeString());
                reglementViewModel.Reglement = reglement;
            }
            else
            {
                return RedirectToAction("Error", "Home");
            }
            return View(reglementViewModel);
        }

        [HttpPost]
        [Authorize(Roles = "ADMIN,SUPERVISEUR")]
        public IActionResult Update(ReglementViewModel reglementViewModel)
        {
            DateTime dtreglement = reglementViewModel.Reglement.DateReglement;
            DateTime hrreglement = reglementViewModel.Reglement.HeureReglement;
            decimal montant = reglementViewModel.Reglement.MontantReglement;
            //ModelState.Remove("Reservation.StatutReservation");
            //reglementViewModel.Reservation.StatutReservation = "Confirme";

            if (ModelState.IsValid)
            {
              

                if (montant <= 0)
                {
                    ModelState.AddModelError("Reglement.MontantReglement", "Le montant de reglement est requis.");
                }
               
                
                if (ModelState.IsValid)
                {
                    Agent agent = _db.Agents.SingleOrDefault(a => a.AgentId == reglementViewModel.Reglement.AgentId);
                    Devise devise = _db.Devises.SingleOrDefault(d => d.DeviseId == reglementViewModel.Reglement.DeviseId);
                    ModeReglement modeReglement = _db.ModeReglements.SingleOrDefault(m => m.ModeReglementId == reglementViewModel.Reglement.ModeReglementId);
                    TypeReglement typeReglement = _db.TypeReglements.SingleOrDefault(t => t.TypeReglementId == reglementViewModel.Reglement.TypeReglementId);
                    Caisse caisse = _db.Caisses.SingleOrDefault(c => c.PosteTravailId == reglementViewModel.Reglement.CaisseId);
                    
                    if (agent is not null && devise is not null && modeReglement is not null && typeReglement is not null && caisse is not null)
                    {
                        _db.Reglements.Update(reglementViewModel.Reglement);
                        _db.SaveChanges();


                        TempData["success"] = "Reglement mis à jour.";
                        return RedirectToAction("Index");
                    }

                }

            }


            reglementViewModel.AgentList = _db.Agents.ToList().Select(a => new SelectListItem
            {
                Text = $"{a.NomAgent} | Prénom : {a.PrenomAgent} | Sexe : {a.SexeAgent} | Nationalité : {a.Nationalite_Agent}",
                Value = a.AgentId.ToString()
            });

            reglementViewModel.ModeReglementList = _db.ModeReglements.ToList().Select(m => new SelectListItem
            {
                Text = $"{m.TitreModeReglement.ToString()}",
                Value = m.ModeReglementId.ToString()
            }).ToList();

            reglementViewModel.TypeReglementList = _db.TypeReglements.ToList().Select(t => new SelectListItem
            {
                Text = $"{t.TitreTypeReglement.ToString()}",
                Value = t.TypeReglementId.ToString()
            }).ToList();

            reglementViewModel.DeviseList = _db.Devises.ToList().Select(d => new SelectListItem
            {
                Text = $"{d.SigleDevise} : {d.TitreDevise}",
                Value = d.DeviseId.ToString()
            }).ToList();
            reglementViewModel.CaisseList = _db.Caisses.ToList().Select(d => new SelectListItem
            {
                Text = $"{d.TitreCaisse}",
                Value = d.PosteTravailId.ToString()
            }).ToList();

            return View(reglementViewModel);
        }
        [Authorize(Roles = "ADMIN,SUPERVISEUR")]
        public IActionResult Delete(int reglementId)
        {
            //var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            //int userId = userIdClaim != null ? int.Parse(userIdClaim.Value) : 0;
            //Agent agent = _db.Agents.FirstOrDefault(a => a.AgentId == userId);
            Reglement reglement = _db.Reglements.FirstOrDefault(r => r.ReglementId == reglementId);

            ReglementViewModel reglementViewModel = new()
            {
                AgentList = _db.Agents.ToList().Select(a => new SelectListItem
                {
                    Text = $"{a.NomAgent} | Prénom : {a.PrenomAgent} | Sexe : {a.SexeAgent} | Nationalité : {a.Nationalite_Agent}",
                    Value = a.AgentId.ToString()
                }),

                ModeReglementList = _db.ModeReglements.ToList().Select(m => new SelectListItem
                {
                    Text = $"{m.TitreModeReglement.ToString()}",
                    Value = m.ModeReglementId.ToString()
                }).ToList(),

                TypeReglementList = _db.TypeReglements.ToList().Select(t => new SelectListItem
                {
                    Text = $"{t.TitreTypeReglement.ToString()}",
                    Value = t.TypeReglementId.ToString()
                }).ToList(),

                DeviseList = _db.Devises.ToList().Select(d => new SelectListItem
                {
                    Text = $"{d.SigleDevise} : {d.TitreDevise}",
                    Value = d.DeviseId.ToString()
                }).ToList(),
                CaisseList = _db.Caisses.ToList().Select(d => new SelectListItem
                {
                    Text = $"{d.TitreCaisse}",
                    Value = d.PosteTravailId.ToString()
                }).ToList(),

                //Reglement = new Reglement
                //{
                //    AgentId = userId
                //}
            };

            if (reglement is not null)
            {

                reglement.HeureReglement = DateTime.Parse(reglement.HeureReglement.ToShortTimeString());
                reglementViewModel.Reglement = reglement;
            }
            else
            {
                return RedirectToAction("Error", "Home");
            }
            return View(reglementViewModel);

        }

        [Authorize(Roles = "ADMIN,SUPERVISEUR")]
        [HttpPost]
        public IActionResult Delete(ReglementViewModel reglementViewModel)
        {
            try
            {
                Reglement? reglement = _db.Reglements.FirstOrDefault(r => r.ReglementId == reglementViewModel.Reglement.ReglementId);

                if (reglement is not null)
                {
                    //reglement.StatutReservation = "Annule";
                    _db.Reglements.Remove(reglement);
                    _db.SaveChanges();
                    TempData["success"] = "Reservation supprimée.";
                    return RedirectToAction("Index");
                }

                reglementViewModel.AgentList = _db.Agents.ToList().Select(a => new SelectListItem
                {
                    Text = $"{a.NomAgent} | Prénom : {a.PrenomAgent} | Sexe : {a.SexeAgent} | Nationalité : {a.Nationalite_Agent}",
                    Value = a.AgentId.ToString()
                });

                reglementViewModel.ModeReglementList = _db.ModeReglements.ToList().Select(m => new SelectListItem
                {
                    Text = $"{m.TitreModeReglement.ToString()}",
                    Value = m.ModeReglementId.ToString()
                }).ToList();

                reglementViewModel.TypeReglementList = _db.TypeReglements.ToList().Select(t => new SelectListItem
                {
                    Text = $"{t.TitreTypeReglement.ToString()}",
                    Value = t.TypeReglementId.ToString()
                }).ToList();

                reglementViewModel.DeviseList = _db.Devises.ToList().Select(d => new SelectListItem
                {
                    Text = $"{d.SigleDevise} : {d.TitreDevise}",
                    Value = d.DeviseId.ToString()
                }).ToList();
                reglementViewModel.CaisseList = _db.Caisses.ToList().Select(d => new SelectListItem
                {
                    Text = $"{d.TitreCaisse}",
                    Value = d.PosteTravailId.ToString()
                }).ToList();

            }
            catch
            {
                TempData["error"] = "Impossible de supprimer le reglement.";

            }
            return View(reglementViewModel);
        }

    public class RepportData
    {
        public string Motif { get; set; }   
        public DateTime Datefin { get; set; }      
        public DateTime Datedebut { get; set; }
    }


        public IActionResult GenererDonnesRapport([FromBody] RepportData data)
        {
            try
            {
                //using var transaction = _db.Database.BeginTransaction();
                var reglements = _db.Reglements
                    .Where(r => r.DateReglement >= data.Datedebut
                             && r.DateReglement <= data.Datefin
                             && r.MotifReglement.ToUpper().Contains(data.Motif.ToUpper()))
                    .Include(r => r.Agent)
                    .Include(r => r.ModeReglement)
                    .Include(r => r.TypeReglement)
                    .Include(r => r.Caisse)
                    .Include(r => r.Devise)
                    .AsNoTracking()
                    .ToList();

                _db.DetailsReglements.RemoveRange(_db.DetailsReglements);
                _db.SaveChanges();

                _db.DetailsReglements.AddRange(reglements.Select(r => new DetailsReglement
                {
                    ReglementId = r.ReglementId,
                    Agent = r.Agent.NomAgent,
                    Caisse = r.Caisse.TitreCaisse,
                    Devise = r.Devise.SigleDevise,
                    DateReglement = r.DateReglement,
                    HeureReglement = r.HeureReglement,
                    ModeReglement = r.ModeReglement.TitreModeReglement,
                    TypeReglement = r.TypeReglement.TitreTypeReglement,
                    MontantReglement = r.MontantReglement,
                    MotifReglement = r.MotifReglement
                }));

                _db.SaveChanges();

                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                // Ajoutez ceci pour voir l'erreur réelle
                var innerException = ex.InnerException?.Message;
                Console.WriteLine($"Erreur interne : {innerException}");
                return Json(new
                {
                    success = false,
                    message = ex.Message
                });
            }
        }
    }
}
