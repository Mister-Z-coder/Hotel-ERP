
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
    public class DeclassementController : Controller
    {
        private readonly ApplicationDbContext _db;

        public DeclassementController(ApplicationDbContext db)
        {
            _db = db;
        }

        public IActionResult Index(int page = 1, int size = 20, string searchType = "", string searchAgent = "", string searchDate = "", string searchMotif = "")
        {

            DateTime parsedDate;
            bool isDateValid = DateTime.TryParse(searchDate, out parsedDate);
            int totaldeclassements = _db.Declassements.Where(dc=>dc.NatureMouvement== "DECLASSEMENT").Count();
            int skipValue = (page - 1) * size;
            int totalPages = (int)Math.Ceiling((double)totaldeclassements / size);
            //var demandes = _db.Demandes.ToList().Skip(skipValue).Take(size);
            
            var declassements = (from mvm in _db.Declassements
                              where mvm.NatureMouvement =="DECLASSEMENT"
                           orderby mvm.MouvementId descending
                           select new Declassement
                           {
                               MouvementId = mvm.MouvementId,
                               AgentId = mvm.AgentId,
                               Agent = mvm.Agent,
                               TypeMouvementId = mvm.TypeMouvementId,
                               TypeMouvement = mvm.TypeMouvement,
                               DateMouvement = mvm.DateMouvement,
                               HeureMouvement = mvm.HeureMouvement,
                               NatureMouvement= mvm.NatureMouvement,
                               MotifDecaissement = mvm.MotifDecaissement,
                               Bougers = mvm.Bougers,
                               Mouvoirs = mvm.Mouvoirs
                           }).Where(m =>
                           (string.IsNullOrEmpty(searchType) ||m.TypeMouvement.TitreTypeMouvement!.ToUpper().Contains(searchType.ToUpper())) &&
                           (string.IsNullOrEmpty(searchDate) || m.DateMouvement! == Convert.ToDateTime(parsedDate.Date)) &&
                           (string.IsNullOrEmpty(searchMotif) || m.MotifDecaissement!.ToUpper().Contains(searchMotif.ToUpper())) &&
                           (string.IsNullOrEmpty(searchAgent) || m.Agent.NomAgent!.ToUpper().Contains(searchAgent.ToUpper()) || m.Agent.PrenomAgent!.ToUpper().Contains(searchAgent.ToUpper())))
                           .ToList().Skip(skipValue).Take(size);

            
            var depart = (totalPages - 1) * skipValue + 1;
            ViewBag.NumAff = depart;
            ViewBag.CurrentPage = page;
            ViewBag.PageSize = size;
            ViewBag.TotalPage = totalPages;

            DeclassementViewModel declassementViewModel = new()
            {
                Declassement = declassements
            };
            return View(declassementViewModel);
        }

       
        public IActionResult Details(int declassementId)
        {

            Declassement? declassement = _db.Declassements.FirstOrDefault(d => d.MouvementId == declassementId);

            DeclassementViewModel declassementViewModel = new DeclassementViewModel();
            if (declassement is null)
            {
                return RedirectToAction("Error", "Home");
            }

            declassementViewModel.Bouger = _db.Bougers
               .Include(b => b.Mouvement)
               .Include(b => b.Boisson)
               .ToList()
               .Where(m => m.MouvementId == declassementId);

            declassementViewModel.Mouvoir = _db.Mouvoirs
                .Include(m => m.Mouvement)
                .Include(m => m.Aliment)
                .ToList()
                .Where(m => m.MouvementId == declassementId);

            declassementViewModel.Boisson = _db.Boissons
                .Include(b => b.CategorieBoisson)
                .Where(b => b.QuantiteStock > 0 &&
                             !_db.Bougers.Any(c => c.BoissonId == b.BoissonId && c.MouvementId == declassementId))
                .OrderBy(b => b.TitreBoisson)
                .ToList();

            declassementViewModel.Aliment = _db.Aliments
                .Where(a => a.QuantiteStock > 0 &&
                             !_db.Mouvoirs.Any(f => f.AlimentId == a.AlimentId && f.MouvementId == declassementId))
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

        public IActionResult Create()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            int userId = userIdClaim != null ? int.Parse(userIdClaim.Value) : 0;
            Agent agent = _db.Agents.FirstOrDefault(a => a.AgentId == userId);


            var typemouvement = _db.TypeMouvements.FirstOrDefault(t => t.TitreTypeMouvement == "SORTIE");

            DeclassementViewModel declassementViewModel = new()
            {
                AgentList = _db.Agents.ToList().Where(a => a.AgentId == userId).Select(a => new SelectListItem
                {
                    Text = $"{a.NomAgent} | Prénom : {a.PrenomAgent} | Sexe : {a.SexeAgent} | Nationalité : {a.Nationalite_Agent}",
                    Value = a.AgentId.ToString()
                }),

               
                TypeMouvementList = _db.TypeMouvements.Where(t => t.TypeMouvementId == typemouvement.TypeMouvementId).ToList().Select(t => new SelectListItem
                {
                    Text = $"{t.TitreTypeMouvement}",
                    Value = t.TypeMouvementId.ToString()
                }),
                
                Declassements = new Declassement
                {
                    AgentId = userId,
                    TypeMouvementId = typemouvement.TypeMouvementId
                }
            };
            return View(declassementViewModel);
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
            return valeur;
        }
        [HttpPost]
        public IActionResult Create(DeclassementViewModel declassementViewModel)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    _db.Declassements.Add(declassementViewModel.Declassements);
                    _db.SaveChanges();
                    DetailsDeclassement detailsDeclassement = new DetailsDeclassement()
                    {
                        Agent = TrouverDonneesBydata(declassementViewModel.Declassements.AgentId, "Agent").ToString(),
                        DeclassementId = declassementViewModel.Declassements.MouvementId,
                        DateMouvement = declassementViewModel.Declassements.DateMouvement,
                        HeureMouvement = declassementViewModel.Declassements.HeureMouvement,
                        NatureMouvement = declassementViewModel.Declassements.NatureMouvement,
                        TypeMouvement = TrouverDonneesBydata(declassementViewModel.Declassements.TypeMouvementId, "TypeMouvement").ToString(),
                        MotifDeclassement = declassementViewModel.Declassements.MotifDecaissement,
                        ProduitDeclasse = "NULL"
                    };
                    _db.DetailsDeclassements.Add(detailsDeclassement);
                    _db.SaveChanges();
                    TempData["success"] = "Declassement crée.";
                    return RedirectToAction("Index");
                }
            }catch(Exception ex)
            {
                Console.Write("Une erreur s'est produite : "+ex);
                TempData["erreur"] = "Demmande non crée.";
                return RedirectToAction("Index");
            }
            
            return View();
        }

        public IActionResult Update(int declassementId)
        {
            Declassement? declassement = _db.Declassements.FirstOrDefault(d => d.MouvementId == declassementId);
            if(declassement is null)
            {
                RedirectToAction("Error", "Home");
            }

            DeclassementViewModel declassementViewModel = new DeclassementViewModel();
            
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
            if(declassement is not null)
            {
                declassementViewModel.Declassements = declassement;
            }
            return View(declassementViewModel);
        }

        [HttpPost]
        public IActionResult Update(DeclassementViewModel declassementViewModel)
        {
            try
            {
                var detailsDeclassement = _db.DetailsDeclassements.Where(dc => dc.DeclassementId == declassementViewModel.Declassements.MouvementId).ToList();

                Declassement declassement = _db.Declassements.FirstOrDefault(d => d.MouvementId == declassementViewModel.Declassements.MouvementId);
                if (declassement is not null)
                {
                    if (ModelState.IsValid)
                    {
                        declassement.MotifDecaissement = declassementViewModel.Declassements.MotifDecaissement;
                        _db.Declassements.Update(declassement);
                        _db.SaveChanges();

                        if (detailsDeclassement is not null)
                        {
                            foreach (var dc in detailsDeclassement)
                            {
                                dc.DeclassementId = declassement.MouvementId;
                                dc.MotifDeclassement = declassement.MotifDecaissement;
                                _db.DetailsDeclassements.UpdateRange(dc);
                                _db.SaveChanges();

                            }


                        }
                        TempData["success"] = "Declassement modifié.";
                        return RedirectToAction("Index");
                    }
                }
                else
                {
                    
                TempData["erreur"] = "Demmande non modifié.";
                return RedirectToAction("Index");
                }
                    
            }
            catch (Exception ex)
            {
                Console.Write("Une erreur s'est produite : " + ex);
                TempData["erreur"] = "Demmande non modifié.";
                return RedirectToAction("Index");
            }

            return View();
        }
        public IActionResult Delete(int declassementId)
        {
            Declassement? declassement = _db.Declassements.FirstOrDefault(d => d.MouvementId == declassementId);
            if (declassement is null)
            {
                RedirectToAction("Error", "Home");
            }

            DeclassementViewModel declassementViewModel = new DeclassementViewModel();

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

        [HttpPost]
        public IActionResult Delete(DeclassementViewModel declassementViewModel)
        {
            var detailsDeclassement = _db.DetailsDeclassements.Where(dc => dc.DeclassementId == declassementViewModel.Declassements.MouvementId).ToList();

            Declassement declassement = _db.Declassements.FirstOrDefault(d => d.MouvementId == declassementViewModel.Declassements.MouvementId);

            try
            {
                if(declassement is not null)
                {
                    _db.Declassements.Remove(declassement);
                    _db.SaveChanges();

                    if (detailsDeclassement is not null)
                    {
                        foreach (var dc in detailsDeclassement)
                        {
                            _db.DetailsDeclassements.RemoveRange(dc);
                            _db.SaveChanges();

                        }

                    }
                    TempData["success"] = "Declassement supprimé.";
                    return RedirectToAction("Index");
                }
                else
                {

                    TempData["erreur"] = "Demmande non supprimé.";
                    return RedirectToAction("Index");
                }
            }
            catch (Exception ex)
            {
                Console.Write("Une erreur s'est produite : " + ex);
                TempData["erreur"] = "Demmande non supprimé.";
                return RedirectToAction("Index");
            }
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
        public IActionResult Associer(int declassementId)
        {

            //Declassement? declassement = _db.Declassements.Include(d=>d.Mouvoirs).Include(d=>d.Bougers).Where(d => d.MouvementId == declassementId && (d.Mouvoirs.Count() == 0 && d.Bougers.Count() ==0)).FirstOrDefault();
            Declassement? declassement = _db.Declassements.Include(d=>d.Mouvoirs).Include(d=>d.Bougers).Where(d => d.MouvementId == declassementId).FirstOrDefault();

            DeclassementViewModel declassementViewModel = new DeclassementViewModel();
            if (declassement is null)
            {
                return RedirectToAction("Error", "Home");
            }

            declassementViewModel.Bouger = _db.Bougers
               .Include(b => b.Mouvement)
               .Include(b => b.Boisson)
               .ToList()
               .Where(m => m.MouvementId == declassementId);

            declassementViewModel.Mouvoir = _db.Mouvoirs
                .Include(m => m.Mouvement)
                .Include(m => m.Aliment)
                .ToList()
                .Where(m => m.MouvementId == declassementId);

            declassementViewModel.Boisson = _db.Boissons
                .Include(b => b.CategorieBoisson)
                .Where(b => b.QuantiteStock > 0 &&
                             !_db.Bougers.Any(c => c.BoissonId == b.BoissonId && c.MouvementId == declassementId))
                .OrderBy(b => b.TitreBoisson)
                .ToList();

            declassementViewModel.Aliment = _db.Aliments
                .Where(a => a.QuantiteStock > 0 &&
                             !_db.Mouvoirs.Any(f => f.AlimentId == a.AlimentId && f.MouvementId == declassementId))
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
        public IActionResult AssocierProduits([FromBody] AssociationProduits data, int declassementId)
        {
            try
            {
                DetailsDeclassement detailsDeclassementcnull = _db.DetailsDeclassements.Where(dc => dc.DeclassementId == declassementId && dc.ProduitDeclasse.ToUpper() == "NULL").FirstOrDefault();
                DetailsDeclassement detailsDeclassementexist = _db.DetailsDeclassements.FirstOrDefault(dc => dc.DeclassementId == declassementId);

                // Traitement pour les aliments
                if (data.AlimentIds != null && data.AlimentIds.Any())
                {
                    foreach (var aliment in data.AlimentIds)
                    {
                        // Récupérer l'aliment de la base de données pour vérifier le stock
                        var alimentEntity = _db.Aliments
                            .Where(a => a.AlimentId == aliment.Id)
                            .FirstOrDefault();

                        // Vérifier si l'aliment existe
                        if (alimentEntity == null)
                        {
                            return Json(new { success = false, message = $"L'aliment {alimentEntity.TitreAliment} n'existe pas." });
                        }

                        // Vérifier si la quantité demandée est supérieure au stock
                        if (aliment.Quantite > alimentEntity.QuantiteStock)
                        {
                            return Json(new { success = false, message = $"La quantité demandée pour {alimentEntity.TitreAliment} dépasse le stock disponible." });
                        }

                        Mouvoir mouvoir = new()
                        {
                            AlimentId = aliment.Id,
                            MouvementId = declassementId,
                            QteMouv = aliment.Quantite
                        };
                        _db.Mouvoirs.Add(mouvoir);

                        // Mettre à jour le stock
                        alimentEntity.QuantiteStock -= aliment.Quantite; // Diminuer le stock
                        DetailsDeclassement detailsDeclassement = new DetailsDeclassement()
                        {
                            Agent = detailsDeclassementexist.Agent,
                            DeclassementId = detailsDeclassementexist.DeclassementId,
                            DateMouvement = detailsDeclassementexist.DateMouvement,
                            HeureMouvement = detailsDeclassementexist.HeureMouvement,
                            NatureMouvement = detailsDeclassementexist.NatureMouvement,
                            MotifDeclassement = detailsDeclassementexist.MotifDeclassement,
                            TypeMouvement = detailsDeclassementexist.TypeMouvement,
                            ProduitDeclasse = TrouverDonneesBydata(mouvoir.AlimentId, "Aliment").ToString(),
                            QteDeclasse = mouvoir.QteMouv
                        };
                        _db.DetailsDeclassements.Add(detailsDeclassement);
                    }
                    _db.SaveChanges();

                }

                // Traitement pour les boissons
                if (data.BoissonIds != null && data.BoissonIds.Any())
                {
                    foreach (var boisson in data.BoissonIds)
                    {
                        // Récupérer la boisson de la base de données pour vérifier le stock
                        var boissonEntity = _db.Boissons
                            .Where(b => b.BoissonId == boisson.Id)
                            .FirstOrDefault();

                        // Vérifier si la boisson existe
                        if (boissonEntity == null)
                        {
                            return Json(new { success = false, message = $"La boisson {boissonEntity.TitreBoisson} n'existe pas." });
                        }

                        // Vérifier si la quantité demandée est supérieure au stock
                        if (boisson.Quantite > boissonEntity.QuantiteStock)
                        {
                            return Json(new { success = false, message = $"La quantité dépasse le stock disponible pour la boisson {boissonEntity.TitreBoisson}." });
                        }

                        Bouger bouger = new()
                        {
                            BoissonId = boisson.Id,
                            MouvementId = declassementId,
                            QteBouger = boisson.Quantite
                        };
                        _db.Bougers.Add(bouger);

                        // Mettre à jour le stock
                        boissonEntity.QuantiteStock -= boisson.Quantite; // Diminuer le stock

                        DetailsDeclassement detailsDeclassement = new DetailsDeclassement()
                        {
                            Agent = detailsDeclassementexist.Agent,
                            DeclassementId = detailsDeclassementexist.DeclassementId,
                            DateMouvement = detailsDeclassementexist.DateMouvement,
                            HeureMouvement = detailsDeclassementexist.HeureMouvement,
                            NatureMouvement = detailsDeclassementexist.NatureMouvement,
                            MotifDeclassement = detailsDeclassementexist.MotifDeclassement,
                            TypeMouvement = detailsDeclassementexist.TypeMouvement,
                            ProduitDeclasse = TrouverDonneesBydata(bouger.BoissonId, "Boisson").ToString(),
                            QteDeclasse = bouger.QteBouger
                        };
                        _db.DetailsDeclassements.Add(detailsDeclassement);
                    }
                    _db.SaveChanges();
                }


                if (detailsDeclassementcnull is not null)
                {
                    _db.DetailsDeclassements.Remove(detailsDeclassementcnull);
                    _db.SaveChanges();

                }
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
            public int declassementId { get; set; }
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

                Mouvoir mouvoir = null;
                Bouger bouger = null;

                if (aliment is not null)
                {
                    mouvoir = _db.Mouvoirs.FirstOrDefault(c => c.AlimentId == aliment.AlimentId && c.MouvementId == data.declassementId);
                }
                else if (boisson is not null)
                {
                    bouger = _db.Bougers.FirstOrDefault(c => c.BoissonId == boisson.BoissonId && c.MouvementId == data.declassementId);
                }

                if (mouvoir is not null)
                {
                    DetailsDeclassement detailsDeclassement = _db.DetailsDeclassements.FirstOrDefault(dc => dc.DeclassementId == mouvoir.MouvementId && dc.ProduitDeclasse == TrouverDonneesBydata(mouvoir.AlimentId, "Aliment"));

                    // Récupérer la quantité dissociée
                    int quantiteDissociee = mouvoir.QteMouv;

                    // Retirer l'entrée de Mouvoir
                    _db.Mouvoirs.Remove(mouvoir);

                    // Mettre à jour le stock
                    aliment.QuantiteStock += quantiteDissociee;
                    if (detailsDeclassement is not null)
                    {
                        _db.DetailsDeclassements.Remove(detailsDeclassement);

                    }
                    _db.SaveChanges();

                    return Json(new { success = true, message = "Aliment reclassé avec succès" });
                }
                else if (bouger is not null)
                {
                    DetailsDeclassement detailsDeclassement = _db.DetailsDeclassements.FirstOrDefault(dc => dc.DeclassementId == bouger.MouvementId && dc.ProduitDeclasse == TrouverDonneesBydata(bouger.BoissonId, "Boisson"));

                    // Récupérer la quantité dissociée
                    int quantiteDissociee = bouger.QteBouger;

                    // Retirer l'entrée de Bouger
                    _db.Bougers.Remove(bouger);

                    // Mettre à jour le stock
                    boisson.QuantiteStock += quantiteDissociee;

                    if (detailsDeclassement is not null)
                    {
                        _db.DetailsDeclassements.Remove(detailsDeclassement);

                    }
                    _db.SaveChanges();

                    return Json(new { success = true, message = "Boisson reclassé avec succès" });
                }
                else
                {
                    return Json(new { success = false, message = "Declassement introuvable" });
                }
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }


    }
}
