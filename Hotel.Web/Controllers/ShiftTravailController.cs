using Hotel.Data;
using Hotel.Models;
using Hotel.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Hotel.Controllers
{

    [Authorize]
    [Authorize(Roles = "ADMIN,ADMINISTRATEUR")]
    [CaisseRedirectFilter]
    public class ShiftTravailController : Controller
    {
        private readonly ApplicationDbContext _db;
        
        public ShiftTravailController(ApplicationDbContext db)
        {
            _db = db;
        }

        public IActionResult Index(int page = 1, int size = 20, string searchDate = "", string searchAgent = "", string searchPoste = "")
        {

            DateTime parsedDate;
            bool isDateValid = DateTime.TryParse(searchDate, out parsedDate);
            int totalshiftTravail = _db.ShiftTravails.OfType<FinShiftTravail>().Count();
            int skipValue = (page - 1) * size;
            int totalPages = (int)Math.Ceiling((double)totalshiftTravail / size);
            var finShiftTravail = _db.ShiftTravails.OfType<FinShiftTravail>()
                .OrderByDescending(f => f.ShiftTravailId)               // Trier par date de fin shift
                .ThenBy(f=>f.Agent.NomAgent)
                .Include(f=>f.Agent)
                .Include(f=>f.PosteTravail)
                .Where(f =>
                           (string.IsNullOrEmpty(searchPoste) || f.PosteTravail.TitrePosteTravail!.ToUpper().Contains(searchPoste.ToString())) &&
                           (string.IsNullOrEmpty(searchDate) || f.DateDebutShift! == Convert.ToDateTime(parsedDate.Date)) &&
                           (string.IsNullOrEmpty(searchAgent) || f.Agent.NomAgent!.ToUpper().Contains(searchAgent.ToUpper()) || f.Agent.PrenomAgent!.ToUpper().Contains(searchAgent.ToUpper()))
                           ).ToList().Skip(skipValue).Take(size);
            var depart = (totalPages - 1) * skipValue + 1;
            ViewBag.NumAff = depart;
            ViewBag.CurrentPage = page;
            ViewBag.PageSize = size;
            ViewBag.TotalPage = totalPages;
            ShiftTravailViewModel shiftTravailViewModel = new()
            {
                FinShiftTravails = finShiftTravail
            };
            return View(shiftTravailViewModel);
        }
        public IActionResult Create()
        {
            ShiftTravailViewModel shiftTravailViewModel = new()
            {
                AgentList = _db.Agents.ToList().Select(a => new SelectListItem
                {
                    Text = $"{a.NomAgent} | Prénom : {a.PrenomAgent} | Sexe : {a.SexeAgent} | Nationalité : {a.Nationalite_Agent}",
                    Value = a.AgentId.ToString()
                }),
                //
                PosteTravailList = _db.PosteTravails.ToList().Select(m => new SelectListItem
                {
                    Text = $"{m.TitrePosteTravail}",
                    Value = m.PosteTravailId.ToString()
                }).ToList()
            };
            return View(shiftTravailViewModel);
        }
        private void ValidationDate(DateTime? date, string fieldName, string errorMessage)
        {
            if (date != DateTime.Today)
            {
                ModelState.AddModelError(fieldName, errorMessage);
            }
        }

        [HttpPost]
        public IActionResult Create(ShiftTravailViewModel shiftTravailViewModel)
        {
            //ShiftTravail exist = _db.ShiftTravails.Where(a=>a.NomAgent== agent.NomAgent && a.PrenomAgent == agent.PrenomAgent).FirstOrDefault(a => a.AgentId != agent.AgentId);
           
            ModelState.Remove("FinShiftTravail.DateFinShift");
            ModelState.Remove("FinShiftTravail.HeureFinShift");
            ModelState.Remove("FinShiftTravail.ObservationFinShift");
            //shiftTravailViewModel.FinShiftTravail.DateDebutShift = shiftTravailViewModel.ShiftTravail.DateDebutShift;
            //shiftTravailViewModel.FinShiftTravail.HeureDebutShift = shiftTravailViewModel.ShiftTravail.HeureDebutShift;
            //shiftTravailViewModel.FinShiftTravail.ObservationDebutShift = shiftTravailViewModel.ShiftTravail.ObservationDebutShift;
            DateTime dtdebutshift = shiftTravailViewModel.ShiftTravail.DateDebutShift.Value;
            bool IsMultishift = shiftTravailViewModel.IsMultishift;
            if (shiftTravailViewModel.ShiftTravail.DateDebutShift is null)
            {
                ModelState.AddModelError("ShiftTravail.DateDebutShift", "Date de debut de shift requis.");
            }
            if (shiftTravailViewModel.ShiftTravail.HeureDebutShift is null)
            {
                ModelState.AddModelError("ShiftTravail.HeureDebutShift", "Heure de fin de shift requis.");
            }
            if (shiftTravailViewModel.ShiftTravail.ObservationDebutShift is null)
            {
                ModelState.AddModelError("ShiftTravail.ObservationDebutShift", "Observation de debut de shift requis.");
            }
            if (shiftTravailViewModel.ShiftTravail.HeureDebutShift.Value.Hour == 0)
            {
                ModelState.AddModelError("ShiftTravail.HeureDebutShift", "Les heures de debut de shift doivent être comprisent entre 1h et 23h.");
            }

            if (ModelState.IsValid)
            {
                ValidationDate(dtdebutshift, "ShiftTravail.DateDebutShift",
                             "La date de début du shift doit être la date du jour.");
                
                if (ModelState.IsValid)
                {
                    if(IsMultishift is false)
                    {
                        DateTime? defautDate = null;
                        FinShiftTravail agentdejaenshift = _db.ShiftTravails.OfType<FinShiftTravail>().Where(s => s.DateFinShift == defautDate).FirstOrDefault(s => s.AgentId == shiftTravailViewModel.ShiftTravail.AgentId);
                        if (agentdejaenshift is not null)
                        {
                            TempData["error"] = "Cet agent est déjà en shift, veuillez d'abord le terminer.";
                            return RedirectToAction("Index");
                        }
                        FinShiftTravail finShiftTravail = new FinShiftTravail()
                        {
                            AgentId = shiftTravailViewModel.ShiftTravail.AgentId,
                            PosteTravailId = shiftTravailViewModel.ShiftTravail.PosteTravailId,
                            DateDebutShift = shiftTravailViewModel.ShiftTravail.DateDebutShift,
                            HeureDebutShift = shiftTravailViewModel.ShiftTravail.HeureDebutShift,
                            ObservationDebutShift = shiftTravailViewModel.ShiftTravail.ObservationDebutShift,
                            DateFinShift = null,
                            HeureFinShift = null
                        };

                        _db.FinShiftTravails.Add(finShiftTravail);
                        _db.SaveChanges();
                        TempData["success"] = "Shift Travail crée.";
                        return RedirectToAction("Index");
                    }
                    else
                    {
                        FinShiftTravail finShiftTravail = new FinShiftTravail()
                        {
                            AgentId = shiftTravailViewModel.ShiftTravail.AgentId,
                            PosteTravailId = shiftTravailViewModel.ShiftTravail.PosteTravailId,
                            DateDebutShift = shiftTravailViewModel.ShiftTravail.DateDebutShift,
                            HeureDebutShift = shiftTravailViewModel.ShiftTravail.HeureDebutShift,
                            ObservationDebutShift = shiftTravailViewModel.ShiftTravail.ObservationDebutShift,
                            DateFinShift = null,
                            HeureFinShift = null
                        };

                        _db.FinShiftTravails.Add(finShiftTravail);
                        _db.SaveChanges();
                        TempData["success"] = "Shift Travail crée.";
                        return RedirectToAction("Index");
                    }
                    
                }
            }

            shiftTravailViewModel.AgentList = _db.Agents.ToList().Select(a => new SelectListItem
            {
                Text = $"{a.NomAgent} | Prénom : {a.PrenomAgent} | Sexe : {a.SexeAgent} | Nationalité : {a.Nationalite_Agent}",
                Value = a.AgentId.ToString()
            });

            shiftTravailViewModel.PosteTravailList = _db.PosteTravails.ToList().Select(m => new SelectListItem
            {
                Text = $"{m.TitrePosteTravail.ToString()}",
                Value = m.PosteTravailId.ToString()
            }).ToList();
            return View(shiftTravailViewModel);
        }

        public IActionResult UpdateShiftEncours(int shiftTravailId)
        {
            ShiftTravail shiftTravail = _db.ShiftTravails.FirstOrDefault(s => s.ShiftTravailId == shiftTravailId);

            if (shiftTravail is null)
            {
                return RedirectToAction("Error", "Home");
            }
            ShiftTravailViewModel shiftTravailViewModel = new();

            shiftTravailViewModel.AgentList = _db.Agents.ToList().Select(a => new SelectListItem
            {
                Text = $"{a.NomAgent} | Prénom : {a.PrenomAgent} | Sexe : {a.SexeAgent} | Nationalité : {a.Nationalite_Agent}",
                Value = a.AgentId.ToString()
            });

            //shiftTravailViewModel.PosteTravailList = _db.PosteTravails.Where(p => p.PosteTravailId != shiftTravail.PosteTravailId).ToList().Select(p => new SelectListItem
            //{
            //    Text = $"{p.TitrePosteTravail.ToString()}",
            //    Value = p.PosteTravailId.ToString()
            //}).ToList();
            shiftTravailViewModel.PosteTravailList = _db.PosteTravails.ToList().Select(m => new SelectListItem
            {
                Text = $"{m.TitrePosteTravail}",
                Value = m.PosteTravailId.ToString()
            }).ToList();

            shiftTravailViewModel.ShiftTravail = shiftTravail;
            return View(shiftTravailViewModel);
        }
        
        [HttpPost]
        public IActionResult UpdateShiftEncours(ShiftTravailViewModel shiftTravailViewModel)
        {
            
            ModelState.Remove("FinShiftTravail.DateFinShift");
            ModelState.Remove("FinShiftTravail.HeureFinShift");
            ModelState.Remove("FinShiftTravail.ObservationFinShift");
            //shiftTravailViewModel.FinShiftTravail.DateDebutShift = shiftTravailViewModel.ShiftTravail.DateDebutShift;
            //shiftTravailViewModel.FinShiftTravail.HeureDebutShift = shiftTravailViewModel.ShiftTravail.HeureDebutShift;
            //shiftTravailViewModel.FinShiftTravail.ObservationDebutShift = shiftTravailViewModel.ShiftTravail.ObservationDebutShift;
            //ShiftTravail exist = _db.ShiftTravails.Where(a=>a.NomAgent== agent.NomAgent && a.PrenomAgent == agent.PrenomAgent).FirstOrDefault(a => a.AgentId != agent.AgentId);
            ShiftTravail shift = _db.ShiftTravails.FirstOrDefault(s => s.ShiftTravailId == shiftTravailViewModel.ShiftTravail.ShiftTravailId);
            if (shift is null)
            {
                return RedirectToAction("Error", "Home");
            }

            shift.AgentId = shiftTravailViewModel.ShiftTravail.AgentId;
            shift.PosteTravailId = shiftTravailViewModel.ShiftTravail.PosteTravailId;
            shift.DateDebutShift = shiftTravailViewModel.ShiftTravail.DateDebutShift;
            shift.HeureDebutShift = shiftTravailViewModel.ShiftTravail.HeureDebutShift;
            shift.ObservationDebutShift = shiftTravailViewModel.ShiftTravail.ObservationDebutShift;

            DateTime dtdebutshift = shiftTravailViewModel.ShiftTravail.DateDebutShift.Value;
            if (shiftTravailViewModel.ShiftTravail.DateDebutShift is null)
            {
                ModelState.AddModelError("ShiftTravail.DateDebutShift", "Date de debut de shift requis.");
            }
            if (shiftTravailViewModel.ShiftTravail.HeureDebutShift is null)
            {
                ModelState.AddModelError("ShiftTravail.HeureDebutShift", "Heure de fin de shift requis.");
            }
            if (shiftTravailViewModel.ShiftTravail.ObservationDebutShift is null)
            {
                ModelState.AddModelError("ShiftTravail.ObservationDebutShift", "Observation de debut de shift requis.");
            }
            if (shiftTravailViewModel.ShiftTravail.HeureDebutShift.Value.Hour == 0)
            {
                ModelState.AddModelError("ShiftTravail.HeureDebutShift", "Les heures de debut de shift doivent être comprisent entre 1h et 23h.");
            }
            if (ModelState.IsValid)
            {
                //ValidationDate(dtdebutshift, "ShiftTravail.DateDebutShift",
                //             "La date de début du shift doit être la date du jour.");

                if (ModelState.IsValid)
                {
                    _db.ShiftTravails.Update(shift);
                    _db.SaveChanges();
                    TempData["success"] = "Shift Travail mis à jour.";
                    return RedirectToAction("Index");
                }
            }

            shiftTravailViewModel.AgentList = _db.Agents.ToList().Select(a => new SelectListItem
            {
                Text = $"{a.NomAgent} | Prénom : {a.PrenomAgent} | Sexe : {a.SexeAgent} | Nationalité : {a.Nationalite_Agent}",
                Value = a.AgentId.ToString()
            });

            shiftTravailViewModel.PosteTravailList = _db.PosteTravails.ToList().Select(m => new SelectListItem
            {
                Text = $"{m.TitrePosteTravail}",
                Value = m.PosteTravailId.ToString()
            }).ToList();
            return View(shiftTravailViewModel);
        }
        public IActionResult UpdateShiftTermine(int shiftTravailId)
        {
            FinShiftTravail finshiftTravail = _db.ShiftTravails.OfType<FinShiftTravail>().FirstOrDefault(s => s.ShiftTravailId == shiftTravailId);

            if (finshiftTravail is null)
            {
                return RedirectToAction("Error", "Home");
            }
            ShiftTravailViewModel shiftTravailViewModel = new();

            shiftTravailViewModel.AgentList = _db.Agents.ToList().Select(a => new SelectListItem
            {
                Text = $"{a.NomAgent} | Prénom : {a.PrenomAgent} | Sexe : {a.SexeAgent} | Nationalité : {a.Nationalite_Agent}",
                Value = a.AgentId.ToString()
            });

            shiftTravailViewModel.PosteTravailList = _db.PosteTravails.ToList().Select(p => new SelectListItem
            {
                Text = $"{p.TitrePosteTravail}",
                Value = p.PosteTravailId.ToString()
            }).ToList();
            shiftTravailViewModel.FinShiftTravail = finshiftTravail;

            return View(shiftTravailViewModel);
        }

        [HttpPost]
        public IActionResult UpdateShiftTermine(ShiftTravailViewModel shiftTravailViewModel)
        {
            ModelState.Remove("FinShiftTravail.DateDebutShift");
            ModelState.Remove("FinShiftTravail.HeureDebutShift");
            ModelState.Remove("FinShiftTravail.DateFinShift");
            ModelState.Remove("FinShiftTravail.HeureFinShift");
            ModelState.Remove("FinShiftTravail.ObservationDebutShift");
            ModelState.Remove("FinShiftTravail.AgentId");
            ModelState.Remove("FinShiftTravail.PosteTravailId");

            //ShiftTravail exist = _db.ShiftTravails.Where(a=>a.NomAgent== agent.NomAgent && a.PrenomAgent == agent.PrenomAgent).FirstOrDefault(a => a.AgentId != agent.AgentId);
            FinShiftTravail finShift = _db.FinShiftTravails.FirstOrDefault(s => s.ShiftTravailId == shiftTravailViewModel.FinShiftTravail.ShiftTravailId);
            if (finShift is null)
            {
                return RedirectToAction("Error", "Home");
            }

            //finShift.DateFinShift = shiftTravailViewModel.FinShiftTravail.DateFinShift;
            //finShift.HeureFinShift = shiftTravailViewModel.FinShiftTravail.HeureFinShift;
            finShift.ObservationFinShift = shiftTravailViewModel.FinShiftTravail.ObservationFinShift;

            DateTime? dtfinshift = finShift.DateFinShift;

            

            if (ModelState.IsValid)
            {
                if (ModelState.IsValid)
                {

                    _db.FinShiftTravails.Update(finShift);
                    _db.SaveChanges();
                    TempData["success"] = "Shift Travail mis à jour.";
                    return RedirectToAction("Index");
                }
            }

            shiftTravailViewModel.AgentList = _db.Agents.ToList().Select(a => new SelectListItem
            {
                Text = $"{a.NomAgent} | Prénom : {a.PrenomAgent} | Sexe : {a.SexeAgent} | Nationalité : {a.Nationalite_Agent}",
                Value = a.AgentId.ToString()
            });

            shiftTravailViewModel.PosteTravailList = _db.PosteTravails.ToList().Select(m => new SelectListItem
            {
                Text = $"{m.TitrePosteTravail}",
                Value = m.PosteTravailId.ToString()
            }).ToList();
            shiftTravailViewModel.FinShiftTravail = finShift;

            return View(shiftTravailViewModel);

        }
        public IActionResult Terminer(int shiftTravailId)
        {
            FinShiftTravail finshiftTravail = _db.ShiftTravails.OfType<FinShiftTravail>().FirstOrDefault(s => s.ShiftTravailId == shiftTravailId);

            if (finshiftTravail is null)
            {
                return RedirectToAction("Error", "Home");
            }
            ShiftTravailViewModel shiftTravailViewModel = new();

            shiftTravailViewModel.AgentList = _db.Agents.ToList().Select(a => new SelectListItem
            {
                Text = $"{a.NomAgent} | Prénom : {a.PrenomAgent} | Sexe : {a.SexeAgent} | Nationalité : {a.Nationalite_Agent}",
                Value = a.AgentId.ToString()
            });

            shiftTravailViewModel.PosteTravailList = _db.PosteTravails.ToList().Select(p => new SelectListItem
            {
                Text = $"{p.TitrePosteTravail}",
                Value = p.PosteTravailId.ToString()
            }).ToList();
            shiftTravailViewModel.FinShiftTravail = finshiftTravail;

            return View(shiftTravailViewModel);
        }

        [HttpPost]
        public IActionResult Terminer(ShiftTravailViewModel shiftTravailViewModel)
        {
            //ShiftTravail exist = _db.ShiftTravails.Where(a=>a.NomAgent== agent.NomAgent && a.PrenomAgent == agent.PrenomAgent).FirstOrDefault(a => a.AgentId != agent.AgentId);
            FinShiftTravail finShift = _db.FinShiftTravails.FirstOrDefault(s => s.ShiftTravailId == shiftTravailViewModel.FinShiftTravail.ShiftTravailId);
            if (finShift is null)
            {
                return RedirectToAction("Error", "Home");
            }

            ModelState.Remove("FinShiftTravail.DateDebutShift");
            ModelState.Remove("FinShiftTravail.HeureDebutShift");
            ModelState.Remove("FinShiftTravail.ObservationDebutShift");

            
            finShift.DateFinShift = shiftTravailViewModel.FinShiftTravail.DateFinShift;
            finShift.HeureFinShift = shiftTravailViewModel.FinShiftTravail.HeureFinShift ;
            finShift.ObservationFinShift = shiftTravailViewModel.FinShiftTravail.ObservationFinShift;

            DateTime? dtfinshift = finShift.DateFinShift;
            if (finShift.DateFinShift is null)
            {
                ModelState.AddModelError("FinShiftTravail.DateFinShift", "Date fin de fin shift requis.");
            }
            if (finShift.HeureFinShift is null)
            {
                ModelState.AddModelError("FinShiftTravail.HeureFinShift", "Heure de fin shift requis.");
            }
            if (finShift.ObservationFinShift is null)
            {
                ModelState.AddModelError("FinShiftTravail.ObservationFinShift", "Observation de fin shift requis.");
            }

            if (finShift.HeureFinShift is null || finShift.HeureFinShift.Value.Hour == 0)
            {
                ModelState.AddModelError("FinShiftTravail.HeureFinShift", "Les heures de fin de fin de shift doivent être comprisent entre 1h et 23h.");
            }
            if (finShift.DateDebutShift == finShift.DateFinShift)
            {
                //Verification heure debut doit être < à heure de fin si la date de debut et de fin shift sont égaux

                if (finShift.HeureDebutShift > finShift.HeureFinShift)
                {
                    ModelState.AddModelError("FinShiftTravail.HeureFinShift", "L\' heure de fin de shift ne peut pas être inférieure à l\' heure de debut de shift.");
                }
            }

            if (ModelState.IsValid)
            {
                ValidationDate(dtfinshift, "FinShiftTravail.DateFinShift",
                             "La date de fin du shift doit être la date du jour.");
               
                if (ModelState.IsValid)
                {

                    _db.FinShiftTravails.Update(finShift);
                    _db.SaveChanges();
                    TempData["success"] = "Shift Travail terminé.";
                    return RedirectToAction("Index");
                }
            }

            shiftTravailViewModel.AgentList = _db.Agents.ToList().Select(a => new SelectListItem
            {
                Text = $"{a.NomAgent} | Prénom : {a.PrenomAgent} | Sexe : {a.SexeAgent} | Nationalité : {a.Nationalite_Agent}",
                Value = a.AgentId.ToString()
            });

            shiftTravailViewModel.PosteTravailList = _db.PosteTravails.ToList().Select(m => new SelectListItem
            {
                Text = $"{m.TitrePosteTravail}",
                Value = m.PosteTravailId.ToString()
            }).ToList();
            shiftTravailViewModel.FinShiftTravail = finShift;

            return View(shiftTravailViewModel);

        }

        public IActionResult Details(int shiftTravailId)
        {
            FinShiftTravail finshiftTravail = _db.ShiftTravails.OfType<FinShiftTravail>().FirstOrDefault(s => s.ShiftTravailId == shiftTravailId);

            if (finshiftTravail is null)
            {
                return RedirectToAction("Error", "Home");
            }
            ShiftTravailViewModel shiftTravailViewModel = new();

            shiftTravailViewModel.AgentList = _db.Agents.ToList().Select(a => new SelectListItem
            {
                Text = $"{a.NomAgent} | Prénom : {a.PrenomAgent} | Sexe : {a.SexeAgent} | Nationalité : {a.Nationalite_Agent}",
                Value = a.AgentId.ToString()
            });

            //shiftTravailViewModel.PosteTravailList = _db.PosteTravails.Where(p => p.PosteTravailId == finshiftTravail.PosteTravailId).ToList().Select(p => new SelectListItem
            //{
            //    Text = $"{p.TitrePosteTravail.ToString()}",
            //    Value = p.PosteTravailId.ToString()
            //}).ToList();

            shiftTravailViewModel.PosteTravailList = _db.PosteTravails.ToList().Select(m => new SelectListItem
            {
                Text = $"{m.TitrePosteTravail}",
                Value = m.PosteTravailId.ToString()
            }).ToList();
            shiftTravailViewModel.FinShiftTravail = finshiftTravail;
            return View(shiftTravailViewModel);
        }
    }
}
