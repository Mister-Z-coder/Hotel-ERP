using Hotel.Data;
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
using System.Threading.Tasks;

namespace Hotel.Controllers
{

    [Authorize]
    [Authorize(Roles = "ADMIN,ADMINISTRATEUR")]
    [CaisseRedirectFilter]
    public class FournisseurController : Controller
    {
        private readonly ApplicationDbContext _db;

        public FournisseurController(ApplicationDbContext db)
        {
            _db = db;
        }

        public IActionResult Index(int page = 1, int size = 20, string searchString = "")
        {
            int totalfournisseurs = _db.Fournisseurs.Count();
            int skipValue = (page - 1) * size;
            int totalPages = (int)Math.Ceiling((double)totalfournisseurs / size);
            var fournisseurs = _db.Fournisseurs.ToList().OrderByDescending(f => f.FournisseurId).Skip(skipValue).Take(size);
            var depart = (totalPages - 1) * skipValue + 1;
            ViewBag.NumAff = depart;
            ViewBag.CurrentPage = page;
            ViewBag.PageSize = size;
            ViewBag.TotalPage = totalPages;
            if (!string.IsNullOrEmpty(searchString))
            {
                fournisseurs = _db.Fournisseurs.Where(f => f.NomFournisseur!.ToUpper().Contains(searchString.ToUpper())).ToList().Skip(skipValue).Take(size);
            }
            FournisseurViewModel fournisseurViewModel = new()
            {
                Fournisseur = fournisseurs
            };
            return View(fournisseurViewModel);
        }
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Create(Fournisseur fournisseur)
        {
            Fournisseur existfournisseur = _db.Fournisseurs.Where(a => a.NomFournisseur == fournisseur.NomFournisseur).FirstOrDefault(a => a.FournisseurId != fournisseur.FournisseurId);
            if (ModelState.IsValid)
            {
                if (existfournisseur is not null)
                {
                    ModelState.AddModelError("NomFournisseur", "Il existe déjà un fournisseur répondant à ce nom.");
                }
                if (ModelState.IsValid)
                {
                    _db.Fournisseurs.Add(fournisseur);
                    _db.SaveChanges();
                    TempData["success"] = "Fournisseur crée.";
                    return RedirectToAction("Index");
                }
            }
            return View();
        }
        public IActionResult Details(int fournisseurId)
        {
            Fournisseur? fournisseur = _db.Fournisseurs.FirstOrDefault(a => a.FournisseurId == fournisseurId);
            FournirViewModel fournisseurViewModel = new FournirViewModel();
            //var fournir = _db.Fournirs.ToList().FirstOrDefault(f => f.FournisseurId == fournisseurId && f.Fournisseur.DomaineActivite.Contains("ALIMENT"));

            //fournisseurViewModel.Fournisseur = _db.Fournisseurs.FirstOrDefault(f => f.FournisseurId == fournisseurId);

            if (fournisseur is null)
            {
                return RedirectToAction("Error", "Home");
            }
            //fournisseurViewModel.FournirList = _db.Fournirs
            //    .Include(f => f.Fournisseur)
            //    .Include(f => f.Aliment)
            //    .ToList()
            //    .Where(f=>f.FournisseurId==fournisseurId)
            //    .Select(f => new SelectListItem
            //    {
            //        Text = $"{f.Aliment.TitreAliment}",
            //        Value = f.AlimentId.ToString()
            //    });
            //
            fournisseurViewModel.Fournir = _db.Fournirs
                .Include(f => f.Fournisseur)
                .Include(f => f.Aliment)
                .ToList()
                .Where(f => f.FournisseurId == fournisseurId);

            fournisseurViewModel.Procurer = _db.Procurers
                .Include(f => f.Fournisseur)
                .Include(f => f.Boisson)
                .ToList()
                .Where(f => f.FournisseurId == fournisseurId);
            //
            //fournisseurViewModel.ProcurerList = _db.Procurers
            //    .Include(f => f.Fournisseur)
            //    .Include(f => f.Boisson)
            //    .ToList()
            //    .Where(f => f.FournisseurId == fournisseurId)
            //    .Select(f => new SelectListItem
            //    {
            //        Text = $"{f.Boisson.TitreBoisson}",
            //        Value = f.BoissonId.ToString()
            //    });
            //fournisseurViewModel.BoissonList = _db.Boissons.Select(b => new SelectListItem
            //{
            //    Text = b.TitreBoisson,
            //    Value = b.BoissonId.ToString()
            //});
            //fournisseurViewModel.AlimentList = _db.Aliments.Select(b => new SelectListItem
            //{
            //    Text = b.TitreAliment,
            //    Value = b.AlimentId.ToString()
            //});
            //fournisseurViewModel.Boisson = _db.Boissons.ToList();
            //var procurer = _db.Procurers.ToList();
            fournisseurViewModel.Boisson = _db.Boissons
                .Include(b => b.CategorieBoisson)
                .Where(b => !_db.Procurers
                .Any(p => p.BoissonId == b.BoissonId && p.FournisseurId == fournisseurId))
                .OrderBy(b => b.TitreBoisson)
                .ToList();

            fournisseurViewModel.Aliment = _db.Aliments
                .Where(a => !_db.Fournirs
                .Any(f => f.AlimentId == a.AlimentId && f.FournisseurId == fournisseurId))
                .ToList();

            fournisseurViewModel.Fournisseur = fournisseur;
            return View(fournisseurViewModel);
        }

