using Hotel.Data;
using Hotel.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;

namespace Hotel.Controllers
{
    [Authorize]
    [Authorize(Roles = "ADMIN,ADMINISTRATEUR")]
    public class PosteTravailController : Controller
    {
        private readonly ApplicationDbContext _db;

        public PosteTravailController(ApplicationDbContext db)
        {
            _db = db;
        }

        public IActionResult Index(int page = 1, int size = 20, string searchString="")
        {
            int totalposttravail = _db.PosteTravails.Count();
            int skipValue = (page - 1) * size;
            int totalPages = (int)Math.Ceiling((double)totalposttravail / size);
            var posttravail = _db.PosteTravails.Skip(skipValue).OrderByDescending(p => p.PosteTravailId).Take(size).ToList();
            var depart = (totalPages - 1) * skipValue + 1;
            ViewBag.NumAff = depart;
            ViewBag.CurrentPage = page;
            ViewBag.PageSize = size;
            ViewBag.TotalPage = totalPages;
            if (!string.IsNullOrEmpty(searchString))
            {
                posttravail = _db.PosteTravails.Where(p => p.TitrePosteTravail!.ToUpper().Contains(searchString.ToUpper())).OrderByDescending(p => p.PosteTravailId).Skip(skipValue).Take(size).ToList();
            }
            PosteTravailsViewModel posteTravailsViewModel = new()
            {
                PosteTravail = posttravail
            };
            return View(posteTravailsViewModel);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Create(PosteTravailViewModel posteTravailViewModel)
        {
            //PosteTravail? posteexist = _db.PosteTravails.Where(t => t.TitrePosteTravail == posteTravailViewModel.PosteTravail.TitrePosteTravail).FirstOrDefault(t => t.PosteTravailId != posteTravailViewModel.PosteTravail.PosteTravailId);
            //Caisse? caissebarexist = _db.Caisses.Where(c => c.TitreCaisse == posteTravailViewModel.Caisse.TitreCaisse && c.BarCaisse == posteTravailViewModel.Caisse.BarCaisse).FirstOrDefault(c => c.PosteTravailId != posteTravailViewModel.Caisse.PosteTravailId);

            //Sans distinction de caisse
            PosteTravail? posteexist = _db.PosteTravails.Where(t => t.TitrePosteTravail == posteTravailViewModel.PosteTravail.TitrePosteTravail).FirstOrDefault(t => t.PosteTravailId != posteTravailViewModel.PosteTravail.PosteTravailId);
            Caisse? caissebarexist = _db.Caisses.Where(c => c.TitreCaisse == posteTravailViewModel.Caisse.TitreCaisse).FirstOrDefault(c => c.PosteTravailId != posteTravailViewModel.Caisse.PosteTravailId);
            
            if (posteTravailViewModel.TypePoste == "AUTRE")
            {
                ModelState.Remove("Caisse.TitreCaisse");
                ModelState.Remove("Caisse.TitrePosteTravail");
                ModelState.Remove("Caisse.BarCaisse");
            }
            if (posteTravailViewModel.TypePoste == "CAISSE")
            {
                ModelState.Remove("Caisse.TitrePosteTravail");
                posteTravailViewModel.Caisse.TitrePosteTravail = posteTravailViewModel.PosteTravail.TitrePosteTravail;
            }

            if (ModelState.IsValid)
            {
                if (posteTravailViewModel.TypePoste == "AUTRE")
                {
                    if (posteexist is not null)
                    {
                        ModelState.AddModelError("PosteTravail.TitrePosteTravail", "Ce poste de travail existe déjà");
                    }
                }
                if (posteTravailViewModel.TypePoste == "CAISSE")
                {

                    if (caissebarexist is not null)
                    {
                        ModelState.AddModelError("Caisse.TitreCaisse", "Il existe déjà une caisse affectée à ce bar");
                        ModelState.AddModelError("Caisse.BarCaisse", "Il existe déjà une caisse affectée à ce bar");
                    }
                }
                if (ModelState.IsValid)
                {
                    if (posteTravailViewModel.TypePoste == "AUTRE")
                    {
                        _db.PosteTravails.Add(posteTravailViewModel.PosteTravail);
                    }
                    else if (posteTravailViewModel.TypePoste == "CAISSE")
                    {
                        _db.Caisses.Add(posteTravailViewModel.Caisse);
                    }

                    _db.SaveChanges();
                    TempData["success"] = "Poste de travail crée.";
                    return RedirectToAction("Index");
                }
            }

            return View();
        }

        public IActionResult Update(int postetravId)
        {
            PosteTravail? posteTravail = _db.PosteTravails.FirstOrDefault(a => a.PosteTravailId == postetravId);
            Caisse? caisse = _db.Caisses.FirstOrDefault(c => c.PosteTravailId == postetravId);
            PosteTravailViewModel posteTravailViewModel = new PosteTravailViewModel();

            if (posteTravail is not null)
            {
                posteTravailViewModel.PosteTravail = posteTravail;
                posteTravailViewModel.TypePoste = "AUTRE";
                if (caisse is not null)
                {
                    posteTravailViewModel.TypePoste = "CAISSE";
                    posteTravailViewModel.Caisse = caisse;

                }
            }
            else
            {
                return RedirectToAction("Error", "Home");
            }
            return View(posteTravailViewModel);

        }
        public IActionResult Details(int postetravId)
        {
            PosteTravail? posteTravail = _db.PosteTravails.FirstOrDefault(a => a.PosteTravailId == postetravId);
            Caisse? caisse = _db.Caisses.FirstOrDefault(c => c.PosteTravailId == postetravId);
            PosteTravailViewModel posteTravailViewModel = new PosteTravailViewModel();

            if (posteTravail is not null)
            {
                posteTravailViewModel.PosteTravail = posteTravail;
                posteTravailViewModel.TypePoste = "AUTRE";
                if (caisse is not null)
                {
                    posteTravailViewModel.TypePoste = "CAISSE";
                    posteTravailViewModel.Caisse = caisse;

                }
            }
            else
            {
                return RedirectToAction("Error", "Home");
            }
            return View(posteTravailViewModel);

        }
        [HttpPost]
        public IActionResult Update(PosteTravailViewModel posteTravailViewModel)
        {


            if (posteTravailViewModel.TypePoste == "AUTRE")
            {
                ModelState.Remove("Caisse.TitreCaisse");
                ModelState.Remove("Caisse.TitrePosteTravail");
                ModelState.Remove("Caisse.BarCaisse");
            }
            if (posteTravailViewModel.TypePoste == "CAISSE")
            {
                ModelState.Remove("Caisse.TitrePosteTravail");
                posteTravailViewModel.Caisse.TitrePosteTravail = posteTravailViewModel.PosteTravail.TitrePosteTravail;
                posteTravailViewModel.Caisse.PosteTravailId = posteTravailViewModel.PosteTravail.PosteTravailId;
            }

            PosteTravail? posteexist = _db.PosteTravails.Where(p => p.PosteTravailId != posteTravailViewModel.PosteTravail.PosteTravailId).FirstOrDefault(p => p.TitrePosteTravail == posteTravailViewModel.PosteTravail.TitrePosteTravail);
            Caisse? caissebarexist = _db.Caisses.Where(c => c.TitreCaisse == posteTravailViewModel.Caisse.TitreCaisse && c.BarCaisse == posteTravailViewModel.Caisse.BarCaisse).FirstOrDefault(c => c.PosteTravailId != posteTravailViewModel.Caisse.PosteTravailId);

            if (ModelState.IsValid)
            {
                if (posteTravailViewModel.TypePoste == "AUTRE")
                {
                    if (posteexist is not null)
                    {
                        ModelState.AddModelError("PosteTravail.TitrePosteTravail", "Ce poste de travail existe déjà");
                    }
                }
                if (posteTravailViewModel.TypePoste == "CAISSE")
                {

                    if (caissebarexist is not null)
                    {
                        ModelState.AddModelError("Caisse.TitreCaisse", "Il existe déjà une caisse affectée à ce bar");
                        ModelState.AddModelError("Caisse.BarCaisse", "Il existe déjà une caisse affectée à ce bar");
                    }
                }
                if (ModelState.IsValid)
                {
                    if (posteTravailViewModel.TypePoste == "AUTRE")
                    {
                        _db.PosteTravails.Update(posteTravailViewModel.PosteTravail);
                    }
                    else if (posteTravailViewModel.TypePoste == "CAISSE")
                    {
                        _db.Caisses.Update(posteTravailViewModel.Caisse);
                    }

                    _db.SaveChanges();
                    TempData["success"] = "Poste travail mis à jour.";
                    return RedirectToAction("Index");
                }
            }

            return View();
        }
        public IActionResult Delete(int postetravId)
        {
            PosteTravail? posteTravail = _db.PosteTravails.FirstOrDefault(a => a.PosteTravailId == postetravId);
            Caisse? caisse = _db.Caisses.FirstOrDefault(c => c.PosteTravailId == postetravId);
            PosteTravailViewModel posteTravailViewModel = new PosteTravailViewModel();

            if (posteTravail is not null)
            {
                posteTravailViewModel.PosteTravail = posteTravail;
                posteTravailViewModel.TypePoste = "AUTRE";
                if (caisse is not null)
                {
                    posteTravailViewModel.TypePoste = "CAISSE";
                    posteTravailViewModel.Caisse = caisse;

                }
            }
            else
            {
                return RedirectToAction("Error", "Home");
            }
            return View(posteTravailViewModel);
        }

        [HttpPost]
        public IActionResult Delete(PosteTravailViewModel posteTravailViewModel)
        {
            try
            {
                PosteTravail? mypostetrav = _db.PosteTravails.FirstOrDefault(p => p.PosteTravailId == posteTravailViewModel.PosteTravail.PosteTravailId);
                Caisse? mycaisse = _db.Caisses.FirstOrDefault(p => p.PosteTravailId == posteTravailViewModel.PosteTravail.PosteTravailId);


                if(mypostetrav is not null || mycaisse is not null)
                {
                    if (mypostetrav is not null)
                    {
                        _db.PosteTravails.Remove(mypostetrav);
                        _db.SaveChanges();
                    }
                    else if (mycaisse is not null)
                    {
                        _db.Caisses.Remove(posteTravailViewModel.Caisse);
                        _db.SaveChanges();
                    }
                    TempData["success"] = "Poste travail supprimé.";
                    return RedirectToAction("Index");
                }
                TempData["error"] = "Impossible de supprimer le poste de travail.";

            }
            catch(Exception ex)
            {
                Console.WriteLine("Mesage d'erreur :" + ex);
                TempData["error"] = "Impossible de supprimer le poste de travail. car il a déjà été utilisé";
            }
            return View();
        }
    }
}
