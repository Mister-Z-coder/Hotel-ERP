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
    public class BoissonController : Controller
    {
        private readonly ApplicationDbContext _db;
        private readonly IWebHostEnvironment _webHostEnvironment;
        public BoissonController(ApplicationDbContext db, IWebHostEnvironment webHostEnvironment)
        {
            _db = db;
            _webHostEnvironment = webHostEnvironment;
        }

        public IActionResult Index(int page = 1, int size = 20, string searchString = "")
        {
            int totalboissons = _db.Boissons.Count();
            int skipValue = (page - 1) * size;
            int totalPages = (int)Math.Ceiling((double)totalboissons / size);
            var boissons = _db.Boissons
                .Include(b =>b.Brasserie)
                .Include(b => b.CategorieBoisson)
                .AsSplitQuery()
                .OrderByDescending(b => b.BoissonId)
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
                boissons = _db.Boissons
                .Include(b => b.Brasserie)
                .Include(b => b.CategorieBoisson)
                .Where(a => a.TitreBoisson!.ToUpper().Contains(searchString.ToUpper()))
                .AsSplitQuery()
                .OrderByDescending(b => b.BoissonId)
                .Skip(skipValue)
                .Take(size)
                .ToList();
            }
            BoissonViewModel boissonViewModel = new()
            {
                Boissons = boissons
            };
            return View(boissonViewModel);
        }
        public IActionResult Create()
        {

            BoissonViewModel boissonViewModel = new()
            {
                BrasserieList = _db.Brasseries.ToList().Select(b => new SelectListItem
                {
                    Text = $"Titre brasserie : {b.TitreBrasserie}",
                    Value = b.BrasserieId.ToString()
                }),
                CategorieBoissonList = _db.CategorieBoissons.ToList().Select(c => new SelectListItem
                {
                    Text = $"Titre categorie : {c.TitreCategorieBoisson}",
                    Value = c.CategorieBoissonId.ToString()
                }),

            };
            return View(boissonViewModel);
        }
        private void ValidationDate(DateTime date, string fieldName, string errorMessage)
        {
            if (date < DateTime.Today)
            {
                ModelState.AddModelError(fieldName, errorMessage);
            }
        }



        [HttpPost]
        public async Task<IActionResult> CreateAsync(BoissonViewModel boissonViewModel)
        {
            Boisson exist = _db.Boissons.Where(b => b.TitreBoisson == boissonViewModel.Boisson.TitreBoisson).FirstOrDefault(b => b.BoissonId != boissonViewModel.Boisson.BoissonId);

            if (ModelState.IsValid)
            {
                
                if (boissonViewModel.Boisson.PrixUnitSB <= 0)
                {
                    ModelState.AddModelError("Boisson.PrixUnitSB", "Prix SB boisson requis.");
                }
                if (boissonViewModel.Boisson.PrixUnitLB <= 0)
                {
                    ModelState.AddModelError("Boisson.PrixUnitLB", "Prix LB boisson requis.");
                }
                if (boissonViewModel.Boisson.QuantiteStock <= 0)
                {
                    ModelState.AddModelError("Boisson.QuantiteStock", "Quantité stock boisson requis.");
                }
                if (exist is not null)
                {
                    ModelState.AddModelError("Boisson.TitreBoisson", "Cette boisson existe déjà");
                }
                // Vérifier le format du fichier
                var allowedExtensions = new[] { ".jpg" };
                var fileExtension = Path.GetExtension(boissonViewModel.Boisson.PhotoBoisson.FileName).ToLowerInvariant();

                if (!allowedExtensions.Contains(fileExtension))
                {
                    ModelState.AddModelError("PhotoBoisson", "Seuls les fichiers .jpg sont autorisés.");
                    return View(boissonViewModel);
                }
                if (ModelState.IsValid)
                {
                    if (boissonViewModel.Boisson.PhotoBoisson != null && boissonViewModel.Boisson.PhotoBoisson.Length > 0)
                    {
                        // Chemin vers le dossier 'images/boissons'
                        var uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "images", "boissons");
                        if (!Directory.Exists(uploadsFolder))
                        {
                            Directory.CreateDirectory(uploadsFolder);
                        }

                        // Inclure l'ID de l'agent dans le nom du fichier
                        var uniqueFileName = $"{boissonViewModel.Boisson.BoissonId}_{Guid.NewGuid()}_{boissonViewModel.Boisson.PhotoBoisson.FileName}";
                        var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                        try
                        {
                            // Enregistrer le fichier sur le serveur
                            using (var fileStream = new FileStream(filePath, FileMode.Create))
                            {
                                await boissonViewModel.Boisson.PhotoBoisson.CopyToAsync(fileStream);
                            }

                            // Stocker le chemin de la photo dans le modèle
                            boissonViewModel.Boisson.PhotoPath = Path.Combine("images", "boissons", uniqueFileName);
                        }
                        catch (Exception ex)
                        {
                            // Gérer l'erreur
                            Console.WriteLine($"Erreur lors du téléchargement de la photo : {ex.Message}");
                            //return BadRequest("Erreur lors du téléchargement de la photo.");
                        }
                    }
                    _db.Boissons.Add(boissonViewModel.Boisson);
                    _db.SaveChanges();
                    TempData["success"] = "Boisson créee.";
                    return RedirectToAction("Index");

                }
                boissonViewModel.BrasserieList = _db.Brasseries.ToList().Select(b => new SelectListItem
                {
                    Text = $"Titre brasserie : {b.TitreBrasserie}",
                    Value = b.BrasserieId.ToString()
                });
                boissonViewModel.CategorieBoissonList = _db.CategorieBoissons.ToList().Select(c => new SelectListItem
                {
                    Text = $"Titre categorie : {c.TitreCategorieBoisson}",
                    Value = c.CategorieBoissonId.ToString()
                });
            }
                return View(boissonViewModel);
        }
        public IActionResult Details(int boissonId)
        {
            Boisson? boisson = _db.Boissons
                .Include(b=>b.PrixLBs.OrderByDescending(p=>p.DateHisto))
                .Include(b=>b.PrixSBs.OrderByDescending(p => p.DateHisto))
                .OrderBy(b=>b.BoissonId)
                .FirstOrDefault(r => r.BoissonId == boissonId);
            //HPrixUnitSBBoisson? hPrixUnitSBBoisson = _db.HPrixUnitSBBoissons.FirstOrDefault(h => h.BoissonId == boissonId);
            //HPrixUnitLBBoisson? hPrixUnitLBBoisson = _db.HPrixUnitLBBoissons.FirstOrDefault(h => h.BoissonId == boissonId);

            BoissonViewModel boissonViewModel = new BoissonViewModel();

            boissonViewModel.BrasserieList = _db.Brasseries.ToList().Select(b => new SelectListItem
            {
                Text = $"Titre brasserie : {b.TitreBrasserie}",
                Value = b.BrasserieId.ToString()
            });
            boissonViewModel.CategorieBoissonList = _db.CategorieBoissons.ToList().Select(c => new SelectListItem
            {
                Text = $"Titre categorie : {c.TitreCategorieBoisson}",
                Value = c.CategorieBoissonId.ToString()
            });

            boissonViewModel.HPrixUnitSBBoissonList = boisson.PrixSBs;
            boissonViewModel.HPrixUnitLBBoissonList = boisson.PrixLBs;

            if (boisson is null)
            {
                return RedirectToAction("Error", "Home");
            }
            else
            {
                boissonViewModel.Boisson = boisson;
            }
            return View(boissonViewModel);
        }

        public IActionResult Update(int boissonId)
        {
            Boisson? boisson = _db.Boissons.FirstOrDefault(r => r.BoissonId == boissonId);
            BoissonViewModel boissonViewModel = new BoissonViewModel();

            boissonViewModel.BrasserieList = _db.Brasseries.ToList().Select(b => new SelectListItem
            {
                Text = $"Titre brasserie : {b.TitreBrasserie}",
                Value = b.BrasserieId.ToString()
            });
            boissonViewModel.CategorieBoissonList = _db.CategorieBoissons.ToList().Select(c => new SelectListItem
            {
                Text = $"Titre categorie : {c.TitreCategorieBoisson}",
                Value = c.CategorieBoissonId.ToString()
            });
            if (boisson is null)
            {
                return RedirectToAction("Error", "Home");
            }
            else
            {
                boissonViewModel.Boisson = boisson;

            }
            return View(boissonViewModel);
        }

        [HttpPost]
        public async Task<IActionResult> UpdateAsync(BoissonViewModel boissonViewModel)
        {
            Boisson exist = _db.Boissons.Where(b => b.TitreBoisson == boissonViewModel.Boisson.TitreBoisson).FirstOrDefault(b => b.BoissonId != boissonViewModel.Boisson.BoissonId);

            var anciennephoto = boissonViewModel.Boisson.PhotoPath;
            ModelState.Remove("Boisson.PhotoBoisson");
            ModelState.Remove("Boisson.PhotoPath");

            if (ModelState.IsValid)
            {
                
                if (boissonViewModel.Boisson.PrixUnitSB <= 0)
                {
                    ModelState.AddModelError("Boisson.PrixUnitSB", "Prix SB boisson requis.");
                }
                if (boissonViewModel.Boisson.PrixUnitLB <= 0)
                {
                    ModelState.AddModelError("Boisson.PrixUnitLB", "Prix LB boisson requis.");
                }
                if (boissonViewModel.Boisson.QuantiteStock <= 0)
                {
                    ModelState.AddModelError("Boisson.QuantiteStock", "Quantité stock boisson requis.");
                }
                if (exist is not null)
                {
                    ModelState.AddModelError("Boisson.TitreBoisson", "Cette boisson existe déjà");
                }
                if(boissonViewModel.Boisson.PhotoBoisson is not null)
                {
                    var allowedExtensions = new[] { ".jpg" };
                    var fileExtension = Path.GetExtension(boissonViewModel.Boisson.PhotoBoisson.FileName).ToLowerInvariant();

                    if (!allowedExtensions.Contains(fileExtension))
                    {
                        ModelState.AddModelError("PhotoBoisson", "Seuls les fichiers .jpg sont autorisés.");
                        return View(boissonViewModel.Boisson);
                    }
                }
                if (ModelState.IsValid)
                {

                    if(boissonViewModel.Boisson.PhotoBoisson is not null)
                    {
                        // Chemin vers le dossier 'images/boissons'
                        var uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "images", "boissons");
                        if (!Directory.Exists(uploadsFolder))
                        {
                            Directory.CreateDirectory(uploadsFolder);
                        }

                        // Supprimer l'ancienne photo si elle existe
                        if (!string.IsNullOrEmpty(boissonViewModel.Boisson.PhotoPath))
                        {
                            var oldFilePath = Path.Combine(_webHostEnvironment.WebRootPath, boissonViewModel.Boisson.PhotoPath);
                            if (System.IO.File.Exists(oldFilePath))
                            {
                                System.IO.File.Delete(oldFilePath);
                            }
                        }

                        // Générer un nouveau nom de fichier
                        var uniqueFileName = $"{boissonViewModel.Boisson.BoissonId}_{Guid.NewGuid()}_{boissonViewModel.Boisson.PhotoBoisson.FileName}";
                        var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                        try
                        {
                            // Enregistrer la nouvelle photo sur le serveur
                            using (var fileStream = new FileStream(filePath, FileMode.Create))
                            {
                                await boissonViewModel.Boisson.PhotoBoisson.CopyToAsync(fileStream);
                            }

                            // Mettre à jour le chemin de la photo dans la boisson
                            boissonViewModel.Boisson.PhotoPath = Path.Combine("images", "boissons", uniqueFileName);
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
                        boissonViewModel.Boisson.PhotoPath = anciennephoto;
                    }
                    _db.Boissons.Update(boissonViewModel.Boisson);
                    _db.SaveChanges();

                    HPrixUnitLBBoisson hPrixUnitLBBoisson = new()
                    {
                        BoissonId = boissonViewModel.Boisson.BoissonId,
                        PrixUnitLB = boissonViewModel.Boisson.PrixUnitLB
                    };

                    HPrixUnitSBBoisson hPrixUnitSBBoisson = new()
                    {
                        BoissonId = boissonViewModel.Boisson.BoissonId,
                        PrixUnitSB = boissonViewModel.Boisson.PrixUnitSB
                    };

                    _db.HPrixUnitLBBoissons.Add(hPrixUnitLBBoisson);
                    _db.SaveChanges();

                    _db.HPrixUnitSBBoissons.Add(hPrixUnitSBBoisson);
                    _db.SaveChanges();

                    TempData["success"] = "Boisson mise à jour.";
                    return RedirectToAction("Index");

                }

            }
            boissonViewModel.BrasserieList = _db.Brasseries.ToList().Select(b => new SelectListItem
            {
                Text = $"Titre brasserie : {b.TitreBrasserie}",
                Value = b.BrasserieId.ToString()
            });
            boissonViewModel.CategorieBoissonList = _db.CategorieBoissons.ToList().Select(c => new SelectListItem
            {
                Text = $"Titre categorie : {c.TitreCategorieBoisson}",
                Value = c.CategorieBoissonId.ToString()
            });
            return View(boissonViewModel);
        }
        public IActionResult Delete(int boissonId)
        {
            Boisson? boisson = _db.Boissons.FirstOrDefault(r => r.BoissonId == boissonId);
            BoissonViewModel boissonViewModel = new BoissonViewModel();

            boissonViewModel.BrasserieList = _db.Brasseries.ToList().Select(b => new SelectListItem
            {
                Text = $"Titre brasserie : {b.TitreBrasserie}",
                Value = b.BrasserieId.ToString()
            });
            boissonViewModel.CategorieBoissonList = _db.CategorieBoissons.ToList().Select(c => new SelectListItem
            {
                Text = $"Titre categorie : {c.TitreCategorieBoisson}",
                Value = c.CategorieBoissonId.ToString()
            });

            if (boisson is null)
            {
                return RedirectToAction("Error", "Home");
            }
            else
            {
                boissonViewModel.Boisson = boisson;
            }
            return View(boissonViewModel);
        }

        [HttpPost]
        public IActionResult Delete(BoissonViewModel boissonViewModel)
        {
            try
            {
                Boisson? boisson = _db.Boissons.FirstOrDefault(a => a.BoissonId == boissonViewModel.Boisson.BoissonId);

                if (boisson is not null)
                {
                    HPrixUnitSBBoisson hPrixUnitSBBoisson = _db.HPrixUnitSBBoissons.FirstOrDefault(b => b.BoissonId == boissonViewModel.Boisson.BoissonId);
                    HPrixUnitLBBoisson hPrixUnitLBBoisson = _db.HPrixUnitLBBoissons.FirstOrDefault(b => b.BoissonId == boissonViewModel.Boisson.BoissonId);

                    _db.HPrixUnitLBBoissons.RemoveRange(hPrixUnitLBBoisson);
                    _db.SaveChanges();

                    _db.HPrixUnitSBBoissons.RemoveRange(hPrixUnitSBBoisson);
                    _db.SaveChanges();
                    // Vérifier si une photo existe
                    if (!string.IsNullOrEmpty(boisson.PhotoPath))
                    {
                        var filePath = Path.Combine(_webHostEnvironment.WebRootPath, boisson.PhotoPath);

                        // Supprimer le fichier du serveur
                        if (System.IO.File.Exists(filePath))
                        {
                            try
                            {
                                _db.Boissons.Remove(boisson);
                                _db.SaveChanges();
                                System.IO.File.Delete(filePath);
                                TempData["success"] = "Boisson supprimée.";
                                return RedirectToAction("Index");
                            }
                            catch (Exception ex)
                            {
                                // Gérer l'erreur de suppression
                                Console.WriteLine($"Erreur lors de la suppression de la photo : {ex.Message}");
                                //return BadRequest("Erreur lors de la suppression de la photo.");

                                TempData["error"] = "Impossible de supprimer la boisson.\nCette boisson est liée à d'autres données et ne peut pas être supprimé";
                                return RedirectToAction("Index");
                            }
                        }

                        // Réinitialiser le chemin de la photo dans la boisson
                        boisson.PhotoPath = null; // Ou "" selon votre logique
                    }
                }

                boissonViewModel.BrasserieList = _db.Brasseries.ToList().Select(b => new SelectListItem
                {
                    Text = $"Titre brasserie : {b.TitreBrasserie}",
                    Value = b.BrasserieId.ToString()
                });
                boissonViewModel.CategorieBoissonList = _db.CategorieBoissons.ToList().Select(c => new SelectListItem
                {
                    Text = $"Titre categorie : {c.TitreCategorieBoisson}",
                    Value = c.CategorieBoissonId.ToString()
                });

                TempData["error"] = "Impossible de supprimer la boisson.";
                return RedirectToAction("Index");
            }
            catch
            {
                TempData["error"] = "Impossible de supprimer la boisson.";
                return RedirectToAction("Index");


            }
            return View(boissonViewModel);
        }
    }
}
