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
    public class CaisseController : Controller
    {
        private readonly ApplicationDbContext _db;

        public CaisseController(ApplicationDbContext db)
        {
            _db = db;
        }

        public IActionResult Index(int page = 1, int size = 20)
        {
            int totalcaisse = _db.PosteTravails.OfType<Caisse>().Count();
            int skipValue = (page - 1) * size;
            int totalPages = (int)Math.Ceiling((double)totalcaisse / size);
            var caisse = _db.PosteTravails.OrderByDescending(c=>c.PosteTravailId).OfType<Caisse>().Skip(skipValue).Take(size).ToList();
            var depart = (totalPages - 1) * skipValue + 1;
            ViewBag.NumAff = depart;
            ViewBag.CurrentPage = page;
            ViewBag.PageSize = size;
            ViewBag.TotalPage = totalPages;

            return View(caisse);
        }
        
        public IActionResult Create()
        {
            return View();
        }


        public IActionResult Details(int postetravId)
        {
            Caisse caisse = _db.PosteTravails.OfType<Caisse>().FirstOrDefault(c => c.PosteTravailId == postetravId);

            if (caisse is null)
            {
                return RedirectToAction("Error", "Home");
            }

            return View(caisse);
        }
        public IActionResult Update(int postetravId)
        {
            Caisse caisse = _db.PosteTravails.OfType<Caisse>().FirstOrDefault(c => c.PosteTravailId == postetravId);

            if (caisse is null)
            {
                return RedirectToAction("Error", "Home");
            }
            
            return View(caisse);
        }//Vérifier le code

        [HttpPost]
        public IActionResult Update(Caisse caisse)
        {
            Caisse? existCaisse = _db.PosteTravails.OfType<Caisse>().Where(c => c.TitreCaisse == caisse.TitreCaisse && c.BarCaisse == caisse.BarCaisse).FirstOrDefault(c => c.PosteTravailId != caisse.PosteTravailId);
            
            if (ModelState.IsValid)
            {
                if (existCaisse is not null)
                {
                    ModelState.AddModelError("TitreCaisse", "Il existe déjà une caisse affectée à ce bar");
                    ModelState.AddModelError("BarCaisse", "Il existe déjà une caisse affectée à ce bar");
                }
                if (ModelState.IsValid)
                {
                    _db.Caisses.Update(caisse);
                    _db.SaveChanges();
                    TempData["success"] = "Caisse mise à jour.";
                    return RedirectToAction("Index");
                }
            }

            return View(caisse);
        }
        public IActionResult Delete(int postetravId)
        {
            Caisse caisse = _db.PosteTravails.OfType<Caisse>().FirstOrDefault(c => c.PosteTravailId == postetravId);


            if (caisse is null)
            {
                return RedirectToAction("Error", "Home");
            }
            return View(caisse);
        }

        [HttpPost]
        public IActionResult Delete(Caisse caisse)
        {
            try
            {
                Caisse mycaisse = _db.PosteTravails.OfType<Caisse>().FirstOrDefault(c => c.PosteTravailId == caisse.PosteTravailId);
                if (mycaisse is not null)
                {
                    _db.Caisses.Remove(mycaisse);
                    _db.SaveChanges();
                    TempData["success"] = "Caisse supprimée.";
                    return RedirectToAction("Index");
                }
                TempData["error"] = "Impossible de supprimer la caisse.";
            }
            catch(Exception ex)
            {
                Console.WriteLine("Mesage d'erreur :" + ex);
                TempData["error"] = "Impossible de supprimer la caisse. car elle a déjà été utilsiée";
            }
            return View();
        }
    }
}