        public IActionResult Associer(int fournisseurId)
        {
            Fournisseur? fournisseur = _db.Fournisseurs.FirstOrDefault(a => a.FournisseurId == fournisseurId);
            FournirViewModel fournisseurViewModel = new FournirViewModel();
            //var fournir = _db.Fournirs.ToList().FirstOrDefault(f => f.FournisseurId == fournisseurId && f.Fournisseur.DomaineActivite.Contains("ALIMENT"));

            //fournisseurViewModel.Fournisseur = _db.Fournisseurs.FirstOrDefault(f => f.FournisseurId == fournisseurId);

            if (fournisseur is null)
            {
                return RedirectToAction("Error", "Home");
            }
            //fournisseurViewModel.FournirList = _db.Fournirs
            //    .Include(f => f.Fournisseur)
            //    .Include(f => f.Aliment)
            //    .ToList()
            //    .Where(f=>f.FournisseurId==fournisseurId)
            //    .Select(f => new SelectListItem
            //    {
            //        Text = $"{f.Aliment.TitreAliment}",
            //        Value = f.AlimentId.ToString()
            //    });
            //
            fournisseurViewModel.Fournir = _db.Fournirs
                .Include(f => f.Fournisseur)
                .Include(f => f.Aliment)
                .ToList()
                .Where(f => f.FournisseurId == fournisseurId);

            fournisseurViewModel.Procurer = _db.Procurers
                .Include(f => f.Fournisseur)
                .Include(f => f.Boisson)
                .ToList()
                .Where(f => f.FournisseurId == fournisseurId);
            //
            //fournisseurViewModel.ProcurerList = _db.Procurers
            //    .Include(f => f.Fournisseur)
            //    .Include(f => f.Boisson)
            //    .ToList()
            //    .Where(f => f.FournisseurId == fournisseurId)
            //    .Select(f => new SelectListItem
            //    {
            //        Text = $"{f.Boisson.TitreBoisson}",
            //        Value = f.BoissonId.ToString()
            //    });
            //fournisseurViewModel.BoissonList = _db.Boissons.Select(b => new SelectListItem
            //{
            //    Text = b.TitreBoisson,
            //    Value = b.BoissonId.ToString()
            //});
            //fournisseurViewModel.AlimentList = _db.Aliments.Select(b => new SelectListItem
            //{
            //    Text = b.TitreAliment,
            //    Value = b.AlimentId.ToString()
            //});
            //fournisseurViewModel.Boisson = _db.Boissons.ToList();
            //var procurer = _db.Procurers.ToList();
            fournisseurViewModel.Boisson = _db.Boissons
                .Include(b => b.CategorieBoisson)
                .Where(b => !_db.Procurers
                .Any(p => p.BoissonId == b.BoissonId && p.FournisseurId == fournisseurId))
                .OrderBy(b => b.TitreBoisson)
                .ToList();

            fournisseurViewModel.Aliment = _db.Aliments
                .Where(a => !_db.Fournirs
                .Any(f => f.AlimentId == a.AlimentId && f.FournisseurId == fournisseurId))
                .ToList();

            fournisseurViewModel.Fournisseur = fournisseur;
            return View(fournisseurViewModel);
        }

