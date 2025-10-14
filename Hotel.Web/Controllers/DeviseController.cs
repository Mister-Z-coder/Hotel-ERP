using Hotel.Data;
using Hotel.Models;
using Hotel.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Hotel.Controllers
{
    [Authorize]
    [Authorize(Roles = "ADMIN,ADMINISTRATEUR")]
    [CaisseRedirectFilter]
    public class DeviseController : Controller
    {
        private readonly ApplicationDbContext _db;

        public DeviseController(ApplicationDbContext db)
        {
            _db = db;
        }

        public IActionResult Index(int page = 1, int size = 20)
        {
            int totaldevises = _db.Devises.Count();
            int skipValue = (page - 1) * size;
            int totalPages = (int)Math.Ceiling((double)totaldevises / size);
            var devises = _db.Devises.ToList().Skip(skipValue).OrderByDescending(d => d.DeviseId).Take(size);
            var depart = (totalPages - 1) * skipValue + 1;
            ViewBag.NumAff = depart;
            ViewBag.CurrentPage = page;
            ViewBag.PageSize = size;
            ViewBag.TotalPage = totalPages;
            return View(devises);
        }
        
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Create(Devise devise)
        {
            var deviseexist = _db.Devises.Where(d => d.TitreDevise == devise.TitreDevise || d.SigleDevise == devise.SigleDevise).FirstOrDefault(d => d.DeviseId != devise.DeviseId);

            if (ModelState.IsValid)
            {
                if (deviseexist is not null)
                {
                    ModelState.AddModelError("TitreDevise", "Cette devise existe déjà");
                    ModelState.AddModelError("SigleDevise", "Cette devise existe déjà");
                }
                if (ModelState.IsValid)
                {
                    _db.Devises.Add(devise);
                    _db.SaveChanges();
                    TempData["success"] = "Devise créee.";
                    return RedirectToAction("Index");
                }
            }
            return View();
        }

        public IActionResult Update(int deviseId)
        {
            Devise? devise = _db.Devises.FirstOrDefault(a => a.DeviseId == deviseId);
            if (devise is null)
            {
                return RedirectToAction("Error", "Home");
            }
            return View(devise);
        }
        public IActionResult Details(int deviseId)
        {
            Devise? devise = _db.Devises.FirstOrDefault(a => a.DeviseId == deviseId);
            if (devise is null)
            {
                return RedirectToAction("Error", "Home");
            }
            return View(devise);
        }

        [HttpPost]
        public IActionResult Update(Devise devise)
        {
            var deviseexist = _db.Devises.Where(d => d.TitreDevise == devise.TitreDevise || d.SigleDevise == devise.SigleDevise).FirstOrDefault(d => d.DeviseId != devise.DeviseId);

            if (ModelState.IsValid && devise.DeviseId > 0)
            {
                if(deviseexist is not null)
                {
                    ModelState.AddModelError("TitreDevise", "Cette devise existe déjà");
                    ModelState.AddModelError("SigleDevise", "Cette devise existe déjà");
                }
                if (ModelState.IsValid)
                {
                    _db.Devises.Update(devise);
                    _db.SaveChanges();
                    TempData["success"] = "Devise mise à jour.";
                    return RedirectToAction("Index");
                }
            }
            return View();
        }
        public IActionResult Delete(int deviseId)
        {
            Devise? devise = _db.Devises.FirstOrDefault(a => a.DeviseId == deviseId);
            if (devise is null)
            {
                return RedirectToAction("Error", "Home");
            }
            return View(devise);
        }

        [HttpPost]
        public IActionResult Delete(Devise devise)
        {
            try
            {
                Devise? mydevise = _db.Devises.FirstOrDefault(a => a.DeviseId == devise.DeviseId);
                if (mydevise is not null)
                {
                    _db.Devises.Remove(mydevise);
                    _db.SaveChanges();
                    TempData["success"] = "Devise supprimé.";
                    return RedirectToAction("Index");
                }
                TempData["error"] = "Impossible de supprimer la devise.";
            }
            catch
            {
                TempData["error"] = "Impossible de supprimer la devise.";
            }
            return View();
        }
    }
}
