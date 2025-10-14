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
    public class TypeMouvementController : Controller
    {
        private readonly ApplicationDbContext _db;

        public TypeMouvementController(ApplicationDbContext db)
        {
            _db = db;
        }

        public IActionResult Index(int page = 1, int size = 20)
        {
            int totaltypemouvement = _db.TypeMouvements.Count();
            int skipValue = (page - 1) * size;
            int totalPages = (int)Math.Ceiling((double)totaltypemouvement / size);
            var typemouvement = _db.TypeMouvements.OrderByDescending(t=>t.TypeMouvementId).Skip(skipValue).Take(size).ToList();
            var depart = (totalPages - 1) * skipValue + 1;
            ViewBag.NumAff = depart;
            ViewBag.CurrentPage = page;
            ViewBag.PageSize = size;
            ViewBag.TotalPage = totalPages;
            return View(typemouvement);
        }
        
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Create(TypeMouvement typeMouvement)
        {
            TypeMouvement? exist = _db.TypeMouvements.FirstOrDefault(t=>t.TitreTypeMouvement == typeMouvement.TitreTypeMouvement);
            if (ModelState.IsValid)
            {
                if(exist is not null)
                {
                    ModelState.AddModelError("TitreTypeMouvement", "Ce type de mouvement existe déjà");
                }
                if(ModelState.IsValid)
                {
                    _db.TypeMouvements.Add(typeMouvement);
                    _db.SaveChanges();
                    TempData["success"] = "Type de mouvement crée.";
                    return RedirectToAction("Index");
                }
            }

            return View();
        }

        public IActionResult Update(int typemouvementId)
        {
            TypeMouvement? typeMouvement = _db.TypeMouvements.FirstOrDefault(a => a.TypeMouvementId == typemouvementId);
           
            if (typeMouvement is null)
            {
                return RedirectToAction("Error", "Home");
            }
            return View(typeMouvement);

        }
        public IActionResult Details(int typemouvementId)
        {
            TypeMouvement? typeMouvement = _db.TypeMouvements.FirstOrDefault(a => a.TypeMouvementId == typemouvementId);

            if (typeMouvement is null)
            {
                return RedirectToAction("Error", "Home");
            }
            return View(typeMouvement);

        }

        [HttpPost]
        public IActionResult Update(TypeMouvement typeMouvement)
        {
            TypeMouvement? exist = _db.TypeMouvements.Where(t=>t.TypeMouvementId!=typeMouvement.TypeMouvementId).FirstOrDefault(t => t.TitreTypeMouvement == typeMouvement.TitreTypeMouvement);
            if (ModelState.IsValid)
            {
                if (exist is not null)
                {
                    ModelState.AddModelError("TitreTypeMouvement", "Ce type de mouvement existe déjà");
                }
                if (ModelState.IsValid && typeMouvement.TypeMouvementId > 0)
                {
                    _db.TypeMouvements.Update(typeMouvement);
                    _db.SaveChanges();
                    TempData["success"] = "Type mouvement mis à jour.";
                    return RedirectToAction("Index");
                }
            }

            
            return View(typeMouvement);
        }
        public IActionResult Delete(int typemouvementId)
        {
            TypeMouvement? typeMouvement = _db.TypeMouvements.FirstOrDefault(a => a.TypeMouvementId == typemouvementId);

            if (typeMouvement is null)
            {
                return RedirectToAction("Error", "Home");
            }
            return View(typeMouvement);
        }

        [HttpPost]
        public IActionResult Delete(TypeMouvement typeMouvement)
        {
            try
            {
                TypeMouvement? mytypeMouvement = _db.TypeMouvements.FirstOrDefault(a => a.TypeMouvementId == typeMouvement.TypeMouvementId);
                if (mytypeMouvement is not null)
                {
                    _db.TypeMouvements.Remove(mytypeMouvement);
                    _db.SaveChanges();
                    TempData["success"] = "Type mouvement supprimé.";
                    return RedirectToAction("Index");
                }
                TempData["error"] = "Impossible de supprimer le type de mouvement.";
            }
            catch
            {
                TempData["error"] = "Impossible de supprimer le type de mouvement.";
            }
            return View(typeMouvement);
        }
    }
}
