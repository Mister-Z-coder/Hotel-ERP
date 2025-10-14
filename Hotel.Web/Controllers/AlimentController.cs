using Hotel.Data;
using Hotel.Models;
using Hotel.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Hotel.Controllers
{
    [Authorize]
    [Authorize(Roles = "ADMIN,GESTIONNAIRE")]
    [CaisseRedirectFilter]
    public class AlimentController : Controller
    {
        private readonly ApplicationDbContext _db;

        private readonly IWebHostEnvironment _webHostEnvironment;
        public AlimentController(ApplicationDbContext db, IWebHostEnvironment webHostEnvironment)
        {
            _db = db;
            _webHostEnvironment = webHostEnvironment;
        }

        public IActionResult Index(int page = 1, int size = 20, string searchString = "")
        {
            int totalaliments = _db.Aliments.Count();
            int skipValue = (page - 1) * size;
            int totalPages = (int)Math.Ceiling((double)totalaliments / size);
            var aliments = _db.Aliments
                .OrderByDescending(a=>a.AlimentId)
                .Skip(skipValue)
                .Take(size)
                .ToList();
            var depart = (totalPages - 1) * skipValue + 1;
            ViewBag.NumAff = depart;
            ViewBag.CurrentPage = page;
            ViewBag.PageSize = size;
            ViewBag.TotalPage = totalPages;
            if (!string.IsNullOrEmpty(searchString))
            {
                aliments = _db.Aliments
                .Where(a=>a.TitreAliment!.ToUpper().Contains(searchString.ToUpper()))
                .OrderByDescending(a => a.AlimentId)
                .Skip(skipValue)
                .Take(size)
                .ToList();
            }
            AlimentViewModel alimentViewModel = new()
            {
                Aliments =  aliments
            };
            return View(alimentViewModel);
        }
        public IActionResult Create()
        {

            return View();
        }


        [HttpPost]
        public async Task<IActionResult> CreateAsync(AlimentViewModel alimentViewModel)
        {
            Aliment exist = _db.Aliments.Where(b => b.TitreAliment == alimentViewModel.Aliment.TitreAliment).FirstOrDefault(b => b.AlimentId != alimentViewModel.Aliment.AlimentId);

            if (ModelState.IsValid)
            {
                //if (alimentViewModel.Aliment.UniteAliment <= 0)
                //{
                //    ModelState.AddModelError("Aliment.UniteAliment", "Unité aliment requis.");
                //}
                if (alimentViewModel.Aliment.PrixUnitAliment <= 0)
                {
                    ModelState.AddModelError("Aliment.PrixUnitAliment", "Prix SB aliment requis.");
                }

                if (alimentViewModel.Aliment.QuantiteStock <= 0)
                {
                    ModelState.AddModelError("Aliment.QuantiteStock", "Quantité stock aliment requis.");
                }
                if (exist is not null)
                {
                    ModelState.AddModelError("Aliment.TitreAliment", "Cet aliment existe déjà");
                }
                // Vérifier le format du fichier
                var allowedExtensions = new[] { ".jpg" };
                var fileExtension = Path.GetExtension(alimentViewModel.Aliment.PhotoAliment.FileName).ToLowerInvariant();

                if (!allowedExtensions.Contains(fileExtension))
                {
                    ModelState.AddModelError("PhotoAliment", "Seuls les fichiers .jpg sont autorisés.");
                    return View(alimentViewModel);
                }
                if (ModelState.IsValid)
                {
                    if (alimentViewModel.Aliment.PhotoAliment != null && alimentViewModel.Aliment.PhotoAliment.Length > 0)
                    {
                        // Chemin vers le dossier 'images/aliments'
                        var uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "images", "aliments");
                        if (!Directory.Exists(uploadsFolder))
                        {
                            Directory.CreateDirectory(uploadsFolder);
                        }

                        // Inclure l'ID de l'agent dans le nom du fichier
                        var uniqueFileName = $"{alimentViewModel.Aliment.AlimentId}_{Guid.NewGuid()}_{alimentViewModel.Aliment.PhotoAliment.FileName}";
                        var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                        try
                        {
                            // Enregistrer le fichier sur le serveur
                            using (var fileStream = new FileStream(filePath, FileMode.Create))
                            {
                                await alimentViewModel.Aliment.PhotoAliment.CopyToAsync(fileStream);
                            }

                            // Stocker le chemin de la photo dans le modèle
                            alimentViewModel.Aliment.PhotoPath = Path.Combine("images", "aliments", uniqueFileName);
                        }
                        catch (Exception ex)
                        {
                            // Gérer l'erreur
                            Console.WriteLine($"Erreur lors du téléchargement de la photo : {ex.Message}");
                            //return BadRequest("Erreur lors du téléchargement de la photo.");
                        }
                    }

                    _db.Aliments.Add(alimentViewModel.Aliment);
                    _db.SaveChanges();

                    TempData["success"] = "Aliment créee.";
                    return RedirectToAction("Index");

                }
            }
            return View(alimentViewModel);

        }
        public IActionResult Details(int alimentId)
        {
            Aliment? aliment = _db.Aliments
                .Include(a=>a.PrixAliments.OrderByDescending(p=>p.DateHisto))
                .OrderBy(a=>a.AlimentId)
                .FirstOrDefault(r => r.AlimentId == alimentId);

            AlimentViewModel alimentViewModel = new AlimentViewModel();

            alimentViewModel.HPrixUnitAlimentList = aliment.PrixAliments;
            if (aliment is null)
            {
                return RedirectToAction("Error", "Home");
            }
            else
            {
                alimentViewModel.Aliment = aliment;
            }
            return View(alimentViewModel);
        }

        public IActionResult Update(int alimentId)
        {
            Aliment? aliment = _db.Aliments.FirstOrDefault(r => r.AlimentId == alimentId);
            AlimentViewModel alimentViewModel = new AlimentViewModel();

            if (aliment is null)
            {
                return RedirectToAction("Error", "Home");
            }
            else
            {
                alimentViewModel.Aliment = aliment;

            }
            return View(alimentViewModel);
        }

        [HttpPost]
        public async Task<IActionResult> UpdateAsync(AlimentViewModel alimentViewModel)
        {
            Aliment exist = _db.Aliments.Where(b => b.TitreAliment == alimentViewModel.Aliment.TitreAliment).FirstOrDefault(b => b.AlimentId != alimentViewModel.Aliment.AlimentId);
            var anciennephoto = alimentViewModel.Aliment.PhotoPath;
            ModelState.Remove("Aliment.PhotoAiment");
            ModelState.Remove("Aliment.PhotoPath");
            if (ModelState.IsValid)
            {
                //if (alimentViewModel.Aliment.UniteAliment <= 0)
                //{
                //    ModelState.AddModelError("Aliment.UniteAliment", "Unité aliment requis.");
                //}
                if (alimentViewModel.Aliment.PrixUnitAliment <= 0)
                {
                    ModelState.AddModelError("Aliment.PrixUnitAliment", "Prix SB aliment requis.");
                }
                if (alimentViewModel.Aliment.QuantiteStock <= 0)
                {
                    ModelState.AddModelError("Aliment.QuantiteStock", "Quantité stock aliment requis.");
                }
                if (exist is not null)
                {
                    ModelState.AddModelError("Aliment.TitreAliment", "Cet aliment existe déjà");
                }
                if(alimentViewModel.Aliment.PhotoAliment is not null)
                {
                    var allowedExtensions = new[] { ".jpg" };
                    var fileExtension = Path.GetExtension(alimentViewModel.Aliment.PhotoAliment.FileName).ToLowerInvariant();

                    if (!allowedExtensions.Contains(fileExtension))
                    {
                        ModelState.AddModelError("PhotoAliment", "Seuls les fichiers .jpg sont autorisés.");
                        return View(alimentViewModel.Aliment);
                    }
                }
                if (ModelState.IsValid)
                {
                    if(alimentViewModel.Aliment.PhotoAliment is not null)
                    {
                        // Chemin vers le dossier 'images/aliments'
                        var uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "images", "aliments");
                        if (!Directory.Exists(uploadsFolder))
                        {
                            Directory.CreateDirectory(uploadsFolder);
                        }

                        // Supprimer l'ancienne photo si elle existe
                        if (!string.IsNullOrEmpty(alimentViewModel.Aliment.PhotoPath))
                        {
                            var oldFilePath = Path.Combine(_webHostEnvironment.WebRootPath, alimentViewModel.Aliment.PhotoPath);
                            if (System.IO.File.Exists(oldFilePath))
                            {
                                System.IO.File.Delete(oldFilePath);
                            }
                        }

                        // Générer un nouveau nom de fichier
                        var uniqueFileName = $"{alimentViewModel.Aliment.AlimentId}_{Guid.NewGuid()}_{alimentViewModel.Aliment.PhotoAliment.FileName}";
                        var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                        try
                        {
                            // Enregistrer la nouvelle photo sur le serveur
                            using (var fileStream = new FileStream(filePath, FileMode.Create))
                            {
                                await alimentViewModel.Aliment.PhotoAliment.CopyToAsync(fileStream);
                            }

                            // Mettre à jour le chemin de la photo dans l'aliment
                            alimentViewModel.Aliment.PhotoPath = Path.Combine("images", "aliments", uniqueFileName);
                        }
                        catch (Exception ex)
                        {
                            // Gérer l'erreur
                            Console.WriteLine($"Erreur lors de la mise à jour de la photo : {ex.Message}");
                            //return BadRequest("Erreur lors de la mise à jour de la photo.");
                        }
                    }
                    else
                    {
                        alimentViewModel.Aliment.PhotoPath = anciennephoto;
                    }
                }
                _db.Aliments.Update(alimentViewModel.Aliment);
                _db.SaveChanges();

                HPrixUnitAliment hPrixUnitAliment = new()
                {
                    AlimentId = alimentViewModel.Aliment.AlimentId,
                    PrixUnitAliment = alimentViewModel.Aliment.PrixUnitAliment
                };
                _db.hPrixUnitAliments.Add(hPrixUnitAliment);
                _db.SaveChanges();

                TempData["success"] = "Aliment mis à jour.";
                return RedirectToAction("Index");

            }
            return View(alimentViewModel);
        }
        public IActionResult Delete(int alimentId)
        {
            Aliment? aliment = _db.Aliments.FirstOrDefault(r => r.AlimentId == alimentId);
            AlimentViewModel alimentViewModel = new AlimentViewModel();


            if (aliment is null)
            {
                return RedirectToAction("Error", "Home");
            }
            else
            {
                alimentViewModel.Aliment = aliment;
            }
            return View(alimentViewModel);
        }

        [HttpPost]
        public IActionResult Delete(AlimentViewModel alimentViewModel)
        {
            try
            {
                Aliment? aliment = _db.Aliments.FirstOrDefault(a => a.AlimentId == alimentViewModel.Aliment.AlimentId);

                if (aliment is not null)
                {
                    HPrixUnitAliment hPrixUnitAliment = _db.hPrixUnitAliments.FirstOrDefault(b => b.AlimentId == alimentViewModel.Aliment.AlimentId);

                    _db.hPrixUnitAliments.RemoveRange(hPrixUnitAliment);
                    _db.SaveChanges();
                    // Vérifier si une photo existe
                    if (!string.IsNullOrEmpty(aliment.PhotoPath))
                    {
                        var filePath = Path.Combine(_webHostEnvironment.WebRootPath, aliment.PhotoPath);

                        // Supprimer le fichier du serveur
                        if (System.IO.File.Exists(filePath))
                        {
                            try
                            {

                                _db.Aliments.Remove(aliment);
                                _db.SaveChanges();

                                System.IO.File.Delete(filePath);
                                TempData["success"] = "Aliment supprimé.";
                                return RedirectToAction("Index");

                            }
                            catch (Exception ex)
                            {
                                // Gérer l'erreur de suppression
                                Console.WriteLine($"Erreur lors de la suppression de la photo : {ex.Message}");
                                //return BadRequest("Erreur lors de la suppression de la photo.");
                                TempData["error"] = "Impossible de supprimer la aliment.\nCet aliment est lié à d'autres données et ne peut pas être supprimé";
                                return RedirectToAction("Index");
                            }
                        }

                        // Réinitialiser le chemin de la photo dans l'aliment
                        aliment.PhotoPath = null; 
                    }
                }
                TempData["error"] = "Impossible de supprimer la aliment.";
                return RedirectToAction("Index");


            }
            catch
            {
                TempData["error"] = "Impossible de supprimer la aliment.";
                return RedirectToAction("Index");
            }
            return View(alimentViewModel);
        }
    }
}
