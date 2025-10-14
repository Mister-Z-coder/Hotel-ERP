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
    [Authorize(Roles = "ADMIN,GESTIONNAIRE,BARMAN,CUISINIER")]
    [CaisseRedirectFilter]
    public class DemandeController : Controller
    {
        private readonly ApplicationDbContext _db;

        public DemandeController(ApplicationDbContext db)
        {
            _db = db;
        }

        public IActionResult Index(int page = 1, int size = 20, string searchType = "", string searchDate = "", string searchEtat = "", string searchAgent = "")
        {
            DateTime parsedDate;
            bool isDateValid = DateTime.TryParse(searchDate, out parsedDate);
            int totaldemandes = _db.Demandes.Count();
            int skipValue = (page - 1) * size;
            int totalPages = (int)Math.Ceiling((double)totaldemandes / size);
            //var demandes = _db.Demandes.ToList().Skip(skipValue).Take(size);

            var demandes = (from dmd in _db.Demandes
                            join appro in _db.Approvisionnements on dmd.DemandeId equals appro.DemandeId into approGroup
                            from appro in approGroup.DefaultIfEmpty() // Jointure externe gauche pour Approvisionnements
                            join ajout in _db.Ajouts on dmd.DemandeId equals ajout.DemandeId into ajoutGroup
                            from ajout in ajoutGroup.DefaultIfEmpty() // Jointure externe gauche pour Ajouts
                            orderby dmd.DemandeId descending
                            select new Demande
                            {
                                DemandeId = dmd.DemandeId,
                                AgentId = dmd.AgentId,
                                Agent = dmd.Agent,
                                DateDemande = dmd.DateDemande,
                                TypeDemande = dmd.TypeDemande,
                                EtatDemande = appro != null ? appro.EtatDemandeAppro : (ajout != null ? ajout.EtatDemandeAjout : null) // Vérifiez les deux états
                            }).Where(d =>
                            (string.IsNullOrEmpty(searchType) || d.TypeDemande!.ToUpper().Contains(searchType.ToUpper())) &&
                            (string.IsNullOrEmpty(searchDate) || d.DateDemande! == Convert.ToDateTime(parsedDate.Date)) &&
                            (string.IsNullOrEmpty(searchAgent) || d.Agent.NomAgent!.ToUpper().Contains(searchAgent.ToUpper()) || d.Agent.PrenomAgent!.ToUpper().Contains(searchAgent.ToUpper())) &&
                            (string.IsNullOrEmpty(searchEtat) || d.EtatDemande!.ToUpper().Contains(searchEtat.ToUpper())))
                               .ToList().Skip(skipValue).Take(size);

            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            int userId = userIdClaim != null ? int.Parse(userIdClaim.Value) : 0;
            Agent agent = _db.Agents.FirstOrDefault(a => a.AgentId == userId);
            if ((agent.Fonction_Agent == "CUISINIER") || (agent.Fonction_Agent == "BARMAN"))
            {
                totaldemandes = _db.Demandes.Where(d=>d.TypeDemande!.ToUpper() == "AJOUT").Count();
                totalPages = (int)Math.Ceiling((double)totaldemandes / size);
                demandes = (from dmd in _db.Demandes
                                join appro in _db.Approvisionnements on dmd.DemandeId equals appro.DemandeId into approGroup
                                from appro in approGroup.DefaultIfEmpty() // Jointure externe gauche pour Approvisionnements
                                join ajout in _db.Ajouts on dmd.DemandeId equals ajout.DemandeId into ajoutGroup
                                from ajout in ajoutGroup.DefaultIfEmpty() // Jointure externe gauche pour Ajouts
                                orderby dmd.DemandeId descending
                                select new Demande
                                {
                                    DemandeId = dmd.DemandeId,
                                    AgentId = dmd.AgentId,
                                    Agent = dmd.Agent,
                                    DateDemande = dmd.DateDemande,
                                    TypeDemande = dmd.TypeDemande,
                                    EtatDemande = appro != null ? appro.EtatDemandeAppro : (ajout != null ? ajout.EtatDemandeAjout : null) // Vérifiez les deux états
                                }).Where(d =>
                                (d.TypeDemande!.ToUpper() == "AJOUT") &&
                                (string.IsNullOrEmpty(searchType) || d.TypeDemande!.ToUpper().Contains(searchType.ToUpper())) &&
                                (string.IsNullOrEmpty(searchDate) || d.DateDemande! == Convert.ToDateTime(parsedDate.Date)) &&
                                (string.IsNullOrEmpty(searchAgent) || d.Agent.NomAgent!.ToUpper().Contains(searchAgent.ToUpper()) || d.Agent.PrenomAgent!.ToUpper().Contains(searchAgent.ToUpper())) &&
                                (string.IsNullOrEmpty(searchEtat) || d.EtatDemande!.ToUpper().Contains(searchEtat.ToUpper())))
                               .ToList().Skip(skipValue).Take(size);
            }
            

            
            var depart = (totalPages - 1) * skipValue + 1;
            ViewBag.NumAff = depart;
            ViewBag.CurrentPage = page;
            ViewBag.PageSize = size;
            ViewBag.TotalPage = totalPages;
            
            DemandeViewModel demandeViewModel = new()
            {
                Demande = demandes
            };
            return View(demandeViewModel);
        }

        public IActionResult Create()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            int userId = userIdClaim != null ? int.Parse(userIdClaim.Value) : 0;
            Agent agent = _db.Agents.FirstOrDefault(a => a.AgentId == userId);

            DemandeViewModel demandeViewModel = new()
            {
                AgentList = _db.Agents.ToList().Where(a => a.AgentId == userId).Select(a => new SelectListItem
                {
                    Text = $"{a.NomAgent} | Prénom : {a.PrenomAgent} | Sexe : {a.SexeAgent} | Nationalité : {a.Nationalite_Agent}",
                    Value = a.AgentId.ToString()
                }),

                Demandes = new Demande
                {
                    AgentId = userId
                }
            };
            return View(demandeViewModel);
        }
        String TrouverDonneesBydata(int donneId, string context)
        {
            string valeur = "";
            if (context == "Agent")
            {
                Agent agent = _db.Agents.FirstOrDefault(a => a.AgentId == donneId);
                valeur = agent.NomAgent + " " + agent.PrenomAgent;
            }
            else if (context == "Aliment")
            {
                Aliment aliment = _db.Aliments.FirstOrDefault(a => a.AlimentId == donneId);
                //valeur = client.NomClient;
                valeur = aliment.TitreAliment;
            }
            else if (context == "Boisson")
            {
                Boisson boisson = _db.Boissons.FirstOrDefault(b => b.BoissonId == donneId);
                valeur = boisson.TitreBoisson;
            }
            return valeur;
        }

        [HttpPost]
        public IActionResult Create(DemandeViewModel demandeViewModel)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            int userId = userIdClaim != null ? int.Parse(userIdClaim.Value) : 0;
            Agent agent = _db.Agents.FirstOrDefault(a => a.AgentId == userId);
            
            DateTime dtdemande = demandeViewModel.Demandes.DateDemande;
            if (ModelState.IsValid)
            {

                ValidationDate(dtdemande, "Demandes.DateDemande",
                             "La date de la demande ne peut pas être antérieure à la date du jour.");
                if (demandeViewModel.Demandes.TypeDemande != "AJOUT")
                {
                    if ((agent.Fonction_Agent == "CUISINIER") || (agent.Fonction_Agent == "BARMAN"))
                    {
                        ModelState.AddModelError("Demandes.TypeDemande", "Votre fonction ne vous permet pas d'emettre ce type de demande.");
                    }
                }

                if (ModelState.IsValid)
                {

                    if (demandeViewModel.Demandes.TypeDemande == "AJOUT")
                    {
                        Ajout ajout = new Ajout
                        {
                            AgentId = demandeViewModel.Demandes.AgentId,
                            DateDemande = demandeViewModel.Demandes.DateDemande,
                            TypeDemande = demandeViewModel.Demandes.TypeDemande,
                            EtatDemandeAjout = "Brouillon",
                        };
                        _db.Ajouts.Add(ajout);
                        _db.SaveChanges();

                        DetailsDemande detailsDemande = new DetailsDemande()
                        {
                            Agent = TrouverDonneesBydata(ajout.AgentId, "Agent").ToString(),
                            DemandeId = ajout.DemandeId,
                            DateDemande = ajout.DateDemande,
                            TypeDemande = ajout.TypeDemande,
                            EtatDemande = ajout.EtatDemandeAjout,
                            ProduitDemande = "NULL"
                        };
                        _db.DetailsDemandes.Add(detailsDemande);
                        _db.SaveChanges();
                    }
                    else if (demandeViewModel.Demandes.TypeDemande == "APPROVISIONNEMENT")
                    {

                        Approvisionnement approv = new Approvisionnement
                        {
                            AgentId = demandeViewModel.Demandes.AgentId,
                            DateDemande = demandeViewModel.Demandes.DateDemande,
                            TypeDemande = demandeViewModel.Demandes.TypeDemande,
                            EtatDemandeAppro = "Brouillon",
                        };

                        _db.Approvisionnements.Add(approv);
                        _db.SaveChanges();

                        DetailsDemande detailsDemande = new DetailsDemande()
                        {
                            Agent = TrouverDonneesBydata(approv.AgentId, "Agent").ToString(),
                            DemandeId = approv.DemandeId,
                            DateDemande = approv.DateDemande,
                            TypeDemande = approv.TypeDemande,
                            EtatDemande = approv.EtatDemandeAppro,
                            ProduitDemande = "NULL"
                        };
                        _db.DetailsDemandes.Add(detailsDemande);
                        _db.SaveChanges();
                    }
                    else
                    {
                        TempData["erreur"] = "Demmande non créee.";
                        return RedirectToAction("Index");
                    }
                    TempData["success"] = "Demmande créee.";
                    return RedirectToAction("Index");
                }
            }
            demandeViewModel.AgentList = _db.Agents.ToList().Where(a => a.AgentId == userId).Select(a => new SelectListItem
            {
                Text = $"{a.NomAgent} | Prénom : {a.PrenomAgent} | Sexe : {a.SexeAgent} | Nationalité : {a.Nationalite_Agent}",
                Value = a.AgentId.ToString()
            });

            demandeViewModel.Demandes = new Demande
            {
                AgentId = userId
            };
            return View(demandeViewModel);
        }
        public IActionResult Details(int demandeId)
        {
            
            Demande? demande = _db.Demandes.FirstOrDefault(d => d.DemandeId == demandeId);
            Ajout? ajout = _db.Ajouts.FirstOrDefault(d => d.DemandeId == demandeId);
            Approvisionnement? approv = _db.Approvisionnements.FirstOrDefault(d => d.DemandeId == demandeId);
            DemandeViewModel demandeViewModel = new DemandeViewModel();
            if (demande is null)
            {
                return RedirectToAction("Error", "Home");
            }

            demandeViewModel.Comprendre = _db.Comprendres
               .Include(d => d.Demande)
               .Include(d => d.Boisson)
               .ToList()
               .Where(d => d.DemandeId == demandeId);

            demandeViewModel.Comporter = _db.Comporters
                .Include(d => d.Demande)
                .Include(d => d.Aliment)
                .ToList()
                .Where(d => d.DemandeId == demandeId);

            demandeViewModel.Boisson = _db.Boissons
                .Include(b => b.CategorieBoisson)
                .Where(b => !_db.Comprendres
                .Any(c => c.BoissonId == b.BoissonId && c.DemandeId == demandeId))
                .OrderBy(b => b.TitreBoisson)
                .ToList();

            demandeViewModel.Aliment = _db.Aliments
                .Where(a => !_db.Comporters
                .Any(f => f.AlimentId == a.AlimentId && f.DemandeId == demandeId))
                .ToList();

            demandeViewModel.AgentList = _db.Agents.ToList().Select(a => new SelectListItem
            {
                Text = $"{a.NomAgent} | Prénom : {a.PrenomAgent} | Sexe : {a.SexeAgent} | Nationalité : {a.Nationalite_Agent}",
                Value = a.AgentId.ToString()
            });

            demandeViewModel.Demandes = new Demande
            {
                DemandeId = demande.DemandeId,
                AgentId = demande.AgentId,
                DateDemande = demande.DateDemande,
                TypeDemande = demande.TypeDemande
            };
            if(ajout is not null)
            {
                demandeViewModel.Demandes.EtatDemande = ajout.EtatDemandeAjout;
            }
            if (approv is not null)
            {
                demandeViewModel.Demandes.EtatDemande = approv.EtatDemandeAppro;
            }

            return View(demandeViewModel);
        }

        public IActionResult Associer(int demandeId)
        {

            Demande? demande = _db.Demandes.FirstOrDefault(d => d.DemandeId == demandeId);
            Ajout? ajout = _db.Ajouts.FirstOrDefault(d => d.DemandeId == demandeId);
            Approvisionnement? approv = _db.Approvisionnements.FirstOrDefault(d => d.DemandeId == demandeId);
            DemandeViewModel demandeViewModel = new DemandeViewModel();
            if (demande is null)
            {
                return RedirectToAction("Error", "Home");
            }

            demandeViewModel.Comprendre = _db.Comprendres
               .Include(d => d.Demande)
               .Include(d => d.Boisson)
               .ToList()
               .Where(d => d.DemandeId == demandeId);

            demandeViewModel.Comporter = _db.Comporters
                .Include(d => d.Demande)
                .Include(d => d.Aliment)
                .ToList()
                .Where(d => d.DemandeId == demandeId);

            demandeViewModel.Boisson = _db.Boissons
                .Include(b => b.CategorieBoisson)
                .Where(b => !_db.Comprendres
                .Any(c => c.BoissonId == b.BoissonId && c.DemandeId == demandeId))
                .OrderBy(b => b.TitreBoisson)
                .ToList();

            demandeViewModel.Aliment = _db.Aliments
                .Where(a => !_db.Comporters
                .Any(f => f.AlimentId == a.AlimentId && f.DemandeId == demandeId))
                .ToList();

            demandeViewModel.AgentList = _db.Agents.ToList().Select(a => new SelectListItem
            {
                Text = $"{a.NomAgent} | Prénom : {a.PrenomAgent} | Sexe : {a.SexeAgent} | Nationalité : {a.Nationalite_Agent}",
                Value = a.AgentId.ToString()
            });

            demandeViewModel.Demandes = new Demande
            {
                DemandeId = demande.DemandeId,
                AgentId = demande.AgentId,
                DateDemande = demande.DateDemande,
                TypeDemande = demande.TypeDemande
            };
            if (ajout is not null)
            {
                demandeViewModel.Demandes.EtatDemande = ajout.EtatDemandeAjout;
            }
            if (approv is not null)
            {
                demandeViewModel.Demandes.EtatDemande = approv.EtatDemandeAppro;
            }

            return View(demandeViewModel);
        }
        private void ValidationDate(DateTime date, string fieldName, string errorMessage)
        {
            if (date < DateTime.Today)
            {
                ModelState.AddModelError(fieldName, errorMessage);
            }
        }
        public IActionResult Update(int demandeId)
        {
            Demande? demande = _db.Demandes.FirstOrDefault(d => d.DemandeId == demandeId);
            Ajout? ajout = _db.Ajouts.FirstOrDefault(d => d.DemandeId == demandeId);
            Approvisionnement? approv = _db.Approvisionnements.FirstOrDefault(d => d.DemandeId == demandeId);

            DemandeViewModel demandeViewModel = new DemandeViewModel();
            if (demande is null)
            {
                return RedirectToAction("Error", "Home");
            }
            demandeViewModel.AgentList = _db.Agents.ToList().Select(a => new SelectListItem
            {
                Text = $"{a.NomAgent} | Prénom : {a.PrenomAgent} | Sexe : {a.SexeAgent} | Nationalité : {a.Nationalite_Agent}",
                Value = a.AgentId.ToString()
            });

            demandeViewModel.Demandes = new Demande
            {
                DemandeId = demande.DemandeId,
                AgentId = demande.AgentId,
                DateDemande = demande.DateDemande,
                TypeDemande = demande.TypeDemande
            };
            if (ajout is not null)
            {
                demandeViewModel.Demandes.EtatDemande = ajout.EtatDemandeAjout;
            }
            if (approv is not null)
            {
                demandeViewModel.Demandes.EtatDemande = approv.EtatDemandeAppro;
            }
            return View(demandeViewModel);
        }

        [HttpPost]
        public IActionResult Update(DemandeViewModel demandeViewModel)
        {
            var detailsDemande = _db.DetailsDemandes.Where(dt => dt.DemandeId == demandeViewModel.Demandes.DemandeId).ToList();

            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            int userId = userIdClaim != null ? int.Parse(userIdClaim.Value) : 0;
            Agent agent = _db.Agents.FirstOrDefault(a => a.AgentId == userId);

            Demande? demande = _db.Demandes.FirstOrDefault(d => d.DemandeId == demandeViewModel.Demandes.DemandeId);
            Ajout? ajout = _db.Ajouts.FirstOrDefault(d => d.DemandeId == demandeViewModel.Demandes.DemandeId);
            Approvisionnement? approv = _db.Approvisionnements.FirstOrDefault(d => d.DemandeId == demandeViewModel.Demandes.DemandeId);

            if (ModelState.IsValid)
            {
                DateTime dtdemande = demandeViewModel.Demandes.DateDemande;

                if (demandeViewModel.Demandes.TypeDemande != "AJOUT")
                {
                    if ((agent.Fonction_Agent == "CUISINIER") || (agent.Fonction_Agent == "BARMAN"))
                    {
                        ModelState.AddModelError("Demandes.TypeDemande", "Votre fonction ne vous permet pas d'emettre ce type de demande.");
                    }
                }
                //ValidationDate(dtdemande, "Demandes.DateDemande",
                //             "La date de la demande ne peut pas être antérieure à la date du jour.");
                if (ModelState.IsValid)
                {
                    
                    if (demandeViewModel.Demandes.TypeDemande.ToUpper() == "AJOUT")
                    {
                        if (ajout is null)
                        {
                            //Si type change pour ajout créer nouvel ajout
                            ajout = new Ajout
                            {
                                AgentId = demandeViewModel.Demandes.AgentId,
                                DateDemande = demandeViewModel.Demandes.DateDemande,
                                TypeDemande = demandeViewModel.Demandes.TypeDemande,
                                EtatDemandeAjout = "Brouillon",
                            };
                            _db.Ajouts.Add(ajout);
                            _db.Approvisionnements.Remove(approv);
                            _db.SaveChanges();

                            if (detailsDemande is not null)
                            {
                                foreach (var dt in detailsDemande)
                                {
                                    dt.DemandeId = ajout.DemandeId;
                                    dt.TypeDemande = "SORTIE POUR AJOUT";
                                    _db.DetailsDemandes.UpdateRange(dt);
                                    _db.SaveChanges();

                                }

                            }
                        }
                        else
                        {
                            //Si type ne change pas modifier ajout
                            ajout.DemandeId = demande.DemandeId;
                            ajout.AgentId = userId;
                            ajout.DateDemande = demandeViewModel.Demandes.DateDemande;
                            ajout.TypeDemande = demandeViewModel.Demandes.TypeDemande;
                            ajout.EtatDemandeAjout = "Brouillon";
                            _db.Ajouts.Update(ajout);
                            _db.SaveChanges();

                        }
                    }
                    else if (demandeViewModel.Demandes.TypeDemande == "APPROVISIONNEMENT")
                    {
                        if (approv is null)
                        {
                            //Si type change pour approvisionnement créer nouveau approvisionnement

                            approv = new Approvisionnement
                            {
                                AgentId = demandeViewModel.Demandes.AgentId,
                                DateDemande = demandeViewModel.Demandes.DateDemande,
                                TypeDemande = demandeViewModel.Demandes.TypeDemande,
                                EtatDemandeAppro = "Brouillon",
                            };
                            _db.Approvisionnements.Add(approv);
                            _db.Ajouts.Remove(ajout);
                            _db.SaveChanges();

                            if (detailsDemande is not null)
                            {
                                foreach (var dt in detailsDemande)
                                {
                                    dt.DemandeId = approv.DemandeId;
                                    dt.TypeDemande = "APPROVISIONNEMENT";
                                    _db.DetailsDemandes.UpdateRange(dt);
                                    _db.SaveChanges();

                                }

                            }

                        }
                        else
                        {
                            //Si type ne change pas modifier approvisionnement

                            approv.DemandeId = demande.DemandeId;
                            approv.AgentId = userId;
                            approv.DateDemande = demandeViewModel.Demandes.DateDemande;
                            approv.TypeDemande = demandeViewModel.Demandes.TypeDemande;
                            approv.EtatDemandeAppro = "Brouillon";
                            _db.Approvisionnements.Update(approv);
                            _db.SaveChanges();

                        }
                    }
                }
                else
                {
                    TempData["erreur"] = "Demmande non trouvée.";
                    return RedirectToAction("Index");
                }
                TempData["success"] = "Demande mise à jour.";
                return RedirectToAction("Index");

            }
            demandeViewModel.AgentList = _db.Agents.ToList().Select(a => new SelectListItem
            {
                Text = $"{a.NomAgent} | Prénom : {a.PrenomAgent} | Sexe : {a.SexeAgent} | Nationalité : {a.Nationalite_Agent}",
                Value = a.AgentId.ToString()
            });

            demandeViewModel.Demandes = new Demande
            {
                DemandeId = demande.DemandeId,
                AgentId = demande.AgentId,
                DateDemande = demande.DateDemande,
                TypeDemande = demande.TypeDemande
            };
            if (ajout is not null)
            {
                demandeViewModel.Demandes.EtatDemande = ajout.EtatDemandeAjout;
            }
            if (approv is not null)
            {
                demandeViewModel.Demandes.EtatDemande = approv.EtatDemandeAppro;
            }
            return View(demandeViewModel);
        }

        public IActionResult Delete(int demandeId)
        {

            Demande? demande = _db.Demandes.FirstOrDefault(d => d.DemandeId == demandeId);
            Ajout? ajout = _db.Ajouts.FirstOrDefault(d => d.DemandeId == demandeId);
            Approvisionnement? approv = _db.Approvisionnements.FirstOrDefault(d => d.DemandeId == demandeId);
            DemandeViewModel demandeViewModel = new DemandeViewModel();
            if (demande is null)
            {
                return RedirectToAction("Error", "Home");
            }

            demandeViewModel.Comprendre = _db.Comprendres
               .Include(d => d.Demande)
               .Include(d => d.Boisson)
               .ToList()
               .Where(d => d.DemandeId == demandeId);

            demandeViewModel.Comporter = _db.Comporters
                .Include(d => d.Demande)
                .Include(d => d.Aliment)
                .ToList()
                .Where(d => d.DemandeId == demandeId);

            demandeViewModel.Boisson = _db.Boissons
                .Include(b => b.CategorieBoisson)
                .Where(b => !_db.Comprendres
                .Any(c => c.BoissonId == b.BoissonId && c.DemandeId == demandeId))
                .OrderBy(b => b.TitreBoisson)
                .ToList();

            demandeViewModel.Aliment = _db.Aliments
                .Where(a => !_db.Comporters
                .Any(f => f.AlimentId == a.AlimentId && f.DemandeId == demandeId))
                .ToList();

            demandeViewModel.AgentList = _db.Agents.ToList().Select(a => new SelectListItem
            {
                Text = $"{a.NomAgent} | Prénom : {a.PrenomAgent} | Sexe : {a.SexeAgent} | Nationalité : {a.Nationalite_Agent}",
                Value = a.AgentId.ToString()
            });

            demandeViewModel.Demandes = new Demande
            {
                DemandeId = demande.DemandeId,
                AgentId = demande.AgentId,
                DateDemande = demande.DateDemande,
                TypeDemande = demande.TypeDemande
            };
            if (ajout is not null)
            {
                demandeViewModel.Demandes.EtatDemande = ajout.EtatDemandeAjout;
            }
            if (approv is not null)
            {
                demandeViewModel.Demandes.EtatDemande = approv.EtatDemandeAppro;
            }

            return View(demandeViewModel);
        }

        [HttpPost]
        public IActionResult Delete(DemandeViewModel demandeViewModel)
        {

            var detailsDemande = _db.DetailsDemandes.Where(dt => dt.DemandeId == demandeViewModel.Demandes.DemandeId).ToList();

            Ajout? ajout = _db.Ajouts.FirstOrDefault(d => d.DemandeId == demandeViewModel.Demandes.DemandeId);
            Approvisionnement? approv = _db.Approvisionnements.FirstOrDefault(d => d.DemandeId == demandeViewModel.Demandes.DemandeId);
            Demande? mydemande = _db.Demandes.FirstOrDefault(d => d.DemandeId == demandeViewModel.Demandes.DemandeId);

            try
            {
                if (mydemande is not null)
                {
                    if (demandeViewModel.Demandes.TypeDemande == "AJOUT")
                    {
                        var ajouttodelete = _db.Ajouts.FirstOrDefault(a => a.DemandeId == demandeViewModel.Demandes.DemandeId);
                        if (ajouttodelete != null)
                        {
                            _db.Ajouts.Remove(ajouttodelete);
                            if(detailsDemande is not null)
                            {
                                foreach(var dt in detailsDemande)
                                {
                                    _db.DetailsDemandes.RemoveRange(dt);
                                }

                            }
                            _db.SaveChanges();
                        }
                    }
                    else if (demandeViewModel.Demandes.TypeDemande == "APPROVISIONNEMENT")
                    {
                        var approvtodelete = _db.Approvisionnements.FirstOrDefault(a => a.DemandeId == demandeViewModel.Demandes.DemandeId);
                        if (approvtodelete != null)
                        {
                            _db.Approvisionnements.Remove(approvtodelete);
                            if (detailsDemande is not null)
                            {
                                foreach (var dt in detailsDemande)
                                {
                                    _db.DetailsDemandes.RemoveRange(dt);
                                }

                            }
                            _db.SaveChanges();
                        }
                    }
                    else
                    {
                        _db.Demandes.Remove(mydemande);
                        if (detailsDemande is not null)
                        {
                            foreach (var dt in detailsDemande)
                            {
                                _db.DetailsDemandes.RemoveRange(dt);
                            }

                        }
                        _db.SaveChanges();
                        TempData["success"] = "Demande supprimée."; // Ajoutez ici le message de succès
                        return RedirectToAction("Index");
                    }

                    // Vérifiez l'état des demandes avant de supprimer les comportements et compréhensions
                    if (approv?.EtatDemandeAppro != "Brouillon" || ajout?.EtatDemandeAjout != "Brouillon")
                    {
                        Comporter? comporter = _db.Comporters.FirstOrDefault(c => c.DemandeId == demandeViewModel.Demandes.DemandeId);
                        Comprendre? comprendre = _db.Comprendres.FirstOrDefault(c => c.DemandeId == demandeViewModel.Demandes.DemandeId);
                        if (comporter is not null)
                        {
                            _db.RemoveRange(comporter);
                        }
                        if (comprendre is not null)
                        {
                            _db.RemoveRange(comprendre);
                        }
                        _db.SaveChanges();
                    }

                    TempData["success"] = "Demande supprimée.";
                    return RedirectToAction("Index");
                }

                TempData["error"] = "Impossible de supprimer cette demande.";
            }
            catch
            {
                TempData["error"] = "Impossible de supprimer la demande : vérifiez qu'elle est un brouillon.";
            }

            // Préparation du modèle de vue pour affichage en cas d'erreur
            demandeViewModel.AgentList = _db.Agents.ToList().Select(a => new SelectListItem
            {
                Text = $"{a.NomAgent} | Prénom : {a.PrenomAgent} | Sexe : {a.SexeAgent} | Nationalité : {a.Nationalite_Agent}",
                Value = a.AgentId.ToString()
            });

            demandeViewModel.Demandes = new Demande
            {
                DemandeId = mydemande.DemandeId,
                AgentId = mydemande.AgentId,
                DateDemande = mydemande.DateDemande,
                TypeDemande = mydemande.TypeDemande
            };

            if (ajout is not null)
            {
                demandeViewModel.Demandes.EtatDemande = ajout.EtatDemandeAjout;
            }
            if (approv is not null)
            {
                demandeViewModel.Demandes.EtatDemande = approv.EtatDemandeAppro;
            }
            return View(demandeViewModel);
        }
        public class ProduitSelectionnes
        {
            public int Id { get; set; }
            public int Quantite { get; set; }
        }
        public class AssociationProduits
        {
            public List<ProduitSelectionnes> AlimentIds { get; set; }
            public List<ProduitSelectionnes> BoissonIds { get; set; }
        }
        
        [HttpPost]
        public IActionResult AssocierProduits([FromBody] AssociationProduits data, int demandeId)
        {
            try
            {
                DetailsDemande detailsDemandenull = _db.DetailsDemandes.Where(dt => dt.DemandeId == demandeId && dt.ProduitDemande.ToUpper() == "NULL").FirstOrDefault();
                DetailsDemande detailsDemandeexist = _db.DetailsDemandes.FirstOrDefault(dt => dt.DemandeId == demandeId);

                // Traitement pour les aliments
                if (data.AlimentIds != null && data.AlimentIds.Any())
                {
                    foreach (var aliment in data.AlimentIds)
                    {
                        Comporter comporter = new()
                        {
                            AlimentId = aliment.Id,
                            DemandeId = demandeId,
                            QteOrdone = aliment.Quantite
                        };
                        _db.Comporters.Add(comporter);

                        DetailsDemande detailsDemande = new DetailsDemande()
                        {
                            Agent = detailsDemandeexist.Agent,
                            DemandeId = detailsDemandeexist.DemandeId,
                            DateDemande = detailsDemandeexist.DateDemande,
                            TypeDemande = detailsDemandeexist.TypeDemande,
                            EtatDemande = detailsDemandeexist.EtatDemande,
                            ProduitDemande = TrouverDonneesBydata(comporter.AlimentId, "Aliment").ToString(),
                            QteDemande = comporter.QteOrdone
                        };
                        _db.DetailsDemandes.Add(detailsDemande);
                    }
                    _db.SaveChanges();
                }

                // Traitement pour les boissons
                if (data.BoissonIds != null && data.BoissonIds.Any())
                {
                    foreach (var boisson in data.BoissonIds)
                    {
                        Comprendre comprendre = new()
                        {
                            BoissonId = boisson.Id,
                            DemandeId = demandeId,
                            QteDemande = boisson.Quantite
                        };
                        _db.Comprendres.Add(comprendre);

                        DetailsDemande detailsDemande = new DetailsDemande()
                        {
                            Agent = detailsDemandeexist.Agent,
                            DemandeId = detailsDemandeexist.DemandeId,
                            DateDemande = detailsDemandeexist.DateDemande,
                            TypeDemande = detailsDemandeexist.TypeDemande,
                            EtatDemande = detailsDemandeexist.EtatDemande,
                            ProduitDemande = TrouverDonneesBydata(comprendre.BoissonId, "Boisson").ToString(),
                            QteDemande = comprendre.QteDemande
                        };
                        _db.DetailsDemandes.Add(detailsDemande);
                    }
                }
                if(detailsDemandenull is not null)
                {
                    _db.DetailsDemandes.Remove(detailsDemandenull);

                }

                var approv = _db.Approvisionnements.FirstOrDefault(d => d.DemandeId == demandeId);
                var ajout = _db.Ajouts.FirstOrDefault(d => d.DemandeId == demandeId);
                if(approv is not null)
                {
                    approv.EtatDemandeAppro = "Validée";
                    _db.Approvisionnements.Update(approv);
                }
                if (ajout is not null)
                {
                    ajout.EtatDemandeAjout = "Validée";
                    _db.Ajouts.Update(ajout);
                }
                
                _db.SaveChanges();
               
                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, error = ex.Message });
            }
        }

        public class DissociationProduits
        {
            public int produitId { get; set; }
            public int demandeId { get; set; }
            public string type { get; set; }
        }
        [HttpPost]
        public IActionResult DissocierProduits([FromBody] DissociationProduits data)
        {
            try
            {
                Aliment aliment = null;
                Boisson boisson = null;

                if (data.type == "aliment")
                {
                    aliment = _db.Aliments.FirstOrDefault(a => a.AlimentId == data.produitId);
                }
                else if (data.type == "boisson")
                {
                    boisson = _db.Boissons.FirstOrDefault(b => b.BoissonId == data.produitId);
                }

                // Vérifiez si l'aliment ou la boisson a été trouvé
                if (data.type == "aliment" && aliment is null)
                {
                    return Json(new { success = false, message = "Aliment introuvable" });
                }
                else if (data.type == "boisson" && boisson is null)
                {
                    return Json(new { success = false, message = "Boisson introuvable" });
                }

                Comporter comporter = null;
                Comprendre comprendre = null;

                if (aliment is not null)
                {
                     comporter = _db.Comporters.FirstOrDefault(c => c.AlimentId == aliment.AlimentId && c.DemandeId == data.demandeId);
                }
                else if (boisson is not null)
                {
                    comprendre = _db.Comprendres.FirstOrDefault(c => c.BoissonId == boisson.BoissonId && c.DemandeId == data.demandeId);
                }
                
                if (comporter is not null)
                {
                    DetailsDemande detailsDemande = _db.DetailsDemandes.FirstOrDefault(dt => dt.DemandeId == comporter.DemandeId && dt.ProduitDemande ==TrouverDonneesBydata(comporter.AlimentId,"Aliment"));

                    _db.Comporters.Remove(comporter);
                    if (detailsDemande is not null)
                    {
                        _db.DetailsDemandes.Remove(detailsDemande);

                    }
                    _db.SaveChanges();

                    // Vérifiez si la demande est liée à produit dans comporter
                    var demande = _db.Demandes
                    .Include(d => d.Comprendres) // Inclure les Comprendres
                    .Include(d => d.Comporters) // Inclure les Comporters
                    .FirstOrDefault(d => d.DemandeId == data.demandeId);


                    // Vérifiez si les listes sont null ou vides
                    var comprendresExist = demande.Comprendres != null && demande.Comprendres.Any();
                    var comportersExist = demande.Comporters != null && demande.Comporters.Any();


                    if (!comprendresExist && !comportersExist)
                    {
                        DetailsDemande detailsDemandeexist = _db.DetailsDemandes.FirstOrDefault(dt => dt.DemandeId == data.demandeId);

                        var approv = _db.Approvisionnements.FirstOrDefault(d => d.DemandeId == data.demandeId);
                        var ajout = _db.Ajouts.FirstOrDefault(d => d.DemandeId == data.demandeId);
                        if (approv is not null)
                        {
                            approv.EtatDemandeAppro = "Brouillon";
                            _db.Approvisionnements.Update(approv);
                            if (detailsDemandeexist is not null)
                            {
                                detailsDemandeexist.EtatDemande = approv.EtatDemandeAppro;
                                _db.DetailsDemandes.UpdateRange(detailsDemande);

                            }
                            _db.SaveChanges();

                        }
                        if (ajout is not null)
                        {
                            ajout.EtatDemandeAjout = "Brouillon";
                            _db.Ajouts.Update(ajout);
                            if (detailsDemandeexist is not null)
                            {
                                detailsDemandeexist.EtatDemande = ajout.EtatDemandeAjout;
                                _db.DetailsDemandes.UpdateRange(detailsDemande);

                            }
                            _db.SaveChanges();

                        }
                    }
                    return Json(new { success = true, message = "Aliment retirés avec succès" });
                }
                else if (comprendre is not null)
                {
                    DetailsDemande detailsDemande = _db.DetailsDemandes.FirstOrDefault(dt => dt.DemandeId == comprendre.DemandeId && dt.ProduitDemande == TrouverDonneesBydata(comprendre.BoissonId, "Boisson"));
                    _db.Comprendres.Remove(comprendre);
                    _db.SaveChanges();
                    if (detailsDemande is not null)
                    {
                        _db.DetailsDemandes.Remove(detailsDemande);

                    }
                    var demande = _db.Demandes
                    .Include(d => d.Comprendres) // Inclure les Comprendres
                    .Include(d => d.Comporters) // Inclure les Comporters
                    .FirstOrDefault(d => d.DemandeId == data.demandeId);

                    // Vérifiez si la demande est liée à produit dans comprendre
                    var comprendresExist = demande.Comprendres != null && demande.Comprendres.Any();
                    var comportersExist = demande.Comporters != null && demande.Comporters.Any();


                    if (!comprendresExist && !comportersExist)
                    {
                        DetailsDemande detailsDemandeexist = _db.DetailsDemandes.FirstOrDefault(dt => dt.DemandeId == data.demandeId);
                        var approv = _db.Approvisionnements.FirstOrDefault(d => d.DemandeId == data.demandeId);
                        var ajout = _db.Ajouts.FirstOrDefault(d => d.DemandeId == data.demandeId);
                        if (approv is not null)
                        {
                            approv.EtatDemandeAppro = "Brouillon";
                            _db.Approvisionnements.Update(approv);
                            if (detailsDemandeexist is not null)
                            {
                                detailsDemandeexist.EtatDemande = approv.EtatDemandeAppro;
                                _db.DetailsDemandes.UpdateRange(detailsDemande);

                            }
                            _db.SaveChanges();

                        }
                        if (ajout is not null)
                        {
                            ajout.EtatDemandeAjout = "Brouillon";
                            _db.Ajouts.Update(ajout);
                            if (detailsDemandeexist is not null)
                            {
                                detailsDemandeexist.EtatDemande = ajout.EtatDemandeAjout;
                                _db.DetailsDemandes.UpdateRange(detailsDemande);

                            }
                            _db.SaveChanges();

                        }
                    }
                    return Json(new { success = true, message = "Boisson retirés avec succès" });
                }
                else
                {
                    return Json(new { success = false, message = "Demande introuvable" });
                }

            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

    }
}
