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
    public class LivraisonController : Controller
    {
        private readonly ApplicationDbContext _db;

        public LivraisonController(ApplicationDbContext db)
        {
            _db = db;
        }

        public IActionResult Index(int page = 1, int size = 20, string searchEtat = "", string searchAgent = "", string searchDate = "")
        {

            DateTime parsedDate;
            bool isDateValid = DateTime.TryParse(searchDate, out parsedDate);
            int totaldemandes = _db.Approvisionnements.Where(d=>d.TypeDemande== "APPROVISIONNEMENT" && d.EtatDemandeAppro != "Brouillon").Count();
            int skipValue = (page - 1) * size;
            int totalPages = (int)Math.Ceiling((double)totaldemandes / size);
            //var demandes = _db.Demandes.ToList().Skip(skipValue).Take(size);
            
            var demandes = (from dmd in _db.Demandes
                           join appro in _db.Approvisionnements on dmd.DemandeId equals appro.DemandeId into approGroup
                           from appro in approGroup.DefaultIfEmpty() // Jointure externe gauche pour Approvisionnements
                           where appro.TypeDemande == "APPROVISIONNEMENT" && appro.EtatDemandeAppro != "Brouillon"
                            orderby dmd.DemandeId descending
                           select new Demande
                           {
                               DemandeId = dmd.DemandeId,
                               AgentId = dmd.AgentId,
                               Agent = dmd.Agent,
                               DateDemande = dmd.DateDemande,
                               TypeDemande = dmd.TypeDemande,
                               Comporters = dmd.Comporters,
                               Comprendres = dmd.Comprendres,
                               EtatDemande = appro.EtatDemandeAppro // Vérifiez les deux états
                           }).Where(d =>
                           (string.IsNullOrEmpty(searchEtat) ||d.EtatDemande!.ToUpper().Contains(searchEtat.ToUpper())) &&
                           (string.IsNullOrEmpty(searchDate) || d.DateDemande! == Convert.ToDateTime(parsedDate.Date)) &&
                           (string.IsNullOrEmpty(searchAgent) || d.Agent.NomAgent!.ToUpper().Contains(searchAgent.ToUpper()) || d.Agent.PrenomAgent!.ToUpper().Contains(searchAgent.ToUpper())))
                           .ToList().Skip(skipValue).Take(size);

            
            var depart = (totalPages - 1) * skipValue + 1;
            ViewBag.NumAff = depart;
            ViewBag.CurrentPage = page;
            ViewBag.PageSize = size;
            ViewBag.TotalPage = totalPages;
            //if (!string.IsNullOrEmpty(searchString))
            //{
            //    demandes = (from dmd in _db.Demandes
            //                join appro in _db.Approvisionnements on dmd.DemandeId equals appro.DemandeId into approGroup
            //                from appro in approGroup.DefaultIfEmpty() // Jointure externe gauche pour Approvisionnements
            //                join ajout in _db.Ajouts on dmd.DemandeId equals ajout.DemandeId into ajoutGroup
            //                from ajout in ajoutGroup.DefaultIfEmpty() // Jointure externe gauche pour Ajouts
            //                orderby dmd.DateDemande descending
            //                select new Demande
            //                {
            //                    AgentId = dmd.AgentId,
            //                    DateDemande = dmd.DateDemande,
            //                    TypeDemande = dmd.TypeDemande,
            //                    EtatDemande = appro != null ? appro.EtatDemandeAppro : (ajout != null ? ajout.EtatDemandeAjout : null) // Vérifiez les deux états
            //                }).Where(d => d.TypeDemande!.ToUpper().Contains(searchString.ToUpper())).ToList().Skip(skipValue).Take(size);
            //}
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
            else if (context == "TypeMouvement")
            {
                TypeMouvement typeMouvement = _db.TypeMouvements.FirstOrDefault(t => t.TypeMouvementId == donneId);
                valeur = typeMouvement.TitreTypeMouvement;
            }
            else if (context == "Fournisseur")
            {
                Fournisseur fournisseur = _db.Fournisseurs.FirstOrDefault(f => f.FournisseurId == donneId);
                valeur = fournisseur.NomFournisseur;
            }
            return valeur;
        }
        [HttpPost]
        public IActionResult Livrer(DemandeViewModel demandeViewModel)
        {
            var detailsDemande = _db.DetailsDemandes.Where(dt => dt.DemandeId == demandeViewModel.Demandes.DemandeId).ToList();

            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            int userId = userIdClaim != null ? int.Parse(userIdClaim.Value) : 0;
            Agent agent = _db.Agents.FirstOrDefault(a => a.AgentId == userId);

            ModelState.Remove("Demandes.TypeDemande");

            Demande? demande = _db.Demandes.FirstOrDefault(d => d.DemandeId == demandeViewModel.Demandes.DemandeId);
            Ajout? ajout = _db.Ajouts.FirstOrDefault(d => d.DemandeId == demandeViewModel.Demandes.DemandeId);
            Approvisionnement? approv = _db.Approvisionnements.FirstOrDefault(d => d.DemandeId == demandeViewModel.Demandes.DemandeId);

            if (ModelState.IsValid)
            {
                if (demandeViewModel.Livraisons.MontantLivraison <= 0)
                {
                    ModelState.AddModelError("Livraisons.MontantLivraison", "Montant livraison requis.");
                }
                if (ModelState.IsValid)
                {
                    Livraison livraison = new Livraison();
                    demandeViewModel.Livraisons.ApprovisionnementId = demandeViewModel.Demandes.DemandeId;
                    demandeViewModel.Livraisons.AgentId = userId;
                    livraison = demandeViewModel.Livraisons;
                    _db.Livraisons.Add(livraison);
                    approv.EtatDemandeAppro = "Livrée";
                    if (detailsDemande is not null)
                    {
                        foreach (var dt in detailsDemande)
                        {
                            dt.EtatDemande = approv.EtatDemandeAppro;
                            _db.DetailsDemandes.UpdateRange(detailsDemande);
                        }
                            

                    }

                    _db.Approvisionnements.Update(approv);
                    _db.SaveChanges();

                    

                    IEnumerable<Comprendre> comprendre = _db.Comprendres
                       .Include(d => d.Demande)
                       .Include(d => d.Boisson)
                       .ToList()
                       .Where(d => d.DemandeId == demandeViewModel.Demandes.DemandeId);

                    IEnumerable<Comporter> comporter = _db.Comporters
                        .Include(d => d.Demande)
                        .Include(d => d.Aliment)
                        .ToList()
                        .Where(d => d.DemandeId == demandeViewModel.Demandes.DemandeId);

                    if(comporter is not null)
                    {
                        foreach(var aliment in comporter)
                        {
                            Mouvoir mouvoir = new()
                            {
                                AlimentId = aliment.AlimentId,
                                MouvementId = livraison.MouvementId,
                                QteMouv = aliment.QteOrdone
                            };
                            _db.Mouvoirs.Add(mouvoir);

                            var alimentEntity = _db.Aliments
                            .Where(a => a.AlimentId == aliment.AlimentId)
                            .FirstOrDefault();
                            // Mettre à jour le stock
                            alimentEntity.QuantiteStock += aliment.QteOrdone; // Diminuer le stock
                            DetailsLivraison detailsLivraison = new DetailsLivraison()
                            {
                                Agent = TrouverDonneesBydata(livraison.AgentId, "Agent").ToString(),
                                TypeMouvement = TrouverDonneesBydata(livraison.TypeMouvementId, "TypeMouvement").ToString(),
                                Fournisseur = TrouverDonneesBydata(livraison.FournisseurId, "Fournisseur").ToString(),
                                ApprovisionnementId = livraison.ApprovisionnementId,
                                LivraisonId = livraison.MouvementId,
                                DateMouvement = livraison.DateMouvement,
                                HeureMouvement = livraison.HeureMouvement,
                                DateLivraison = livraison.DateLivraison,
                                HeureLivraison = livraison.HeureLivraison,
                                NatureMouvement = livraison.NatureMouvement,
                                MontantLivraison = livraison.MontantLivraison,
                                ProduitLivre = TrouverDonneesBydata(mouvoir.AlimentId,"Aliment"),
                                QteLivre = mouvoir.QteMouv
                            };
                            _db.DetailsLivraisons.Add(detailsLivraison);
                            _db.SaveChanges();
                        }
                    }

                    if (comprendre is not null)
                    {
                        foreach (var boisson in comprendre)
                        {
                            Bouger bouger = new()
                            {
                                BoissonId = boisson.BoissonId,
                                MouvementId = livraison.MouvementId,
                                QteBouger = boisson.QteDemande
                            };
                            _db.Bougers.Add(bouger);

                            var boissonEntity = _db.Boissons
                            .Where(b => b.BoissonId == boisson.BoissonId)
                            .FirstOrDefault();
                            // Mettre à jour le stock
                            boissonEntity.QuantiteStock += boisson.QteDemande; // Diminuer le stock
                            DetailsLivraison detailsLivraison = new DetailsLivraison()
                            {
                                Agent = TrouverDonneesBydata(livraison.AgentId, "Agent").ToString(),
                                TypeMouvement = TrouverDonneesBydata(livraison.TypeMouvementId, "TypeMouvement").ToString(),
                                Fournisseur = TrouverDonneesBydata(livraison.FournisseurId, "Fournisseur").ToString(),
                                ApprovisionnementId = livraison.ApprovisionnementId,
                                LivraisonId = livraison.MouvementId,
                                DateMouvement = livraison.DateMouvement,
                                HeureMouvement = livraison.HeureMouvement,
                                DateLivraison = livraison.DateLivraison,
                                HeureLivraison = livraison.HeureLivraison,
                                NatureMouvement = livraison.NatureMouvement,
                                MontantLivraison = livraison.MontantLivraison,
                                ProduitLivre = TrouverDonneesBydata(bouger.BoissonId, "Boisson"),
                                QteLivre = bouger.QteBouger
                            };
                            _db.DetailsLivraisons.Add(detailsLivraison);
                            _db.SaveChanges();
                        }
                    }
                    _db.SaveChanges();
                    TempData["success"] = "Livraison effectuée.";
                    return RedirectToAction("Index");
                }
            }
            if (demande is null)
            {
                return RedirectToAction("Error", "Home");
            }

            demandeViewModel.Comprendre = _db.Comprendres
               .Include(d => d.Demande)
               .Include(d => d.Boisson)
               .ToList()
               .Where(d => d.DemandeId == demandeViewModel.Demandes.DemandeId);

            demandeViewModel.Comporter = _db.Comporters
                .Include(d => d.Demande)
                .Include(d => d.Aliment)
                .ToList()
                .Where(d => d.DemandeId == demandeViewModel.Demandes.DemandeId);

            demandeViewModel.Boisson = _db.Boissons
                .Include(b => b.CategorieBoisson)
                .Where(b => !_db.Comprendres
                .Any(c => c.BoissonId == b.BoissonId && c.DemandeId == demandeViewModel.Demandes.DemandeId))
                .OrderBy(b => b.TitreBoisson)
                .ToList();

            demandeViewModel.Aliment = _db.Aliments
                .Where(a => !_db.Comporters
                .Any(f => f.AlimentId == a.AlimentId && f.DemandeId == demandeViewModel.Demandes.DemandeId))
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

            if (demandeViewModel.Livraisons == null)
            {
                demandeViewModel.Livraisons = new Livraison(); // Remplacez Livraisons par le type approprié
            }
            demandeViewModel.Livraisons.TypeMouvementId = typemouvement.TypeMouvementId;
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
        public IActionResult Update(int demandeId)
        {
            Livraison livraison = _db.Livraisons.FirstOrDefault(l => l.ApprovisionnementId == demandeId);
            Demande? demande = _db.Demandes.FirstOrDefault(d => d.DemandeId == demandeId);

            Ajout? ajout = _db.Ajouts.FirstOrDefault(d => d.DemandeId == demandeId);
            Approvisionnement? approv = _db.Approvisionnements.FirstOrDefault(d => d.DemandeId == demandeId && d.EtatDemandeAppro == "Livrée");
            DemandeViewModel demandeViewModel = new DemandeViewModel();
            if (approv is null)
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
        public IActionResult DetailsLivraison(int demandeId)
        {
            Livraison livraison = _db.Livraisons.FirstOrDefault(l => l.ApprovisionnementId == demandeId);
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
        public IActionResult DetailsDemande(int demandeId)
        {
            Livraison livraison = _db.Livraisons.FirstOrDefault(l => l.ApprovisionnementId == demandeId);
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

            if (demandeViewModel.Livraisons == null)
            {
                demandeViewModel.Livraisons = new Livraison(); // Remplacez Livraisons par le type approprié
            }
            demandeViewModel.Livraisons.TypeMouvementId = typemouvement.TypeMouvementId;
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

        public IActionResult Livrer(int demandeId)
        {
            Demande? demande = _db.Demandes.FirstOrDefault(d => d.DemandeId == demandeId);
            Ajout? ajout = _db.Ajouts.FirstOrDefault(d => d.DemandeId == demandeId);
            Approvisionnement? approv = _db.Approvisionnements.FirstOrDefault(d => d.DemandeId == demandeId && d.EtatDemandeAppro == "Validée");
            DemandeViewModel demandeViewModel = new DemandeViewModel();
            if (approv is null)
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

            if (demandeViewModel.Livraisons == null)
            {
                demandeViewModel.Livraisons = new Livraison(); // Remplacez Livraisons par le type approprié
            }
            demandeViewModel.Livraisons.TypeMouvementId = typemouvement.TypeMouvementId;
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
            var detailsLivraison = _db.DetailsLivraisons.Where(dt => dt.ApprovisionnementId == demandeViewModel.Demandes.DemandeId).ToList();

            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            int userId = userIdClaim != null ? int.Parse(userIdClaim.Value) : 0;
            Agent agent = _db.Agents.FirstOrDefault(a => a.AgentId == userId);

            ModelState.Remove("Demandes.TypeDemande");

            Demande? demande = _db.Demandes.FirstOrDefault(d => d.DemandeId == demandeViewModel.Demandes.DemandeId);
            Ajout? ajout = _db.Ajouts.FirstOrDefault(d => d.DemandeId == demandeViewModel.Demandes.DemandeId);
            Approvisionnement? approv = _db.Approvisionnements.FirstOrDefault(d => d.DemandeId == demandeViewModel.Demandes.DemandeId);
            Livraison livraison = _db.Livraisons.FirstOrDefault(l => l.MouvementId == demandeViewModel.Livraisons.MouvementId);

            if (ModelState.IsValid)
            {
                if (demandeViewModel.Livraisons.MontantLivraison <= 0)
                {
                    ModelState.AddModelError("Livraisons.MontantLivraison", "Montant livraison requis.");
                }
                if (ModelState.IsValid)
                {
                    //demandeViewModel.Livraisons.DemandeId = demandeViewModel.Demandes.DemandeId;
                    livraison.FournisseurId = demandeViewModel.Livraisons.FournisseurId;
                    livraison.MontantLivraison = demandeViewModel.Livraisons.MontantLivraison;
                    livraison.AgentId = userId;
                    _db.Livraisons.Update(livraison);
                    approv.EtatDemandeAppro = "Livrée";
                    _db.Approvisionnements.Update(approv);
                    if (detailsDemande is not null)
                    {
                        foreach (var dt in detailsDemande)
                        {
                            dt.EtatDemande = approv.EtatDemandeAppro;
                            _db.DetailsDemandes.UpdateRange(detailsDemande);
                        }

                    }

                    if (detailsLivraison is not null)
                    {
                        foreach (var dl in detailsLivraison)
                        {
                            dl.Fournisseur = TrouverDonneesBydata(demandeViewModel.Livraisons.FournisseurId,"Fournisseur");
                            dl.MontantLivraison = demandeViewModel.Livraisons.MontantLivraison;
                            _db.DetailsDemandes.UpdateRange(detailsDemande);
                        }

                    }
                    _db.SaveChanges();
                    TempData["success"] = "Livraison modifiée.";
                    return RedirectToAction("Index");
                }
            }
            if (demande is null)
            {
                return RedirectToAction("Error", "Home");
            }

            demandeViewModel.Comprendre = _db.Comprendres
               .Include(d => d.Demande)
               .Include(d => d.Boisson)
               .ToList()
               .Where(d => d.DemandeId == demandeViewModel.Demandes.DemandeId);

            demandeViewModel.Comporter = _db.Comporters
                .Include(d => d.Demande)
                .Include(d => d.Aliment)
                .ToList()
                .Where(d => d.DemandeId == demandeViewModel.Demandes.DemandeId);

            demandeViewModel.Boisson = _db.Boissons
                .Include(b => b.CategorieBoisson)
                .Where(b => !_db.Comprendres
                .Any(c => c.BoissonId == b.BoissonId && c.DemandeId == demandeViewModel.Demandes.DemandeId))
                .OrderBy(b => b.TitreBoisson)
                .ToList();

            demandeViewModel.Aliment = _db.Aliments
                .Where(a => !_db.Comporters
                .Any(f => f.AlimentId == a.AlimentId && f.DemandeId == demandeViewModel.Demandes.DemandeId))
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
        

    }
}
