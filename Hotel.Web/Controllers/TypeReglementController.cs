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
    public class TypeReglementController : Controller
    {
        private readonly ApplicationDbContext _db;

        public TypeReglementController(ApplicationDbContext db)
        {
            _db = db;
        }

        public IActionResult Index(int page = 1, int size = 20)
        {
            int totaltypereglement = _db.TypeReglements.Count();
            int skipValue = (page - 1) * size;
            int totalPages = (int)Math.Ceiling((double)totaltypereglement / size);
            var typereglement = _db.TypeReglements.OrderByDescending(t=>t.TypeReglementId).Skip(skipValue).Take(size).ToList();
            var depart = (totalPages - 1) * skipValue + 1;
            ViewBag.NumAff = depart;
            ViewBag.CurrentPage = page;
            ViewBag.PageSize = size;
            ViewBag.TotalPage = totalPages;
            return View(typereglement);
        }
        
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Create(TypeReglement typeReglement)
        {
            TypeReglement? exist = _db.TypeReglements.FirstOrDefault(t=>t.TitreTypeReglement == typeReglement.TitreTypeReglement);
            if (ModelState.IsValid)
            {
                if(exist is not null)
                {
                    ModelState.AddModelError("TitreTypeReglement", "Ce type de reglement existe déjà");
                }
                if(ModelState.IsValid)
                {
                    _db.TypeReglements.Add(typeReglement);
                    _db.SaveChanges();
                    TempData["success"] = "Type de reglement crée.";
                    return RedirectToAction("Index");
                }
            }

            return View();
        }

        public IActionResult Update(int typereglId)
        {
            TypeReglement? typeReglement = _db.TypeReglements.FirstOrDefault(a => a.TypeReglementId == typereglId);
           
            if (typeReglement is null)
            {
                return RedirectToAction("Error", "Home");
            }
            return View(typeReglement);

        }
        public IActionResult Details(int typereglId)
        {
            TypeReglement? typeReglement = _db.TypeReglements.FirstOrDefault(a => a.TypeReglementId == typereglId);

            if (typeReglement is null)
            {
                return RedirectToAction("Error", "Home");
            }
            return View(typeReglement);

        }

        [HttpPost]
        public IActionResult Update(TypeReglement typeReglement)
        {
            TypeReglement? exist = _db.TypeReglements.Where(t=>t.TypeReglementId!=typeReglement.TypeReglementId).FirstOrDefault(t => t.TitreTypeReglement == typeReglement.TitreTypeReglement);
            if (ModelState.IsValid)
            {
                if (exist is not null)
                {
                    ModelState.AddModelError("TitreTypeReglement", "Ce type de reglement existe déjà");
                }
                if (ModelState.IsValid && typeReglement.TypeReglementId > 0)
                {
                    _db.TypeReglements.Update(typeReglement);
                    _db.SaveChanges();
                    TempData["success"] = "Type reglement mis à jour.";
                    return RedirectToAction("Index");
                }
            }

            
            return View(typeReglement);
        }
        public IActionResult Delete(int typereglId)
        {
            TypeReglement? typeReglement = _db.TypeReglements.FirstOrDefault(a => a.TypeReglementId == typereglId);

            if (typeReglement is null)
            {
                return RedirectToAction("Error", "Home");
            }
            return View(typeReglement);
        }

        [HttpPost]
        public IActionResult Delete(TypeReglement typeReglement)
        {
            try
            {
                TypeReglement? mytypeReglement = _db.TypeReglements.FirstOrDefault(a => a.TypeReglementId == typeReglement.TypeReglementId);
                if (mytypeReglement is not null)
                {
                    _db.TypeReglements.Remove(mytypeReglement);
                    _db.SaveChanges();
                    TempData["success"] = "Type reglement supprimé.";
                    return RedirectToAction("Index");
                }
                TempData["error"] = "Impossible de supprimer le type de reglement.";
            }
            catch
            {
                TempData["error"] = "Impossible de supprimer le type de reglement.";
            }
            return View(typeReglement);
        }
    }
}
