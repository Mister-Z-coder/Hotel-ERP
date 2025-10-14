
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
    [Authorize(Roles = "ADMIN,GESTIONNAIRE")]
    [CaisseRedirectFilter]
    public class MouvementController : Controller
    {
        private readonly ApplicationDbContext _db;

        public MouvementController(ApplicationDbContext db)
        {
            _db = db;
        }

        public IActionResult Index(int page = 1, int size = 20, string searchType = "", string searchAgent = "", string searchDate = "", string searchNature = "")
        {

            DateTime parsedDate;
            bool isDateValid = DateTime.TryParse(searchDate, out parsedDate);
            int totalmouvements = _db.Mouvements.Count();
            int skipValue = (page - 1) * size;
            int totalPages = (int)Math.Ceiling((double)totalmouvements / size);
            //var demandes = _db.Demandes.ToList().Skip(skipValue).Take(size);
            
            var mouvements = (from mvm in _db.Mouvements
                           orderby mvm.MouvementId descending
                           select new Mouvement
                           {
                               MouvementId = mvm.MouvementId,
                               AgentId = mvm.AgentId,
                               Agent = mvm.Agent,
                               TypeMouvementId = mvm.TypeMouvementId,
                               TypeMouvement = mvm.TypeMouvement,
                               DateMouvement = mvm.DateMouvement,
                               HeureMouvement = mvm.HeureMouvement,
                               NatureMouvement= mvm.NatureMouvement,

                           }).Where(m =>
                           (string.IsNullOrEmpty(searchType) ||m.TypeMouvement.TitreTypeMouvement!.ToUpper().Contains(searchType.ToUpper())) &&
                           (string.IsNullOrEmpty(searchDate) || m.DateMouvement! == Convert.ToDateTime(parsedDate.Date)) &&
                           (string.IsNullOrEmpty(searchNature) || m.NatureMouvement!.ToUpper().Contains(searchNature.ToUpper())) &&
                           (string.IsNullOrEmpty(searchAgent) || m.Agent.NomAgent!.ToUpper().Contains(searchAgent.ToUpper()) || m.Agent.PrenomAgent!.ToUpper().Contains(searchAgent.ToUpper())))
                           .ToList().Skip(skipValue).Take(size);

            
            var depart = (totalPages - 1) * skipValue + 1;
            ViewBag.NumAff = depart;
            ViewBag.CurrentPage = page;
            ViewBag.PageSize = size;
            ViewBag.TotalPage = totalPages;
            
            MouvementViewModel mouvementViewModel = new()
            {
                Mouvement = mouvements
            };
            return View(mouvementViewModel);
        }

        public IActionResult DetailsLivraison(int mouvementId)
        {
            var livraisonEntity = _db.Livraisons
                           .Where(l => l.MouvementId == mouvementId)
                           .FirstOrDefault();
            Livraison livraison = _db.Livraisons.FirstOrDefault(l => l.ApprovisionnementId == livraisonEntity.ApprovisionnementId);
            Demande? demande = _db.Demandes.FirstOrDefault(d => d.DemandeId == livraisonEntity.ApprovisionnementId);

            Ajout? ajout = _db.Ajouts.FirstOrDefault(d => d.DemandeId == livraisonEntity.ApprovisionnementId);
            Approvisionnement? approv = _db.Approvisionnements.FirstOrDefault(d => d.DemandeId == livraisonEntity.ApprovisionnementId);
            DemandeViewModel demandeViewModel = new DemandeViewModel();
            if (demande is null)
            {
                return RedirectToAction("Error", "Home");
            }

            demandeViewModel.Comprendre = _db.Comprendres
               .Include(d => d.Demande)
               .Include(d => d.Boisson)
               .ToList()
               .Where(d => d.DemandeId == livraisonEntity.ApprovisionnementId);

            demandeViewModel.Comporter = _db.Comporters
                .Include(d => d.Demande)
                .Include(d => d.Aliment)
                .ToList()
                .Where(d => d.DemandeId == livraisonEntity.ApprovisionnementId);

            demandeViewModel.Boisson = _db.Boissons
                .Include(b => b.CategorieBoisson)
                .Where(b => !_db.Comprendres
                .Any(c => c.BoissonId == b.BoissonId && c.DemandeId == livraisonEntity.ApprovisionnementId))
                .OrderBy(b => b.TitreBoisson)
                .ToList();

            demandeViewModel.Aliment = _db.Aliments
                .Where(a => !_db.Comporters
                .Any(f => f.AlimentId == a.AlimentId && f.DemandeId == livraisonEntity.ApprovisionnementId))
                .ToList();

            demandeViewModel.AgentList = _db.Agents.ToList().Select(a => new SelectListItem
            {
                Text = $"{a.NomAgent} | Prénom : {a.PrenomAgent} | Sexe : {a.SexeAgent} | Nationalité : {a.Nationalite_Agent}",
                Value = a.AgentId.ToString()
            });

            var typemouvement = _db.TypeMouvements.FirstOrDefault(t => t.TitreTypeMouvement == "ENTREE");

            demandeViewModel.TypeMouvementList = _db.TypeMouvements.Where(t => t.TypeMouvementId == typemouvement.TypeMouvementId).ToList().Select(t => new SelectListItem
            {
                Text = $"{t.TitreTypeMouvement}",
                Value = t.TypeMouvementId.ToString()
            });

            //demandeViewModel.Livraisons.TypeMouvementId = typemouvement.TypeMouvementId;
            // Récupérer les IDs des produits dans Comporters et Comprendres
            var alimentIds = demandeViewModel.Comporter.Select(c => c.AlimentId).ToList();
            var boissonIds = demandeViewModel.Comprendre.Select(c => c.BoissonId).ToList();

            // Récupérer les fournisseurs qui peuvent fournir les produits spécifiques
            var fournisseursAliment = _db.Fournirs
                .Where(f => alimentIds.Contains(f.AlimentId)) // Filtrer par AlimentId
                .Select(f => new
                {
                    FournisseurId = f.FournisseurId,
                    NomFournisseur = f.Fournisseur.NomFournisseur,
                    PhoneFournisseur = f.Fournisseur.PhoneFournisseur,
                    DomaineActivite = f.Fournisseur.DomaineActivite
                });

            var fournisseursBoisson = _db.Procurers
                .Where(f => boissonIds.Contains(f.BoissonId)) // Filtrer par BoissonId
                .Select(f => new
                {
                    FournisseurId = f.FournisseurId,
                    NomFournisseur = f.Fournisseur.NomFournisseur,
                    PhoneFournisseur = f.Fournisseur.PhoneFournisseur,
                    DomaineActivite = f.Fournisseur.DomaineActivite
                });

            // Combiner les listes de fournisseurs
            var fournisseursCombines = fournisseursAliment
                .Union(fournisseursBoisson)
                .Distinct()
                .ToList();

            // Convertir en SelectListItem
            demandeViewModel.FournisseurList = fournisseursCombines
                .Select(f => new SelectListItem
                {
                    Text = $"{f.NomFournisseur} | Téléphone : {f.PhoneFournisseur} | Domaine : {f.DomaineActivite}",
                    Value = f.FournisseurId.ToString()
                }).ToList();


            demandeViewModel.Demandes = new Demande
            {
                DemandeId = demande.DemandeId,
                AgentId = demande.AgentId,
                DateDemande = demande.DateDemande,
                TypeDemande = demande.TypeDemande
            };

            demandeViewModel.Livraisons = livraison; // Remplacez Livraisons par le type approprié
            demandeViewModel.Livraisons.TypeMouvementId = typemouvement.TypeMouvementId;
            demandeViewModel.Livraisons.MontantLivraison = Convert.ToInt32(demandeViewModel.Livraisons.MontantLivraison);
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

        public IActionResult DetailsSortie(int mouvementId)
        {

            var SortiePourAjoutEntity = _db.SortiePourAjouts
                           .Where(s => s.MouvementId == mouvementId)
                           .FirstOrDefault();
            SortiePourAjout sortiePourAjout = _db.SortiePourAjouts.FirstOrDefault(s => s.AjoutId == SortiePourAjoutEntity.AjoutId);
            Demande? demande = _db.Demandes.FirstOrDefault(d => d.DemandeId == SortiePourAjoutEntity.AjoutId);

            Ajout? ajout = _db.Ajouts.FirstOrDefault(d => d.DemandeId == SortiePourAjoutEntity.AjoutId);
            Approvisionnement? approv = _db.Approvisionnements.FirstOrDefault(d => d.DemandeId == SortiePourAjoutEntity.AjoutId);
            DemandeViewModel demandeViewModel = new DemandeViewModel();
            if (demande is null)
            {
                return RedirectToAction("Error", "Home");
            }

            demandeViewModel.Comprendre = _db.Comprendres
               .Include(d => d.Demande)
               .Include(d => d.Boisson)
               .ToList()
               .Where(d => d.DemandeId == SortiePourAjoutEntity.AjoutId);

            demandeViewModel.Comporter = _db.Comporters
                .Include(d => d.Demande)
                .Include(d => d.Aliment)
                .ToList()
                .Where(d => d.DemandeId == SortiePourAjoutEntity.AjoutId);

            demandeViewModel.Boisson = _db.Boissons
                .Include(b => b.CategorieBoisson)
                .Where(b => !_db.Comprendres
                .Any(c => c.BoissonId == b.BoissonId && c.DemandeId == SortiePourAjoutEntity.AjoutId))
                .OrderBy(b => b.TitreBoisson)
                .ToList();

            demandeViewModel.Aliment = _db.Aliments
                .Where(a => !_db.Comporters
                .Any(f => f.AlimentId == a.AlimentId && f.DemandeId == SortiePourAjoutEntity.AjoutId))
                .ToList();

            demandeViewModel.AgentList = _db.Agents.ToList().Select(a => new SelectListItem
            {
                Text = $"{a.NomAgent} | Prénom : {a.PrenomAgent} | Sexe : {a.SexeAgent} | Nationalité : {a.Nationalite_Agent}",
                Value = a.AgentId.ToString()
            });

            var typemouvement = _db.TypeMouvements.FirstOrDefault(t => t.TitreTypeMouvement == "SORTIE");

            demandeViewModel.TypeMouvementList = _db.TypeMouvements.Where(t => t.TypeMouvementId == typemouvement.TypeMouvementId).ToList().Select(t => new SelectListItem
            {
                Text = $"{t.TitreTypeMouvement}",
                Value = t.TypeMouvementId.ToString()
            });

            //demandeViewModel.Livraisons.TypeMouvementId = typemouvement.TypeMouvementId;
            // Récupérer les IDs des produits dans Comporters et Comprendres
            var alimentIds = demandeViewModel.Comporter.Select(c => c.AlimentId).ToList();
            var boissonIds = demandeViewModel.Comprendre.Select(c => c.BoissonId).ToList();

            // Récupérer les fournisseurs qui peuvent fournir les produits spécifiques
            var fournisseursAliment = _db.Fournirs
                .Where(f => alimentIds.Contains(f.AlimentId)) // Filtrer par AlimentId
                .Select(f => new
                {
                    FournisseurId = f.FournisseurId,
                    NomFournisseur = f.Fournisseur.NomFournisseur,
                    PhoneFournisseur = f.Fournisseur.PhoneFournisseur,
                    DomaineActivite = f.Fournisseur.DomaineActivite
                });

            var fournisseursBoisson = _db.Procurers
                .Where(f => boissonIds.Contains(f.BoissonId)) // Filtrer par BoissonId
                .Select(f => new
                {
                    FournisseurId = f.FournisseurId,
                    NomFournisseur = f.Fournisseur.NomFournisseur,
                    PhoneFournisseur = f.Fournisseur.PhoneFournisseur,
                    DomaineActivite = f.Fournisseur.DomaineActivite
                });

            // Combiner les listes de fournisseurs
            var fournisseursCombines = fournisseursAliment
                .Union(fournisseursBoisson)
                .Distinct()
                .ToList();

            // Convertir en SelectListItem
            demandeViewModel.FournisseurList = fournisseursCombines
                .Select(f => new SelectListItem
                {
                    Text = $"{f.NomFournisseur} | Téléphone : {f.PhoneFournisseur} | Domaine : {f.DomaineActivite}",
                    Value = f.FournisseurId.ToString()
                }).ToList();


            demandeViewModel.Demandes = new Demande
            {
                DemandeId = demande.DemandeId,
                AgentId = demande.AgentId,
                DateDemande = demande.DateDemande,
                TypeDemande = demande.TypeDemande
            };

            demandeViewModel.SortiePourAjouts = sortiePourAjout; // Remplacez Livraisons par le type approprié
            demandeViewModel.SortiePourAjouts.TypeMouvementId = typemouvement.TypeMouvementId;
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
        public IActionResult DetailsDeclassement(int mouvementId)
        {
            var declassementEntity = _db.Declassements
                           .Where(d => d.MouvementId == mouvementId)
                           .FirstOrDefault();
            Declassement? declassement = _db.Declassements.FirstOrDefault(d => d.MouvementId == declassementEntity.MouvementId);

            DeclassementViewModel declassementViewModel = new DeclassementViewModel();
            if (declassement is null)
            {
                return RedirectToAction("Error", "Home");
            }

            declassementViewModel.Bouger = _db.Bougers
               .Include(b => b.Mouvement)
               .Include(b => b.Boisson)
               .ToList()
               .Where(m => m.MouvementId == declassementEntity.MouvementId);

            declassementViewModel.Mouvoir = _db.Mouvoirs
                .Include(m => m.Mouvement)
                .Include(m => m.Aliment)
                .ToList()
                .Where(m => m.MouvementId == declassementEntity.MouvementId);

            declassementViewModel.Boisson = _db.Boissons
                .Include(b => b.CategorieBoisson)
                .Where(b => b.QuantiteStock > 0 &&
                             !_db.Bougers.Any(c => c.BoissonId == b.BoissonId && c.MouvementId == declassementEntity.MouvementId))
                .OrderBy(b => b.TitreBoisson)
                .ToList();

            declassementViewModel.Aliment = _db.Aliments
                .Where(a => a.QuantiteStock > 0 &&
                             !_db.Mouvoirs.Any(f => f.AlimentId == a.AlimentId && f.MouvementId == declassementEntity.MouvementId))
                .OrderBy(a => a.TitreAliment) // Ajout de l'ordre si nécessaire
                .ToList();

            declassementViewModel.AgentList = _db.Agents.ToList().Select(a => new SelectListItem
            {
                Text = $"{a.NomAgent} | Prénom : {a.PrenomAgent} | Sexe : {a.SexeAgent} | Nationalité : {a.Nationalite_Agent}",
                Value = a.AgentId.ToString()
            });

            declassementViewModel.TypeMouvementList = _db.TypeMouvements.ToList().Select(t => new SelectListItem
            {
                Text = $"{t.TitreTypeMouvement}",
                Value = t.TypeMouvementId.ToString()
            });
            if (declassement is not null)
            {
                declassementViewModel.Declassements = declassement;
            }

            return View(declassementViewModel);
        }
        public IActionResult Details(int mouvementId)
        {
            
            Mouvement? mouvement = _db.Mouvements.FirstOrDefault(m => m.MouvementId == mouvementId);
            
            MouvementViewModel mouvementViewModel = new MouvementViewModel();
            if (mouvement is null)
            {
                return RedirectToAction("Error", "Home");
            }

            mouvementViewModel.Bouger = _db.Bougers
               .Include(b => b.Mouvement)
               .Include(b => b.Boisson)
               .ToList()
               .Where(m => m.MouvementId == mouvementId);

            mouvementViewModel.Mouvoir = _db.Mouvoirs
                .Include(m => m.Mouvement)
                .Include(m => m.Aliment)
                .ToList()
                .Where(m => m.MouvementId == mouvementId);

            mouvementViewModel.Boisson = _db.Boissons
                .Include(b => b.CategorieBoisson)
                .Where(b => !_db.Bougers
                .Any(c => c.BoissonId == b.BoissonId && c.MouvementId == mouvementId))
                .OrderBy(b => b.TitreBoisson)
                .ToList();

            mouvementViewModel.Aliment = _db.Aliments
                .Where(a => !_db.Mouvoirs
                .Any(f => f.AlimentId == a.AlimentId && f.MouvementId == mouvementId))
                .ToList();
            
            mouvementViewModel.AgentList = _db.Agents.ToList().Select(a => new SelectListItem
            {
                Text = $"{a.NomAgent} | Prénom : {a.PrenomAgent} | Sexe : {a.SexeAgent} | Nationalité : {a.Nationalite_Agent}",
                Value = a.AgentId.ToString()
            });
            mouvementViewModel.TypeMouvementList = _db.TypeMouvements.ToList().Select(t => new SelectListItem
            {
                Text = $"{t.TitreTypeMouvement}",
                Value = t.TypeMouvementId.ToString()
            });

            mouvementViewModel.Mouvements = new Mouvement
            {
                MouvementId = mouvement.MouvementId,
                AgentId = mouvement.AgentId,
                Agent = mouvement.Agent,
                TypeMouvementId = mouvement.TypeMouvementId,
                TypeMouvement = mouvement.TypeMouvement,
                DateMouvement = mouvement.DateMouvement,
                HeureMouvement = mouvement.HeureMouvement,
                NatureMouvement = mouvement.NatureMouvement
            };
            

            return View(mouvementViewModel);
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
                        _db.Ajouts.Remove(ajouttodelete);
                        _db.SaveChanges();
                    }
                    else if (demandeViewModel.Demandes.TypeDemande == "APPROVISIONNEMENT")
                    {

                        var approvtodelete = _db.Approvisionnements.FirstOrDefault(a => a.DemandeId == demandeViewModel.Demandes.DemandeId);

                        _db.Approvisionnements.Remove(approvtodelete);
                        _db.SaveChanges();
                    }
                    else
                    {
                        _db.Demandes.Remove(mydemande);
                        _db.SaveChanges();
                        TempData["erreur"] = "Demmande non trouvée.";
                        return RedirectToAction("Index");
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
        public class RepportData
        {
            public string Agent { get; set; }
            public string Type { get; set; }
            public string Nature { get; set; }
            public string Date { get; set; }
        }

        [HttpPost]
        public IActionResult GenererDonnesRapportMouvement([FromBody] RepportData data)
        {
            try
            {
                var mouvements = _db.Mouvements
                    .Include(m => m.Agent)
                    .Include(m => m.TypeMouvement)
                    .Include(m => m.Bougers)
                    .Include(m => m.Mouvoirs)
                    .AsNoTracking()
                    .ToList();
                //using var transaction = _db.Database.BeginTransaction();
                if (data.Agent != "" && data.Nature != "" && data.Type != "" && data.Date != "")
                {
                    mouvements = _db.Mouvements
                    .Where(m => m.DateMouvement == Convert.ToDateTime(data.Date)
                             && (m.Agent.NomAgent == data.Agent || m.Agent.PrenomAgent == data.Agent)
                             && m.TypeMouvement.TitreTypeMouvement == data.Type
                             && m.NatureMouvement == data.Nature)
                    .Include(m => m.Agent)
                    .Include(m => m.TypeMouvement)
                    .Include(m => m.Bougers)
                    .Include(m => m.Mouvoirs)
                    .AsNoTracking()
                    .ToList();
                }

                _db.DetailsMouvements.RemoveRange(_db.DetailsMouvements);
                _db.SaveChanges();

                foreach (var mouvement in mouvements)
                {
                    foreach (var mouvoir in mouvement.Mouvoirs)
                    {
                        var detailsMouvement = new DetailsMouvement
                        {
                            MouvementId = mouvement.MouvementId,
                            Agent = mouvement.Agent.NomAgent + " " + mouvement.Agent.PrenomAgent,
                            TypeMouvement = mouvement.TypeMouvement.TitreTypeMouvement,
                            DateMouvement = mouvement.DateMouvement,
                            HeureMouvement = mouvement.HeureMouvement,
                            NatureMouvement = mouvement.NatureMouvement,
                            ProduitMouvement = TrouverDonneesBydata(mouvoir.AlimentId,"Aliment").ToString(),
                            QteMouvement = mouvoir.QteMouv
                        };

                        _db.DetailsMouvements.Add(detailsMouvement);
                        _db.SaveChanges();

                    }

                    foreach (var bouger in mouvement.Bougers)
                    {
                        var detailsMouvement = new DetailsMouvement
                        {
                            MouvementId = mouvement.MouvementId,
                            Agent = mouvement.Agent.NomAgent + " " + mouvement.Agent.PrenomAgent,
                            TypeMouvement = mouvement.TypeMouvement.TitreTypeMouvement,
                            DateMouvement = mouvement.DateMouvement,
                            HeureMouvement = mouvement.HeureMouvement,
                            NatureMouvement = mouvement.NatureMouvement,
                            ProduitMouvement = TrouverDonneesBydata(bouger.BoissonId, "Boisson").ToString(),
                            QteMouvement = bouger.QteBouger
                        };

                        _db.DetailsMouvements.Add(detailsMouvement);
                        _db.SaveChanges();

                    }
                }

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
