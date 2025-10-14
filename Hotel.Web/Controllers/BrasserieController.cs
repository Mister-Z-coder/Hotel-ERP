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
    public class BrasserieController : Controller
    {
        private readonly ApplicationDbContext _db;

        public BrasserieController(ApplicationDbContext db)
        {
            _db = db;
        }

        public IActionResult Index(int page = 1, int size = 20, string searchString="")
        {
            int totalbrasseries = _db.Brasseries.Count();
            int skipValue = (page - 1) * size;
            int totalPages = (int)Math.Ceiling((double)totalbrasseries / size);
            var brasserie = _db.Brasseries.OrderByDescending(b=>b.BrasserieId).Skip(skipValue).Take(size).ToList();
            var depart = (totalPages - 1) * skipValue + 1;
            ViewBag.NumAff = depart;
            ViewBag.CurrentPage = page;
            ViewBag.PageSize = size;
            ViewBag.TotalPage = totalPages;
            if (!string.IsNullOrEmpty(searchString))
            {
                brasserie = _db.Brasseries.Where(b => b.TitreBrasserie!.ToUpper().Contains(searchString.ToUpper())).OrderByDescending(b => b.BrasserieId).Skip(skipValue).Take(size).ToList();
            }
            BrasserieViewModel brasserieViewModel = new()
            {
                Brasserie = brasserie
            };
            return View(brasserieViewModel);
        }
        
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Create(Brasserie brasserie)
        {
            Brasserie? exist = _db.Brasseries.FirstOrDefault(t=>t.TitreBrasserie == brasserie.TitreBrasserie);
            if (ModelState.IsValid)
            {
                if(exist is not null)
                {
                    ModelState.AddModelError("TitreBrasserie", "Cette brasserie existe déjà");
                }
                if(ModelState.IsValid)
                {
                    _db.Brasseries.Add(brasserie);
                    _db.SaveChanges();
                    TempData["success"] = "Brasserie créee.";
                    return RedirectToAction("Index");
                }
            }

            return View();
        }

        public IActionResult Update(int brasserieId)
        {
            Brasserie? brasserie = _db.Brasseries.FirstOrDefault(a => a.BrasserieId == brasserieId);
           
            if (brasserie is null)
            {
                return RedirectToAction("Error", "Home");
            }
            return View(brasserie);

        }

        public IActionResult Details(int brasserieId)
        {
            Brasserie? brasserie = _db.Brasseries.FirstOrDefault(a => a.BrasserieId == brasserieId);

            if (brasserie is null)
            {
                return RedirectToAction("Error", "Home");
            }
            return View(brasserie);

        }

        [HttpPost]
        public IActionResult Update(Brasserie brasserie)
        {
            Brasserie? exist = _db.Brasseries.Where(t=>t.BrasserieId!=brasserie.BrasserieId).FirstOrDefault(t => t.TitreBrasserie == brasserie.TitreBrasserie);
            if (ModelState.IsValid)
            {
                if (exist is not null)
                {
                    ModelState.AddModelError("TitreBrasserie", "Ce type de reglement existe déjà");
                }
                if (ModelState.IsValid && brasserie.BrasserieId > 0)
                {
                    _db.Brasseries.Update(brasserie);
                    _db.SaveChanges();
                    TempData["success"] = "Brasserie mise à jour.";
                    return RedirectToAction("Index");
                }
            }

            
            return View(brasserie);
        }
        public IActionResult Delete(int brasserieId)
        {
            Brasserie? brasserie = _db.Brasseries.FirstOrDefault(a => a.BrasserieId == brasserieId);

            if (brasserie is null)
            {
                return RedirectToAction("Error", "Home");
            }
            return View(brasserie);
        }

        [HttpPost]
        public IActionResult Delete(Brasserie brasserie)
        {
            try
            {
                Brasserie? mybrasserie = _db.Brasseries.FirstOrDefault(a => a.BrasserieId == brasserie.BrasserieId);
                if (mybrasserie is not null)
                {
                    _db.Brasseries.Remove(mybrasserie);
                    _db.SaveChanges();
                    TempData["success"] = "Brasserie supprimé.";
                    return RedirectToAction("Index");
                }
                TempData["error"] = "Impossible de supprimer la brasserie.";
            }
            catch
            {
                TempData["error"] = "Impossible de supprimer la brasserie.";
            }
            return View(brasserie);
        }
    }
}
