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
    [Authorize(Roles = "ADMIN,GESTIONNAIRE")]
    [CaisseRedirectFilter]
    public class CategorieBoissonController : Controller
    {
        private readonly ApplicationDbContext _db;

        public CategorieBoissonController(ApplicationDbContext db)
        {
            _db = db;
        }

        public IActionResult Index(int page = 1, int size = 20, string searchString="")
        {
            int totalcatboisson = _db.CategorieBoissons.Count();
            int skipValue = (page - 1) * size;
            int totalPages = (int)Math.Ceiling((double)totalcatboisson / size);
            var catboisson = _db.CategorieBoissons.OrderByDescending(c=>c.CategorieBoissonId).Skip(skipValue).Take(size).ToList();
            var depart = (totalPages - 1) * skipValue + 1;
            ViewBag.NumAff = depart;
            ViewBag.CurrentPage = page;
            ViewBag.PageSize = size;
            ViewBag.TotalPage = totalPages;
            if (!string.IsNullOrEmpty(searchString))
            {
                catboisson = _db.CategorieBoissons.Where(c => c.TitreCategorieBoisson!.ToUpper().Contains(searchString.ToUpper())).Skip(skipValue).OrderByDescending(c => c.CategorieBoissonId).Take(size).ToList();
            }
            CategorieBoissonViewModel categorieBoissonViewModel = new()
            {
                CategorieBoisson = catboisson
            };
            return View(categorieBoissonViewModel);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Create(CategorieBoisson catboisson)
        {
            CategorieBoisson? exist = _db.CategorieBoissons.FirstOrDefault(t => t.TitreCategorieBoisson == catboisson.TitreCategorieBoisson);
            if (ModelState.IsValid)
            {
                if (exist is not null)
                {
                    ModelState.AddModelError("TitreCategorieBoisson", "Ce type de reglement existe déjà");
                }
                if (ModelState.IsValid)
                {
                    _db.CategorieBoissons.Add(catboisson);
                    _db.SaveChanges();
                    TempData["success"] = "Categorie boisson créee.";
                    return RedirectToAction("Index");
                }
            }

            return View();
        }

        public IActionResult Update(int catboissonId)
        {
            CategorieBoisson? catboisson = _db.CategorieBoissons.FirstOrDefault(a => a.CategorieBoissonId == catboissonId);

            if (catboisson is null)
            {
                return RedirectToAction("Error", "Home");
            }
            return View(catboisson);

        }
        public IActionResult Details(int catboissonId)
        {
            CategorieBoisson? catboisson = _db.CategorieBoissons.FirstOrDefault(a => a.CategorieBoissonId == catboissonId);

            if (catboisson is null)
            {
                return RedirectToAction("Error", "Home");
            }
            return View(catboisson);

        }
        [HttpPost]
        public IActionResult Update(CategorieBoisson catboisson)
        {
            CategorieBoisson? exist = _db.CategorieBoissons.Where(t => t.CategorieBoissonId != catboisson.CategorieBoissonId).FirstOrDefault(t => t.TitreCategorieBoisson == catboisson.TitreCategorieBoisson);
            if (ModelState.IsValid)
            {
                if (exist is not null)
                {
                    ModelState.AddModelError("TitreCategorieBoisson", "Cette Categorie boisson existe déjà");
                }
                if (ModelState.IsValid && catboisson.CategorieBoissonId > 0)
                {
                    _db.CategorieBoissons.Update(catboisson);
                    _db.SaveChanges();
                    TempData["success"] = "Categorie boisson mise à jour.";
                    return RedirectToAction("Index");
                }
            }


            return View(catboisson);
        }
        public IActionResult Delete(int catboissonId)
        {
            CategorieBoisson? catboisson = _db.CategorieBoissons.FirstOrDefault(a => a.CategorieBoissonId == catboissonId);

            if (catboisson is null)
            {
                return RedirectToAction("Error", "Home");
            }
            return View(catboisson);
        }

        [HttpPost]
        public IActionResult Delete(CategorieBoisson catboisson)
        {
            try
            {
                CategorieBoisson? mycatboisson = _db.CategorieBoissons.FirstOrDefault(a => a.CategorieBoissonId == catboisson.CategorieBoissonId);
                if (mycatboisson is not null)
                {
                    _db.CategorieBoissons.Remove(mycatboisson);
                    _db.SaveChanges();
                    TempData["success"] = "Categorie boisson supprimée.";
                    return RedirectToAction("Index");
                }
                TempData["error"] = "Impossible de supprimer la Categorie boisson.";
            }
            catch
            {
                TempData["error"] = "Impossible de supprimer la Categorie boisson.";
            }
            return View(catboisson);
        }
    }
}
