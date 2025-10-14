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
    [Authorize(Roles = "ADMIN, SUPERVISEUR,RECEPTIONNISTE")]
    [CaisseRedirectFilter]

    public class ChambreController : Controller
    {
        private readonly ApplicationDbContext _db;
        
        public ChambreController(ApplicationDbContext db)
        {
            _db = db;
        }

        public IActionResult Index(int page = 1, int size = 20,string searchString="")
        {
            int totalclients = _db.Chambres.Count();
            int skipValue = (page - 1) * size;
            int totalPages = (int)Math.Ceiling((double)totalclients / size);
            var chambres = _db.Chambres.OrderByDescending(c=>c.ChambreId).ToList().Skip(skipValue).Take(size);
            var depart = (totalPages - 1) * skipValue + 1;
            ViewBag.NumAff = depart;
            ViewBag.CurrentPage = page;
            ViewBag.PageSize = size;
            ViewBag.TotalPage = totalPages;
            if (!string.IsNullOrEmpty(searchString))
            {
                chambres = _db.Chambres.Where(c => c.NumeroChambre.ToString()!.ToUpper().Contains(searchString.ToUpper())).OrderByDescending(c => c.ChambreId).Skip(skipValue).Take(size).ToList();
            }
            ChambreViewModel chambreViewModel = new()
            {
                Chambre = chambres
            };
            return View(chambreViewModel);
        }

        [Authorize]
        [Authorize(Roles = "ADMIN, SUPERVISEUR")]
        public IActionResult Create()
        {
            return View();
        }

        [Authorize]
        [Authorize(Roles = "ADMIN, SUPERVISEUR")]
        [HttpPost]
        public IActionResult Create(Chambre chambre)
        {
            
            if (ModelState.IsValid)
            {
                //Verification doublon numero chambre
                int doubl_chambre = _db.Chambres.Count(c => c.NumeroChambre == chambre.NumeroChambre);
                if (doubl_chambre == 0)
                {
                    _db.Add(chambre);
                    _db.SaveChanges();
                    TempData["success"] = "Chambre créee.";
                    return RedirectToAction("Index");
                }
                else
                {
                    ModelState.AddModelError("numeroChambre", "Le numéro de chambre a déjà été attribué.");
                }
            }
            return View();
        }

        [Authorize]
        [Authorize(Roles = "ADMIN, SUPERVISEUR")]
        public IActionResult Update(int chambreId)
        {
            Chambre? chambre = _db.Chambres.FirstOrDefault(a => a.ChambreId == chambreId);
            if(chambre is null)
            {
                return RedirectToAction("Error", "Home");
            }
            return View(chambre);
        }

        [Authorize]
        [Authorize(Roles = "ADMIN, SUPERVISEUR")]
        public IActionResult Details(int chambreId)
        {
            Chambre? chambre = _db.Chambres.FirstOrDefault(a => a.ChambreId == chambreId);
            if (chambre is null)
            {
                return RedirectToAction("Error", "Home");
            }
            return View(chambre);
        }

        [Authorize]
        [Authorize(Roles = "ADMIN, SUPERVISEUR")]
        [HttpPost]
        public IActionResult Update(Chambre chambre)
        {
            if(ModelState.IsValid && chambre.ChambreId > 0)
            {
                int doubl_chambre = _db.Chambres.Count(c => c.NumeroChambre == chambre.NumeroChambre);
                if (doubl_chambre == 1)
                {
                    _db.Update(chambre);
                    _db.SaveChanges();
                    TempData["success"] = "Chambre mis à jour.";
                    return RedirectToAction("Index");
                }
                else
                {
                    ModelState.AddModelError("numeroChambre", "Le numéro de chambre a déjà été attribué.");
                }
            }
            return View();
        }

        [Authorize]
        [Authorize(Roles = "ADMIN, SUPERVISEUR")]
        public IActionResult Delete(int chambreId)
        {
            Chambre? chambre = _db.Chambres.FirstOrDefault(a => a.ChambreId == chambreId);
            if (chambre is null)
            {
                return RedirectToAction("Error", "Home");
            }
            return View(chambre);
        }

        [Authorize]
        [Authorize(Roles = "ADMIN, SUPERVISEUR")]
        [HttpPost]
        public IActionResult Delete(Chambre chambre)
        {
            
            try
            {
                Chambre? mychambre = _db.Chambres.FirstOrDefault(a => a.ChambreId == chambre.ChambreId);
                if (mychambre is not null)
                {
                    _db.Remove(mychambre);
                    _db.SaveChanges();
                    TempData["success"] = "Chambre Supprimé.";
                    return RedirectToAction("Index");
                }
                TempData["error"] = "Impossible de supprimer la chambre.";
            }
            catch
            {
                TempData["error"] = "Impossible de supprimer la chambre.";
            }
            return View();
        }
    }
}
