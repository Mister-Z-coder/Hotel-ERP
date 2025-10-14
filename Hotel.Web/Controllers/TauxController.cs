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
    public class TauxController : Controller
    {
        private readonly ApplicationDbContext _db;

        public TauxController(ApplicationDbContext db)
        {
            _db = db;
        }

        public IActionResult Index(int page = 1, int size = 20)
        {
            int totaltaux = _db.Tauxes.Count();
            int skipValue = (page - 1) * size;
            int totalPages = (int)Math.Ceiling((double)totaltaux / size);
            var taux = _db.Tauxes.Include(t=>t.Devise).Skip(skipValue).OrderByDescending(t=>t.TauxId).Take(size).ToList();
            var depart = (totalPages - 1) * skipValue + 1;
            ViewBag.NumAff = depart;
            ViewBag.CurrentPage = page;
            ViewBag.PageSize = size;
            ViewBag.TotalPage = totalPages;
            return View(taux);
        }
        
        public IActionResult Create()
        {
            TauxViewModel tauxViewModel = new()
            {
                DeviseList = _db.Devises.ToList().Select(d => new SelectListItem
                {
                    Text = $"{d.TitreDevise} | Sigle : {d.SigleDevise}",
                    Value = d.DeviseId.ToString()
                }).ToList(),
            };
            return View(tauxViewModel);
        }

        [HttpPost]
        public IActionResult Create(TauxViewModel tauxViewModel)
        {
            ModelState.Remove("Taux.DateTaux");
            tauxViewModel.Taux.DateTaux = DateTime.Today;
            if (ModelState.IsValid)
            {
                _db.Tauxes.Add(tauxViewModel.Taux);
                _db.SaveChanges();
                TempData["success"] = "Taux crée.";
                return RedirectToAction("Index");
            }
            tauxViewModel.DeviseList = _db.Devises.ToList().Select(d => new SelectListItem
            {
                Text = $"{d.TitreDevise} | Sigle : {d.SigleDevise}",
                Value = d.DeviseId.ToString()
            }).ToList();

            return View();
        }

        public IActionResult Update(int tauxId)
        {
            Taux? taux = _db.Tauxes.FirstOrDefault(a => a.TauxId == tauxId);
            TauxViewModel tauxViewModel = new TauxViewModel();
            tauxViewModel.DeviseList = _db.Devises.ToList().Select(d => new SelectListItem
            {
                Text = $"{d.TitreDevise} | Sigle : {d.SigleDevise}",
                Value = d.DeviseId.ToString()
            }).ToList();

            if (taux is not null)
            {
                tauxViewModel.Taux = taux;
            }
            else
            {
                return RedirectToAction("Error", "Home");
            }
            return View(tauxViewModel);
        }
        public IActionResult Details(int tauxId)
        {
            Taux? taux = _db.Tauxes.FirstOrDefault(a => a.TauxId == tauxId);
            TauxViewModel tauxViewModel = new TauxViewModel();
            tauxViewModel.DeviseList = _db.Devises.ToList().Select(d => new SelectListItem
            {
                Text = $"{d.TitreDevise} | Sigle : {d.SigleDevise}",
                Value = d.DeviseId.ToString()
            }).ToList();

            if (taux is not null)
            {
                tauxViewModel.Taux = taux;
            }
            else
            {
                return RedirectToAction("Error", "Home");
            }
            return View(tauxViewModel);
        }
        [HttpPost]
        public IActionResult Update(TauxViewModel tauxViewModel)
        {
            if (ModelState.IsValid && tauxViewModel.Taux.DeviseId > 0)
            {
                _db.Tauxes.Update(tauxViewModel.Taux);
                _db.SaveChanges();
                TempData["success"] = "Taux mis à jour.";
                return RedirectToAction("Index");
            }
            tauxViewModel.DeviseList = _db.Devises.ToList().Select(d => new SelectListItem
            {
                Text = $"{d.TitreDevise} | Sigle : {d.SigleDevise}",
                Value = d.DeviseId.ToString()
            }).ToList();
            return View(tauxViewModel);
        }
        public IActionResult Delete(int tauxId)
        {
            Taux? taux = _db.Tauxes.FirstOrDefault(a => a.TauxId == tauxId);
            TauxViewModel tauxViewModel = new TauxViewModel();
            tauxViewModel.DeviseList = _db.Devises.ToList().Select(d => new SelectListItem
            {
                Text = $"{d.TitreDevise} | Sigle : {d.SigleDevise}",
                Value = d.DeviseId.ToString()
            }).ToList();

            if (taux is not null)
            {
                tauxViewModel.Taux = taux;
            }
            else
            {
                return RedirectToAction("Error", "Home");
            }
            return View(tauxViewModel);
        }

        [HttpPost]
        public IActionResult Delete(TauxViewModel tauxViewModel)
        {
            try
            {
                Taux? mytaux = _db.Tauxes.FirstOrDefault(a => a.TauxId == tauxViewModel.Taux.TauxId);
                if (mytaux is not null)
                {
                    _db.Tauxes.Remove(mytaux);
                    _db.SaveChanges();
                    TempData["success"] = "Taux supprimé.";
                    return RedirectToAction("Index");
                }
                TempData["error"] = "Impossible de supprimer le taux.";
            }
            catch
            {
                TempData["error"] = "Impossible de supprimer le taux.";
            }
            tauxViewModel.DeviseList = _db.Devises.ToList().Select(d => new SelectListItem
            {
                Text = $"{d.TitreDevise} | Sigle : {d.SigleDevise}",
                Value = d.DeviseId.ToString()
            }).ToList();
            return View(tauxViewModel);
        }
    }
}
