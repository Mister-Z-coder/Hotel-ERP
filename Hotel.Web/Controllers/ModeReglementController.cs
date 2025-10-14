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
    public class ModeReglementController : Controller
    {
        private readonly ApplicationDbContext _db;

        public ModeReglementController(ApplicationDbContext db)
        {
            _db = db;
        }

        public IActionResult Index(int page = 1, int size = 20)
        {
            int totalmodereglement = _db.ModeReglements.Count();
            int skipValue = (page - 1) * size;
            int totalPages = (int)Math.Ceiling((double)totalmodereglement / size);
            var modereglement = _db.ModeReglements.Skip(skipValue).OrderByDescending(m => m.ModeReglementId).Take(size).ToList();
            var depart = (totalPages - 1) * skipValue + 1;
            ViewBag.NumAff = depart;
            ViewBag.CurrentPage = page;
            ViewBag.PageSize = size;
            ViewBag.TotalPage = totalPages;
            return View(modereglement);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Create(ModeReglement modeReglement)
        {
            ModeReglement? exist = _db.ModeReglements.FirstOrDefault(t => t.TitreModeReglement == modeReglement.TitreModeReglement);
            if (ModelState.IsValid)
            {
                if (exist is not null)
                {
                    ModelState.AddModelError("TitreModeReglement", "Ce mode de reglement existe déjà");
                }
                if (ModelState.IsValid)
                {
                    _db.ModeReglements.Add(modeReglement);
                    _db.SaveChanges();
                    TempData["success"] = "Type de reglement crée.";
                    return RedirectToAction("Index");
                }
            }

            return View();
        }
        public IActionResult Details(int modereglId)
        {
            ModeReglement? modeReglement = _db.ModeReglements.FirstOrDefault(a => a.ModeReglementId == modereglId);

            if (modeReglement is null)
            {
                return RedirectToAction("Error", "Home");
            }
            return View(modeReglement);

        }
        public IActionResult Update(int modereglId)
        {
            ModeReglement? modeReglement = _db.ModeReglements.FirstOrDefault(a => a.ModeReglementId == modereglId);

            if (modeReglement is null)
            {
                return RedirectToAction("Error", "Home");
            }
            return View(modeReglement);

        }

        [HttpPost]
        public IActionResult Update(ModeReglement modeReglement)
        {
            ModeReglement? exist = _db.ModeReglements.Where(t => t.ModeReglementId != modeReglement.ModeReglementId).FirstOrDefault(t => t.TitreModeReglement == modeReglement.TitreModeReglement);
            if (ModelState.IsValid)
            {
                if (exist is not null)
                {
                    ModelState.AddModelError("TitreModeReglement", "Ce mode de reglement existe déjà");
                }
                if (ModelState.IsValid && modeReglement.ModeReglementId > 0)
                {
                    _db.ModeReglements.Update(modeReglement);
                    _db.SaveChanges();
                    TempData["success"] = "Mode reglement mis à jour.";
                    return RedirectToAction("Index");
                }
            }


            return View(modeReglement);
        }
        public IActionResult Delete(int modereglId)
        {
            ModeReglement? modeReglement = _db.ModeReglements.FirstOrDefault(a => a.ModeReglementId == modereglId);

            if (modeReglement is null)
            {
                return RedirectToAction("Error", "Home");
            }
            return View(modeReglement);
        }

        [HttpPost]
        public IActionResult Delete(ModeReglement modeReglement)
        {
            try
            {
                ModeReglement? mymodeReglement = _db.ModeReglements.FirstOrDefault(a => a.ModeReglementId == modeReglement.ModeReglementId);
                if (mymodeReglement is not null)
                {
                    _db.ModeReglements.Remove(mymodeReglement);
                    _db.SaveChanges();
                    TempData["success"] = "Mode reglement supprimé.";
                    return RedirectToAction("Index");
                }
                TempData["error"] = "Impossible de supprimer le mode de reglement.";
            }
            catch
            {
                TempData["error"] = "Impossible de supprimer le mode de reglement.";
            }
            return View(modeReglement);
        }
    }
}

