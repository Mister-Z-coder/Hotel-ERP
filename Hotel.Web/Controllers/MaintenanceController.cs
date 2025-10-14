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
    [Authorize(Roles = "ADMIN,SUPERVISEUR")]
    [CaisseRedirectFilter]
    public class MaintenanceController : Controller
    {
        private readonly ApplicationDbContext _db;
        
        public MaintenanceController(ApplicationDbContext db)
        {
            _db = db;
        }

        public IActionResult Index(int page = 1, int size = 20, string searchDate="", string searchAgent = "", string searchChambre = "")
        {

            DateTime parsedDate;
            bool isDateValid = DateTime.TryParse(searchDate, out parsedDate);
            int totalmaintenanceChambre = _db.MaintenanceChambres.OfType<FinMaintenanceChambre>().Count();
            int skipValue = (page - 1) * size;
            int totalPages = (int)Math.Ceiling((double)totalmaintenanceChambre / size);
            var finMaintenance = _db.MaintenanceChambres.OfType<FinMaintenanceChambre>()
                .OrderByDescending(f => f.MaintenanceChambreId)               // Trier par date de fin maintenance
                .ThenBy(f=>f.Agent.NomAgent)
                .Include(f=>f.Agent)
                .Include(f=>f.Chambre)
                .Where(f =>
                           (string.IsNullOrEmpty(searchChambre) || f.Chambre.NumeroChambre!.ToString().Contains(searchChambre.ToString())) &&
                           (string.IsNullOrEmpty(searchDate) || f.DateDebutMaintenance! == Convert.ToDateTime(parsedDate.Date)) &&
                           (string.IsNullOrEmpty(searchAgent) ||f.Agent.NomAgent!.ToUpper().Contains(searchAgent.ToUpper()) || f.Agent.PrenomAgent!.ToUpper().Contains(searchAgent.ToUpper()))
                           ).ToList().Skip(skipValue).Take(size);
            var depart = (totalPages - 1) * skipValue + 1;
            ViewBag.NumAff = depart;
            ViewBag.CurrentPage = page;
            ViewBag.PageSize = size;
            ViewBag.TotalPage = totalPages;
            MaintenanceChambreViewModel maintenanceChambreViewModel = new()
            {
                FinMaintenanceChambres = finMaintenance
            };
            return View(maintenanceChambreViewModel);
        }
        public IActionResult Create()
        {
            MaintenanceChambreViewModel maintenanceChambreViewModel = new()
            {
                AgentList = _db.Agents.ToList().Select(a => new SelectListItem
                {
                    Text = $"{a.NomAgent} | Prénom : {a.PrenomAgent} | Sexe : {a.SexeAgent} | Nationalité : {a.Nationalite_Agent}",
                    Value = a.AgentId.ToString()
                }),
                //
                ChambreList = _db.Chambres.ToList().Select(c => new SelectListItem
                {
                    Text = $"{c.NumeroChambre.ToString()} | Type : {c.TypeChambre} | Statut : {c.StatutChambre} | Prix-heure : {c.PrixHeure} | Prix-nuit : {c.PrixNuit} | Capacité : {c.CapaciteMaxChambre}",
                    Value = c.ChambreId.ToString()
                }).ToList()
        };
            return View(maintenanceChambreViewModel);
        }
        private void ValidationDate(DateTime? date, string fieldName, string errorMessage)
        {
            if (date != DateTime.Today)
            {
                ModelState.AddModelError(fieldName, errorMessage);
            }
        }

        [HttpPost]
        public IActionResult Create(MaintenanceChambreViewModel maintenanceChambreViewModel)
        {
            //MaintenanceChambre exist = _db.MaintenanceChambres.Where(a=>a.NomAgent== agent.NomAgent && a.PrenomAgent == agent.PrenomAgent).FirstOrDefault(a => a.AgentId != agent.AgentId);
           
            ModelState.Remove("FinMaintenanceChambre.DateFinMaintenance");
            ModelState.Remove("FinMaintenanceChambre.HeureFinMaintenance");
            ModelState.Remove("FinMaintenanceChambre.ObservationFinMaintenance");
            //maintenanceChambreViewModel.FinMaintenanceChambre.DateDebutMaintenance = maintenanceChambreViewModel.MaintenanceChambre.DateDebutMaintenance;
            //maintenanceChambreViewModel.FinMaintenanceChambre.HeureDebutMaintenance = maintenanceChambreViewModel.MaintenanceChambre.HeureDebutMaintenance;
            //maintenanceChambreViewModel.FinMaintenanceChambre.ObservationDebutMaintenance = maintenanceChambreViewModel.MaintenanceChambre.ObservationDebutMaintenance;
            DateTime dtdebutmaintenance = maintenanceChambreViewModel.MaintenanceChambre.DateDebutMaintenance.Value;
            bool IsMultimaintenance = maintenanceChambreViewModel.IsMultimaintenance;
            if (maintenanceChambreViewModel.MaintenanceChambre.DateDebutMaintenance is null)
            {
                ModelState.AddModelError("MaintenanceChambre.DateDebutMaintenance", "Date de debut de maintenance requis.");
            }
            if (maintenanceChambreViewModel.MaintenanceChambre.HeureDebutMaintenance is null)
            {
                ModelState.AddModelError("MaintenanceChambre.HeureDebutMaintenance", "Heure de fin de maintenance requis.");
            }
            if (maintenanceChambreViewModel.MaintenanceChambre.ObservationDebutMaintenance is null)
            {
                ModelState.AddModelError("MaintenanceChambre.ObservationDebutMaintenance", "Observation de debut de maintenance requis.");
            }
            if (maintenanceChambreViewModel.MaintenanceChambre.HeureDebutMaintenance.Value.Hour == 0)
            {
                ModelState.AddModelError("MaintenanceChambre.HeureDebutMaintenance", "Les heures de debut de maintenance doivent être comprisent entre 1h et 23h.");
            }

            if (ModelState.IsValid)
            {
                ValidationDate(dtdebutmaintenance, "MaintenanceChambre.DateDebutMaintenance",
                             "La date de début du maintenance doit être la date du jour.");
                
                if (ModelState.IsValid)
                {
                    if(IsMultimaintenance is false)
                    {
                        DateTime? defautDate = null;
                        FinMaintenanceChambre agentdejaenmainteannce = _db.MaintenanceChambres.OfType<FinMaintenanceChambre>().Where(s => s.DateFinMaintenance == defautDate).FirstOrDefault(s => s.AgentId == maintenanceChambreViewModel.MaintenanceChambre.AgentId);
                        if (agentdejaenmainteannce is not null)
                        {
                            TempData["error"] = "Cet agent est déjà en mainteance, veuillez d'abord notifier la fin de sa maintenance encours.";
                            return RedirectToAction("Index");
                        }
                        FinMaintenanceChambre finMaintenanceChambre = new FinMaintenanceChambre()
                        {
                            AgentId = maintenanceChambreViewModel.MaintenanceChambre.AgentId,
                            ChambreId = maintenanceChambreViewModel.MaintenanceChambre.ChambreId,
                            DateDebutMaintenance = maintenanceChambreViewModel.MaintenanceChambre.DateDebutMaintenance,
                            HeureDebutMaintenance = maintenanceChambreViewModel.MaintenanceChambre.HeureDebutMaintenance,
                            ObservationDebutMaintenance = maintenanceChambreViewModel.MaintenanceChambre.ObservationDebutMaintenance,
                            DateFinMaintenance = null,
                            HeureFinMaintenance = null
                        };

                        _db.FinMaintenanceChambres.Add(finMaintenanceChambre);
                        _db.SaveChanges();

                        var chambre = _db.Chambres.FirstOrDefault(c => c.ChambreId == finMaintenanceChambre.ChambreId);
                        if(chambre.StatutChambre.Contains("Libre"))
                        {
                            chambre.StatutChambre = "Libre|Nettoyage";

                        }else if(chambre.StatutChambre.Contains("Occupée"))
                        {
                            chambre.StatutChambre = "Occupée|Nettoyage";
                        }
                        _db.Chambres.Update(chambre);
                        _db.SaveChanges();

                        TempData["success"] = "Maintenance Chambre crée.";
                        return RedirectToAction("Index");
                    }
                    else
                    {
                        FinMaintenanceChambre finMaintenanceChambre = new FinMaintenanceChambre()
                        {
                            AgentId = maintenanceChambreViewModel.MaintenanceChambre.AgentId,
                            ChambreId = maintenanceChambreViewModel.MaintenanceChambre.ChambreId,
                            DateDebutMaintenance = maintenanceChambreViewModel.MaintenanceChambre.DateDebutMaintenance,
                            HeureDebutMaintenance = maintenanceChambreViewModel.MaintenanceChambre.HeureDebutMaintenance,
                            ObservationDebutMaintenance = maintenanceChambreViewModel.MaintenanceChambre.ObservationDebutMaintenance,
                            DateFinMaintenance = null,
                            HeureFinMaintenance = null
                        };

                        _db.FinMaintenanceChambres.Add(finMaintenanceChambre);
                        _db.SaveChanges();
                        var chambre = _db.Chambres.FirstOrDefault(c => c.ChambreId == finMaintenanceChambre.ChambreId);
                        if (chambre.StatutChambre.Contains("Libre"))
                        {
                            chambre.StatutChambre = "Libre|Nettoyage";

                        }
                        else if (chambre.StatutChambre.Contains("Occupée"))
                        {
                            chambre.StatutChambre = "Occupée|Nettoyage";
                        }
                        _db.Chambres.Update(chambre);
                        _db.SaveChanges();
                        TempData["success"] = "Maintenance Chambre crée.";
                        return RedirectToAction("Index");
                    }
                    
                }
            }

            maintenanceChambreViewModel.AgentList = _db.Agents.ToList().Select(a => new SelectListItem
            {
                Text = $"{a.NomAgent} | Prénom : {a.PrenomAgent} | Sexe : {a.SexeAgent} | Nationalité : {a.Nationalite_Agent}",
                Value = a.AgentId.ToString()
            });

            maintenanceChambreViewModel.ChambreList = _db.Chambres.ToList().Select(c => new SelectListItem
            {
                Text = $"{c.NumeroChambre.ToString()} | Type : {c.TypeChambre} | Statut : {c.StatutChambre} | Prix-heure : {c.PrixHeure} | Prix-nuit : {c.PrixNuit} | Capacité : {c.CapaciteMaxChambre}",
                Value = c.ChambreId.ToString()
            }).ToList();
            return View(maintenanceChambreViewModel);
        }

        public IActionResult UpdateMaintenanceEncours(int maintenancechambreId)
        {
            MaintenanceChambre maintenancechambre = _db.MaintenanceChambres.FirstOrDefault(s => s.MaintenanceChambreId == maintenancechambreId);

            if (maintenancechambre is null)
            {
                return RedirectToAction("Error", "Home");
            }
            MaintenanceChambreViewModel maintenanceChambreViewModel = new();

            maintenanceChambreViewModel.AgentList = _db.Agents.ToList().Select(a => new SelectListItem
            {
                Text = $"{a.NomAgent} | Prénom : {a.PrenomAgent} | Sexe : {a.SexeAgent} | Nationalité : {a.Nationalite_Agent}",
                Value = a.AgentId.ToString()
            });

            //maintenanceChambreViewModel.PosteTravailList = _db.PosteTravails.Where(p => p.ChambreId != maintenancechambre.ChambreId).ToList().Select(p => new SelectListItem
            //{
            //    Text = $"{p.TitrePosteTravail.ToString()}",
            //    Value = p.ChambreId.ToString()
            //}).ToList();
            maintenanceChambreViewModel.ChambreList = _db.Chambres.ToList().Select(c => new SelectListItem
            {
                Text = $"{c.NumeroChambre.ToString()} | Type : {c.TypeChambre} | Statut : {c.StatutChambre} | Prix-heure : {c.PrixHeure} | Prix-nuit : {c.PrixNuit} | Capacité : {c.CapaciteMaxChambre}",
                Value = c.ChambreId.ToString()
            }).ToList();

            maintenanceChambreViewModel.MaintenanceChambre = maintenancechambre;
            return View(maintenanceChambreViewModel);
        }
        
        [HttpPost]
        public IActionResult UpdateMaintenanceEncours(MaintenanceChambreViewModel maintenanceChambreViewModel)
        {
            
            ModelState.Remove("FinMaintenanceChambre.DateFinMaintenance");
            ModelState.Remove("FinMaintenanceChambre.HeureFinMaintenance");
            ModelState.Remove("FinMaintenanceChambre.ObservationFinMaintenance");
            //maintenanceChambreViewModel.FinMaintenanceChambre.DateDebutMaintenance = maintenanceChambreViewModel.MaintenanceChambre.DateDebutMaintenance;
            //maintenanceChambreViewModel.FinMaintenanceChambre.HeureDebutMaintenance = maintenanceChambreViewModel.MaintenanceChambre.HeureDebutMaintenance;
            //maintenanceChambreViewModel.FinMaintenanceChambre.ObservationDebutMaintenance = maintenanceChambreViewModel.MaintenanceChambre.ObservationDebutMaintenance;
            //MaintenanceChambre exist = _db.MaintenanceChambres.Where(a=>a.NomAgent== agent.NomAgent && a.PrenomAgent == agent.PrenomAgent).FirstOrDefault(a => a.AgentId != agent.AgentId);
            MaintenanceChambre maintenance = _db.MaintenanceChambres.FirstOrDefault(s => s.MaintenanceChambreId == maintenanceChambreViewModel.MaintenanceChambre.MaintenanceChambreId);
            if (maintenance is null)
            {
                return RedirectToAction("Error", "Home");
            }

            maintenance.AgentId = maintenanceChambreViewModel.MaintenanceChambre.AgentId;
            maintenance.ChambreId = maintenanceChambreViewModel.MaintenanceChambre.ChambreId;
            maintenance.DateDebutMaintenance = maintenanceChambreViewModel.MaintenanceChambre.DateDebutMaintenance;
            maintenance.HeureDebutMaintenance = maintenanceChambreViewModel.MaintenanceChambre.HeureDebutMaintenance;
            maintenance.ObservationDebutMaintenance = maintenanceChambreViewModel.MaintenanceChambre.ObservationDebutMaintenance;

            DateTime dtdebutmaintenance = maintenanceChambreViewModel.MaintenanceChambre.DateDebutMaintenance.Value;
            if (maintenanceChambreViewModel.MaintenanceChambre.DateDebutMaintenance is null)
            {
                ModelState.AddModelError("MaintenanceChambre.DateDebutMaintenance", "Date de debut de maintenance requis.");
            }
            if (maintenanceChambreViewModel.MaintenanceChambre.HeureDebutMaintenance is null)
            {
                ModelState.AddModelError("MaintenanceChambre.HeureDebutMaintenance", "Heure de fin de maintenance requis.");
            }
            if (maintenanceChambreViewModel.MaintenanceChambre.ObservationDebutMaintenance is null)
            {
                ModelState.AddModelError("MaintenanceChambre.ObservationDebutMaintenance", "Observation de debut de maintenance requis.");
            }
            if (maintenanceChambreViewModel.MaintenanceChambre.HeureDebutMaintenance.Value.Hour == 0)
            {
                ModelState.AddModelError("MaintenanceChambre.HeureDebutMaintenance", "Les heures de debut de maintenance doivent être comprisent entre 1h et 23h.");
            }
            if (ModelState.IsValid)
            {
                //ValidationDate(dtdebutmaintenance, "MaintenanceChambre.DateDebutMaintenance",
                //             "La date de début du maintenance doit être la date du jour.");

                if (ModelState.IsValid)
                {
                    _db.MaintenanceChambres.Update(maintenance);
                    _db.SaveChanges();

                    TempData["success"] = "Maintenance Chambre mise à jour.";
                    return RedirectToAction("Index");
                }
            }

            maintenanceChambreViewModel.AgentList = _db.Agents.ToList().Select(a => new SelectListItem
            {
                Text = $"{a.NomAgent} | Prénom : {a.PrenomAgent} | Sexe : {a.SexeAgent} | Nationalité : {a.Nationalite_Agent}",
                Value = a.AgentId.ToString()
            });

            maintenanceChambreViewModel.ChambreList = _db.Chambres.ToList().Select(c => new SelectListItem
            {
                Text = $"{c.NumeroChambre.ToString()} | Type : {c.TypeChambre} | Statut : {c.StatutChambre} | Prix-heure : {c.PrixHeure} | Prix-nuit : {c.PrixNuit} | Capacité : {c.CapaciteMaxChambre}",
                Value = c.ChambreId.ToString()
            }).ToList();
            return View(maintenanceChambreViewModel);
        }
        public IActionResult UpdateMaintenanceTermine(int maintenancechambreId)
        {
            FinMaintenanceChambre finmaintenancechambre = _db.MaintenanceChambres.OfType<FinMaintenanceChambre>().FirstOrDefault(s => s.MaintenanceChambreId == maintenancechambreId);

            if (finmaintenancechambre is null)
            {
                return RedirectToAction("Error", "Home");
            }
            MaintenanceChambreViewModel maintenanceChambreViewModel = new();

            maintenanceChambreViewModel.AgentList = _db.Agents.ToList().Select(a => new SelectListItem
            {
                Text = $"{a.NomAgent} | Prénom : {a.PrenomAgent} | Sexe : {a.SexeAgent} | Nationalité : {a.Nationalite_Agent}",
                Value = a.AgentId.ToString()
            });

            maintenanceChambreViewModel.ChambreList = _db.Chambres.ToList().Select(c => new SelectListItem
            {
                Text = $"{c.NumeroChambre.ToString()} | Type : {c.TypeChambre} | Statut : {c.StatutChambre} | Prix-heure : {c.PrixHeure} | Prix-nuit : {c.PrixNuit} | Capacité : {c.CapaciteMaxChambre}",
                Value = c.ChambreId.ToString()
            }).ToList();
            maintenanceChambreViewModel.FinMaintenanceChambre = finmaintenancechambre;

            return View(maintenanceChambreViewModel);
        }

        [HttpPost]
        public IActionResult UpdateMaintenanceTermine(MaintenanceChambreViewModel maintenanceChambreViewModel)
        {
            ModelState.Remove("FinMaintenanceChambre.DateDebutMaintenance");
            ModelState.Remove("FinMaintenanceChambre.HeureDebutMaintenance");
            ModelState.Remove("FinMaintenanceChambre.DateFinMaintenance");
            ModelState.Remove("FinMaintenanceChambre.HeureFinMaintenance");
            ModelState.Remove("FinMaintenanceChambre.ObservationDebutMaintenance");
            ModelState.Remove("FinMaintenanceChambre.AgentId");
            ModelState.Remove("FinMaintenanceChambre.ChambreId");

            //MaintenanceChambre exist = _db.MaintenanceChambres.Where(a=>a.NomAgent== agent.NomAgent && a.PrenomAgent == agent.PrenomAgent).FirstOrDefault(a => a.AgentId != agent.AgentId);
            FinMaintenanceChambre finMaintenanceChambre = _db.FinMaintenanceChambres.FirstOrDefault(s => s.MaintenanceChambreId == maintenanceChambreViewModel.FinMaintenanceChambre.MaintenanceChambreId);
            if (finMaintenanceChambre is null)
            {
                return RedirectToAction("Error", "Home");
            }

            //finMaintenanceChambre.DateFinMaintenance = maintenanceChambreViewModel.FinMaintenanceChambre.DateFinMaintenance;
            //finMaintenanceChambre.HeureFinMaintenance = maintenanceChambreViewModel.FinMaintenanceChambre.HeureFinMaintenance;
            finMaintenanceChambre.ObservationFinMaintenance = maintenanceChambreViewModel.FinMaintenanceChambre.ObservationFinMaintenance;

            DateTime? dtfinmaintenance = finMaintenanceChambre.DateFinMaintenance;

            

            if (ModelState.IsValid)
            {
                if (ModelState.IsValid)
                {

                    _db.FinMaintenanceChambres.Update(finMaintenanceChambre);
                    _db.SaveChanges();
                    TempData["success"] = "Maintenance Chambre mise à jour.";
                    return RedirectToAction("Index");
                }
            }

            maintenanceChambreViewModel.AgentList = _db.Agents.ToList().Select(a => new SelectListItem
            {
                Text = $"{a.NomAgent} | Prénom : {a.PrenomAgent} | Sexe : {a.SexeAgent} | Nationalité : {a.Nationalite_Agent}",
                Value = a.AgentId.ToString()
            });

            maintenanceChambreViewModel.ChambreList = _db.Chambres.ToList().Select(c => new SelectListItem
            {
                Text = $"{c.NumeroChambre.ToString()} | Type : {c.TypeChambre} | Statut : {c.StatutChambre} | Prix-heure : {c.PrixHeure} | Prix-nuit : {c.PrixNuit} | Capacité : {c.CapaciteMaxChambre}",
                Value = c.ChambreId.ToString()
            }).ToList();
            maintenanceChambreViewModel.FinMaintenanceChambre = finMaintenanceChambre;

            return View(maintenanceChambreViewModel);

        }
        public IActionResult Terminer(int maintenancechambreId)
        {
            FinMaintenanceChambre finmaintenancechambre = _db.MaintenanceChambres.OfType<FinMaintenanceChambre>().FirstOrDefault(s => s.MaintenanceChambreId == maintenancechambreId && s.ObservationFinMaintenance == null);

            if (finmaintenancechambre is null)
            {
                return RedirectToAction("Error", "Home");
            }
            MaintenanceChambreViewModel maintenanceChambreViewModel = new();

            maintenanceChambreViewModel.AgentList = _db.Agents.ToList().Select(a => new SelectListItem
            {
                Text = $"{a.NomAgent} | Prénom : {a.PrenomAgent} | Sexe : {a.SexeAgent} | Nationalité : {a.Nationalite_Agent}",
                Value = a.AgentId.ToString()
            });

            maintenanceChambreViewModel.ChambreList = _db.Chambres.ToList().Select(c => new SelectListItem
            {
                Text = $"{c.NumeroChambre.ToString()} | Type : {c.TypeChambre} | Statut : {c.StatutChambre} | Prix-heure : {c.PrixHeure} | Prix-nuit : {c.PrixNuit} | Capacité : {c.CapaciteMaxChambre}",
                Value = c.ChambreId.ToString()
            }).ToList();
            maintenanceChambreViewModel.FinMaintenanceChambre = finmaintenancechambre;

            return View(maintenanceChambreViewModel);
        }

        [HttpPost]
        public IActionResult Terminer(MaintenanceChambreViewModel maintenanceChambreViewModel)
        {
            //MaintenanceChambre exist = _db.MaintenanceChambres.Where(a=>a.NomAgent== agent.NomAgent && a.PrenomAgent == agent.PrenomAgent).FirstOrDefault(a => a.AgentId != agent.AgentId);
            FinMaintenanceChambre finMaintenanceChambre = _db.FinMaintenanceChambres.FirstOrDefault(s => s.MaintenanceChambreId == maintenanceChambreViewModel.FinMaintenanceChambre.MaintenanceChambreId);
            if (finMaintenanceChambre is null)
            {
                return RedirectToAction("Error", "Home");
            }

            ModelState.Remove("FinMaintenanceChambre.DateDebutMaintenance");
            ModelState.Remove("FinMaintenanceChambre.HeureDebutMaintenance");
            ModelState.Remove("FinMaintenanceChambre.ObservationDebutMaintenance");

            
            finMaintenanceChambre.DateFinMaintenance = maintenanceChambreViewModel.FinMaintenanceChambre.DateFinMaintenance;
            finMaintenanceChambre.HeureFinMaintenance = maintenanceChambreViewModel.FinMaintenanceChambre.HeureFinMaintenance ;
            finMaintenanceChambre.ObservationFinMaintenance = maintenanceChambreViewModel.FinMaintenanceChambre.ObservationFinMaintenance;

            DateTime? dtfinmaintenance = finMaintenanceChambre.DateFinMaintenance;
            if (finMaintenanceChambre.DateFinMaintenance is null)
            {
                ModelState.AddModelError("FinMaintenanceChambre.DateFinMaintenance", "Date de fin maintenance requis.");
            }
            if (finMaintenanceChambre.HeureFinMaintenance is null)
            {
                ModelState.AddModelError("FinMaintenanceChambre.HeureFinMaintenance", "Heure de fin maintenance requis.");
            }
            if (finMaintenanceChambre.ObservationFinMaintenance is null)
            {
                ModelState.AddModelError("FinMaintenanceChambre.ObservationFinMaintenance", "Observation de fin maintenance requis.");
            }

            if (finMaintenanceChambre.HeureFinMaintenance is null || finMaintenanceChambre.HeureFinMaintenance.Value.Hour == 0)
            {
                ModelState.AddModelError("FinMaintenanceChambre.HeureFinMaintenance", "Les heures de fin de maintenance doivent être comprisent entre 1h et 23h.");
            }
            if (finMaintenanceChambre.DateDebutMaintenance == finMaintenanceChambre.DateFinMaintenance)
            {
                //Verification heure debut doit être < à heure de fin si la date de debut et de fin maintenance sont égaux

                if (finMaintenanceChambre.HeureDebutMaintenance > finMaintenanceChambre.HeureFinMaintenance)
                {
                    ModelState.AddModelError("FinMaintenanceChambre.HeureFinMaintenance", "L\' heure de fin de maintenance ne peut pas être inférieure à l\' heure de debut de maintenance.");
                }
            }

            if (ModelState.IsValid)
            {
                ValidationDate(dtfinmaintenance, "FinMaintenanceChambre.DateFinMaintenance",
                             "La date de fin dela maintenance doit être la date du jour.");
               
                if (ModelState.IsValid)
                {

                    _db.FinMaintenanceChambres.Update(finMaintenanceChambre);
                    _db.SaveChanges();
                    var chambre = _db.Chambres.FirstOrDefault(c => c.ChambreId == finMaintenanceChambre.ChambreId);
                    if (chambre.StatutChambre.Contains("Libre"))
                    {
                        chambre.StatutChambre = "Libre|Propre";

                    }
                    else if (chambre.StatutChambre.Contains("Occupée"))
                    {
                        chambre.StatutChambre = "Occupée|Propre";
                    }
                    _db.Chambres.Update(chambre);
                    _db.SaveChanges();
                    TempData["success"] = "Maintenance Chambre terminée.";
                    return RedirectToAction("Index");
                }
            }

            maintenanceChambreViewModel.AgentList = _db.Agents.ToList().Select(a => new SelectListItem
            {
                Text = $"{a.NomAgent} | Prénom : {a.PrenomAgent} | Sexe : {a.SexeAgent} | Nationalité : {a.Nationalite_Agent}",
                Value = a.AgentId.ToString()
            });

            maintenanceChambreViewModel.ChambreList = _db.Chambres.ToList().Select(c => new SelectListItem
            {
                Text = $"{c.NumeroChambre.ToString()} | Type : {c.TypeChambre} | Statut : {c.StatutChambre} | Prix-heure : {c.PrixHeure} | Prix-nuit : {c.PrixNuit} | Capacité : {c.CapaciteMaxChambre}",
                Value = c.ChambreId.ToString()
            }).ToList();
            maintenanceChambreViewModel.FinMaintenanceChambre = finMaintenanceChambre;

            return View(maintenanceChambreViewModel);

        }

        public IActionResult Details(int maintenancechambreId)
        {
            FinMaintenanceChambre finmaintenancechambre = _db.MaintenanceChambres.OfType<FinMaintenanceChambre>().FirstOrDefault(s => s.MaintenanceChambreId == maintenancechambreId);

            if (finmaintenancechambre is null)
            {
                return RedirectToAction("Error", "Home");
            }
            MaintenanceChambreViewModel maintenanceChambreViewModel = new();

            maintenanceChambreViewModel.AgentList = _db.Agents.ToList().Select(a => new SelectListItem
            {
                Text = $"{a.NomAgent} | Prénom : {a.PrenomAgent} | Sexe : {a.SexeAgent} | Nationalité : {a.Nationalite_Agent}",
                Value = a.AgentId.ToString()
            });

            //maintenanceChambreViewModel.PosteTravailList = _db.PosteTravails.Where(p => p.ChambreId == finmaintenancechambre.ChambreId).ToList().Select(p => new SelectListItem
            //{
            //    Text = $"{p.TitrePosteTravail.ToString()}",
            //    Value = p.ChambreId.ToString()
            //}).ToList();

            maintenanceChambreViewModel.ChambreList = _db.Chambres.ToList().Select(c => new SelectListItem
            {
                Text = $"{c.NumeroChambre.ToString()} | Type : {c.TypeChambre} | Statut : {c.StatutChambre} | Prix-heure : {c.PrixHeure} | Prix-nuit : {c.PrixNuit} | Capacité : {c.CapaciteMaxChambre}",
                Value = c.ChambreId.ToString()
            }).ToList();
            maintenanceChambreViewModel.FinMaintenanceChambre = finmaintenancechambre;
            return View(maintenanceChambreViewModel);
        }
    }
}
