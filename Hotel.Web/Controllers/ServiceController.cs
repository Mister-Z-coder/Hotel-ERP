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
    public class ServiceController : Controller
    {
        private readonly ApplicationDbContext _db;
        private readonly IWebHostEnvironment _webHostEnvironment;
        public ServiceController(ApplicationDbContext db, IWebHostEnvironment webHostEnvironment)
        {
            _db = db;
            _webHostEnvironment = webHostEnvironment;
        }

        public IActionResult Index(int page = 1, int size = 20, string searchString="")
        {
            int totalservices = _db.Services.Count();
            int skipValue = (page - 1) * size;
            int totalPages = (int)Math.Ceiling((double)totalservices / size);
            var services = _db.Services
                .OrderByDescending(s => s.ServiceId)
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
                services = _db.Services
                .Where(a => a.NomService!.ToUpper().Contains(searchString.ToUpper()))
                .OrderByDescending(s => s.ServiceId)
                .Skip(skipValue)
                .Take(size)
                .ToList();
            }
            ServiceViewModel serviceViewModel = new()
            {
                Service = services
            };
            return View(serviceViewModel);
        }
        public IActionResult Create()
        {
            return View();
        }
        

        [HttpPost]
        public async Task<IActionResult> CreateAsync(Service service)
        {
            Service exist = _db.Services.Where(s => s.NomService==service.NomService).FirstOrDefault(b => b.ServiceId != service.ServiceId);

            if (ModelState.IsValid)
            {
                if (service.PrixService <= 0)
                {
                    ModelState.AddModelError("PrixService", "Prix service requis.");
                }

                if (exist is not null)
                {
                    ModelState.AddModelError("NomService", "Ce service existe déjà");
                }
                // Vérifier le format du fichier
                var allowedExtensions = new[] { ".jpg" };
                var fileExtension = Path.GetExtension(service.PhotoService.FileName).ToLowerInvariant();

                if (!allowedExtensions.Contains(fileExtension))
                {
                    ModelState.AddModelError("PhotoService", "Seuls les fichiers .jpg sont autorisés.");
                    return View(service);
                }
                if (ModelState.IsValid)
                {
                    if (service.PhotoService != null && service.PhotoService.Length > 0)
                    {
                        // Chemin vers le dossier 'images/services'
                        var uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "images", "services");
                        if (!Directory.Exists(uploadsFolder))
                        {
                            Directory.CreateDirectory(uploadsFolder);
                        }

                        // Inclure l'ID de l'agent dans le nom du fichier
                        var uniqueFileName = $"{service.ServiceId}_{Guid.NewGuid()}_{service.PhotoService.FileName}";
                        var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                        try
                        {
                            // Enregistrer le fichier sur le serveur
                            using (var fileStream = new FileStream(filePath, FileMode.Create))
                            {
                                await service.PhotoService.CopyToAsync(fileStream);
                            }

                            // Stocker le chemin de la photo dans le modèle
                            service.PhotoPath = Path.Combine("images", "services", uniqueFileName);
                        }
                        catch (Exception ex)
                        {
                            // Gérer l'erreur
                            Console.WriteLine($"Erreur lors du téléchargement de la photo : {ex.Message}");
                            //return BadRequest("Erreur lors du téléchargement de la photo.");
                        }
                    }
                    _db.Services.Add(service);
                    _db.SaveChanges();
                    TempData["success"] = "Service crée.";
                    return RedirectToAction("Index");

                }

            }
            return View(service);
        }
        [HttpPost]
        public async Task<IActionResult> UpdateAsync(Service service)
        {
            var anciennephoto = service.PhotoPath;
            ModelState.Remove("PhotoService");
            ModelState.Remove("PhotoPath");
            Service exist = _db.Services.Where(b => b.NomService == service.NomService).FirstOrDefault(b => b.ServiceId != service.ServiceId);

            if (ModelState.IsValid)
            {
                if (service.PrixService <= 0)
                {
                    ModelState.AddModelError("PrixService", "Prix service requis.");
                }

                if (exist is not null)
                {
                    ModelState.AddModelError("NomService", "Ce service existe déjà");
                }
                if(service.PhotoService is not null)
                {
                    var allowedExtensions = new[] { ".jpg" };
                    var fileExtension = Path.GetExtension(service.PhotoService.FileName).ToLowerInvariant();

                    if (!allowedExtensions.Contains(fileExtension))
                    {
                        ModelState.AddModelError("PhotoService", "Seuls les fichiers .jpg sont autorisés.");
                        return View(service);
                    }
                }
                
                if (ModelState.IsValid)
                {
                    if(service.PhotoService is not null)
                    {
                        // Chemin vers le dossier 'images/services'
                        var uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "images", "services");
                        if (!Directory.Exists(uploadsFolder))
                        {
                            Directory.CreateDirectory(uploadsFolder);
                        }

                        // Supprimer l'ancienne photo si elle existe
                        if (!string.IsNullOrEmpty(service.PhotoPath))
                        {
                            var oldFilePath = Path.Combine(_webHostEnvironment.WebRootPath, service.PhotoPath);
                            if (System.IO.File.Exists(oldFilePath))
                            {
                                System.IO.File.Delete(oldFilePath);
                            }
                        }

                        // Générer un nouveau nom de fichier
                        var uniqueFileName = $"{service.ServiceId}_{Guid.NewGuid()}_{service.PhotoService.FileName}";
                        var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                        try
                        {
                            // Enregistrer la nouvelle photo sur le serveur
                            using (var fileStream = new FileStream(filePath, FileMode.Create))
                            {
                                await service.PhotoService.CopyToAsync(fileStream);
                            }

                            // Mettre à jour le chemin de la photo dans le service
                            service.PhotoPath = Path.Combine("images", "services", uniqueFileName);
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
                        service.PhotoPath = anciennephoto;
                    }
                    _db.Services.Update(service);
                    _db.SaveChanges();
                    
                    TempData["success"] = "Service mis à jour.";
                    return RedirectToAction("Index");

                }

            }
            return View(service);
        }
        public IActionResult Delete(int serviceId)
        {
            Service? service = _db.Services.FirstOrDefault(r => r.ServiceId == serviceId);
            if (service is null)
            {
                return RedirectToAction("Error", "Home");
            }
            return View(service);
        }
        public IActionResult Update(int serviceId)
        {
            Service? service = _db.Services.FirstOrDefault(r => r.ServiceId == serviceId);
            if (service is null)
            {
                return RedirectToAction("Error", "Home");
            }
            return View(service);
        }
        public IActionResult Details(int serviceId)
        {
            Service? service = _db.Services.FirstOrDefault(r => r.ServiceId == serviceId);
            if (service is null)
            {
                return RedirectToAction("Error", "Home");
            }
            return View(service);
        }
        [HttpPost]
        public IActionResult Delete(Service service)
        {
            try
            {
                Service? exist = _db.Services.FirstOrDefault(a => a.ServiceId == service.ServiceId);

                if (exist is not null)
                {
                    // Vérifier si une photo existe
                    if (!string.IsNullOrEmpty(exist.PhotoPath))
                    {
                        var filePath = Path.Combine(_webHostEnvironment.WebRootPath, exist.PhotoPath);

                        // Supprimer le fichier du serveur
                        if (System.IO.File.Exists(filePath))
                        {
                            try
                            {
                                _db.Services.Remove(exist);
                                _db.SaveChanges();
                                System.IO.File.Delete(filePath);
                                TempData["success"] = "Service supprimé.";
                                return RedirectToAction("Index");
                            }
                            catch (Exception ex)
                            {
                                // Gérer l'erreur de suppression
                                Console.WriteLine($"Erreur lors de la suppression de la photo : {ex.Message}");
                                //return BadRequest("Erreur lors de la suppression de la photo.");

                                TempData["error"] = "Impossible de supprimer le service.\nCe service est lié à d'autres données et ne peut pas être supprimé";
                                return RedirectToAction("Index");
                            }
                        }

                        // Réinitialiser le chemin de la photo dans le service
                        exist.PhotoPath = null; 
                    }

                }
                TempData["error"] = "Impossible de supprimer le service.";
                return RedirectToAction("Index");
            }
            catch
            {
                TempData["error"] = "Impossible de supprimer le service.";
                return RedirectToAction("Index");
            }
            return View();
        }
    }
}