        public IActionResult Update(int fournisseurId)
        {
            Fournisseur? fournisseur = _db.Fournisseurs.FirstOrDefault(a => a.FournisseurId == fournisseurId);
            if (fournisseur is null)
            {
                return RedirectToAction("Error", "Home");
            }
            return View(fournisseur);
        }

        [HttpPost]
        public IActionResult Update(Fournisseur fournisseur)
        {
            if (ModelState.IsValid && fournisseur.FournisseurId > 0)
            {
                Fournisseur existfournisseur = _db.Fournisseurs.Where(a => a.NomFournisseur == fournisseur.NomFournisseur).FirstOrDefault(a => a.FournisseurId != fournisseur.FournisseurId);

                if (existfournisseur is not null)
                {
                    ModelState.AddModelError("NomFournisseur", "Il existe déjà un fournisseur répondant à ce nom.");
                }
                if (ModelState.IsValid)
                {

                    _db.Fournisseurs.Update(fournisseur);
                    _db.SaveChanges();
                    TempData["success"] = "Fournisseur mis à jour.";
                    return RedirectToAction("Index");
                }
            }
            return View();
        }
        public IActionResult Delete(int fournisseurId)
        {
            Fournisseur? fournisseur = _db.Fournisseurs.FirstOrDefault(a => a.FournisseurId == fournisseurId);
            if (fournisseur is null)
            {
                return RedirectToAction("Error", "Home");
            }
            return View(fournisseur);
        }

        [HttpPost]
        public IActionResult Delete(Fournisseur fournisseur)
        {
            try
            {
                Fournisseur? myfournisseur = _db.Fournisseurs.FirstOrDefault(a => a.FournisseurId == fournisseur.FournisseurId);
                if (myfournisseur is not null)
                {
                    _db.Fournisseurs.Remove(myfournisseur);
                    _db.SaveChanges();
                    TempData["success"] = "Fournisseur Supprimé.";
                    return RedirectToAction("Index");
                }
                TempData["error"] = "Impossible de supprimer le fournisseur.";
            }
            catch
            {
                TempData["error"] = "Impossible de supprimer le fournisseur.";
            }
            return View();
        }
        public class AssociationProduits
        {
            public List<int> AlimentIds { get; set; }
            public List<int> BoissonIds { get; set; }
        }
        
        [HttpPost]
        public IActionResult AssocierProduits([FromBody] AssociationProduits data, int fournisseurId)
        {
            try
            {
                // Traitement pour les aliments
                if (data.AlimentIds != null && data.AlimentIds.Any())
                {
                    foreach (var alimentId in data.AlimentIds)
                    {
                        Fournir fournir = new()
                        {
                            AlimentId = alimentId,
                            FournisseurId = fournisseurId
                        };
                        _db.Fournirs.Add(fournir);
                    }
                }

                // Traitement pour les boissons
                if (data.BoissonIds != null && data.BoissonIds.Any())
                {
                    foreach (var boissonId in data.BoissonIds)
                    {
                        Procurer procurer = new()
                        {
                            BoissonId = boissonId,
                            FournisseurId = fournisseurId
                        };
                        _db.Procurers.Add(procurer);
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

        public class DissociationProduits
        {
            public int produitId { get; set; }
            public int fournisseurId { get; set; }
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

                if (aliment != null)
                {
                    Fournir fournir = _db.Fournirs.FirstOrDefault(f => f.AlimentId == aliment.AlimentId && f.FournisseurId == data.fournisseurId);
                    if (fournir != null)
                    {
                        _db.Fournirs.Remove(fournir);
                        _db.SaveChanges();
                        return Json(new { success = true, message = "Aliment dissocié avec succès" });
                    }
                }
                else if (boisson != null)
                {
                    Procurer procurer = _db.Procurers.FirstOrDefault(p => p.BoissonId == boisson.BoissonId && p.FournisseurId == data.fournisseurId);
                    if (procurer != null)
                    {
                        _db.Procurers.Remove(procurer);
                        _db.SaveChanges();
                        return Json(new { success = true, message = "Boisson dissociée avec succès" });
                    }
                }
                return Json(new { success = false, message = "Association introuvable" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }
    }
}
