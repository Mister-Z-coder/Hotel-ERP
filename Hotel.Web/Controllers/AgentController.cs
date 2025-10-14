using Hotel.Data;
using Hotel.Models;
using Hotel.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Hotel.Controllers
{

    [Authorize]
    [Authorize(Roles = "ADMIN,ADMINISTRATEUR")]
    [CaisseRedirectFilter]
    public class AgentController : Controller
    {
        private readonly ApplicationDbContext _db;
        private readonly IWebHostEnvironment _webHostEnvironment;
        public AgentController(ApplicationDbContext db, IWebHostEnvironment webHostEnvironment)
        {
            _db = db;
            _webHostEnvironment = webHostEnvironment;
        }

        public IActionResult Index(int page = 1, int size = 20, string searchString="")
        {
            int totalagents = _db.Agents.Count();
            int skipValue = (page - 1) * size;
            int totalPages = (int)Math.Ceiling((double)totalagents / size);
            var agents = _db.Agents.ToList().OrderByDescending(a=>a.AgentId).Skip(skipValue).Take(size);
            var depart = (totalPages - 1) * skipValue + 1;
            ViewBag.NumAff = depart;
            ViewBag.CurrentPage = page;
            ViewBag.PageSize = size;
            ViewBag.TotalPage = totalPages;
            if (!string.IsNullOrEmpty(searchString))
            {
                agents = _db.Agents.Where(a=>a.NomAgent!.ToUpper().Contains(searchString.ToUpper())).ToList().OrderByDescending(a => a.AgentId).Skip(skipValue).Take(size);
            }
            AgentViewModel agentViewModel = new()
            {
                Agent = agents
            };

            return View(agentViewModel);
        }
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> CreateAsync(Agent agent)
        {
            Agent existagent = _db.Agents.Where(a=>a.NomAgent== agent.NomAgent && a.PrenomAgent == agent.PrenomAgent).FirstOrDefault(a => a.AgentId != agent.AgentId);
            if(agent.PhotoAgent is null)
            {
                ModelState.AddModelError("PhotoAgent", "Photo de l'agent requis");
            }
            if (ModelState.IsValid)
            {
                int age = DateTime.Today.Year - agent.Date_nais_Agent.Year;
                if(age < 18)
                {
                    ModelState.AddModelError("date_nais_Agent", "L'agent doit avoir au moins 18 ans.");
                }
                if(existagent is not null)
                {
                    ModelState.AddModelError("NomAgent", "Il existe déjà un agent répondant à ces deux noms.");
                }
                // Vérifier le format du fichier
                var allowedExtensions = new[] { ".jpg" };
                var fileExtension = Path.GetExtension(agent.PhotoAgent.FileName).ToLowerInvariant();

                if (!allowedExtensions.Contains(fileExtension))
                {
                    ModelState.AddModelError("PhotoAgent", "Seuls les fichiers .jpg sont autorisés.");
                    return View(agent);
                }
                

                if (ModelState.IsValid)
                {

                    if (agent.PhotoAgent != null && agent.PhotoAgent.Length > 0)
                    {
                        // Chemin vers le dossier 'images/agents'
                        var uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "images", "agents");
                        if (!Directory.Exists(uploadsFolder))
                        {
                            Directory.CreateDirectory(uploadsFolder);
                        }

                        // Inclure l'ID de l'agent dans le nom du fichier
                        var uniqueFileName = $"{agent.AgentId}_{Guid.NewGuid()}_{agent.PhotoAgent.FileName}";
                        var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                        try
                        {
                            // Enregistrer le fichier sur le serveur
                            using (var fileStream = new FileStream(filePath, FileMode.Create))
                            {
                                await agent.PhotoAgent.CopyToAsync(fileStream);
                            }

                            // Stocker le chemin de la photo dans le modèle
                            //agent.PhotoPath = Path.Combine("images", "agents", uniqueFileName);
                            agent.PhotoPath = uniqueFileName;

                        }
                        catch (Exception ex)
                        {
                            // Gérer l'erreur
                            Console.WriteLine($"Erreur lors du téléchargement de la photo : {ex.Message}");
                            //return BadRequest("Erreur lors du téléchargement de la photo.");
                        }
                    }
                    agent.Fonction_Agent.ToUpper();
                    _db.Agents.Add(agent);
                    _db.SaveChanges();
                    TempData["success"] = "Agent crée.";
                    return RedirectToAction("Index");
                }
            }
            return View();
        }

        public IActionResult Update(int agentId)
        {
            Agent? agent = _db.Agents.FirstOrDefault(a => a.AgentId == agentId);
            if(agent is null)
            {
                return RedirectToAction("Error", "Home");
            }
            return View(agent);
        }
        public IActionResult Details(int agentId)
        {
            Agent? agent = _db.Agents.FirstOrDefault(a => a.AgentId == agentId);
            if (agent is null)
            {
                return RedirectToAction("Error", "Home");
            }
            return View(agent);
        }
        [HttpPost]
        public async Task<IActionResult> UpdateAsync(Agent agent)
        {
            var anciennephoto = agent.PhotoPath;
            ModelState.Remove("PhotoAgent");
            ModelState.Remove("PhotoPath");
            if (ModelState.IsValid && agent.AgentId > 0)
            {
                Agent existagent = _db.Agents.Where(a => a.NomAgent == agent.NomAgent && a.PrenomAgent == agent.PrenomAgent).FirstOrDefault(a => a.AgentId != agent.AgentId);
                User user = _db.Users.SingleOrDefault(u => u.AgentId == agent.AgentId);
                int age = DateTime.Today.Year - agent.Date_nais_Agent.Year;
                
                if (age < 18)
                {
                    ModelState.AddModelError("date_nais_Agent", "L'agent doit avoir au moins 18 ans.");
                }
                if (existagent is not null)
                {
                    ModelState.AddModelError("NomAgent", "Il existe déjà un agent répondant à ces deux noms.");
                }

                if(agent.PhotoAgent is not null)
                {
                    var allowedExtensions = new[] { ".jpg" };
                    var fileExtension = Path.GetExtension(agent.PhotoAgent.FileName).ToLowerInvariant();

                    if (!allowedExtensions.Contains(fileExtension))
                    {
                        ModelState.AddModelError("PhotoAgent", "Seuls les fichiers .jpg sont autorisés.");
                        return View(agent);
                    }
                }

                if (ModelState.IsValid)
                {
                    if(agent.PhotoAgent is not null)
                    {
                        // Chemin vers le dossier 'images/agents'
                        var uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "images", "agents");
                        if (!Directory.Exists(uploadsFolder))
                        {
                            Directory.CreateDirectory(uploadsFolder);
                        }

                        // Supprimer l'ancienne photo si elle existe
                        if (!string.IsNullOrEmpty(agent.PhotoPath))
                        {
                            var oldFilePath = Path.Combine(_webHostEnvironment.WebRootPath, "images", "agents", agent.PhotoPath);
                            if (System.IO.File.Exists(oldFilePath))
                            {
                                System.IO.File.Delete(oldFilePath);
                            }
                        }

                        // Générer un nouveau nom de fichier
                        var uniqueFileName = $"{agent.AgentId}_{Guid.NewGuid()}_{agent.PhotoAgent.FileName}";
                        var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                        try
                        {
                            // Enregistrer la nouvelle photo sur le serveur
                            using (var fileStream = new FileStream(filePath, FileMode.Create))
                            {
                                await agent.PhotoAgent.CopyToAsync(fileStream);
                            }

                            // Mettre à jour le chemin de la photo dans l'agent
                            //agent.PhotoPath = Path.Combine("images", "agents", uniqueFileName);
                            agent.PhotoPath = uniqueFileName;
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
                        agent.PhotoPath = anciennephoto;
                    }
                    _db.Agents.Update(agent);
                    if(user is not null)
                    {
                        user.Role = agent.Fonction_Agent.ToUpper();
                        _db.Users.Update(user);
                        _db.SaveChanges();
                    }

                    _db.SaveChanges();
                    TempData["success"] = "Agent mis à jour.";
                    return RedirectToAction("Index");
                }
            }
            return View();
        }
        public IActionResult Delete(int agentId)
        {
            Agent? agent = _db.Agents.FirstOrDefault(a => a.AgentId == agentId);
            if (agent is null)
            {
                return RedirectToAction("Error", "Home");
            }
            return View(agent);
        }

        

        [HttpPost]
        public IActionResult Delete(Agent agent)
        {
            try
            {
                Agent? myagent = _db.Agents.FirstOrDefault(a => a.AgentId == agent.AgentId);
                User user = _db.Users.SingleOrDefault(u => u.AgentId == agent.AgentId);
                if (myagent is not null)
                {

                    // Vérifier si une photo existe
                    if (!string.IsNullOrEmpty(myagent.PhotoPath))
                    {
                        var filePath = Path.Combine(_webHostEnvironment.WebRootPath, "images", "agents", myagent.PhotoPath);

                        // Supprimer le fichier du serveur
                        if (System.IO.File.Exists(filePath))
                        {
                            try
                            {

                                if (user is not null)
                                {
                                    _db.Users.Remove(user);
                                    _db.SaveChanges();
                                }
                                _db.Agents.Remove(myagent);
                                _db.SaveChanges();

                                System.IO.File.Delete(filePath);


                                TempData["success"] = "Agent Supprimé.";
                                return RedirectToAction("Index");
                            }
                            catch (Exception ex)
                            {
                                // Gérer l'erreur de suppression
                                Console.WriteLine($"Erreur lors de la suppression de la photo : {ex.Message}");
                                //return BadRequest("Erreur lors de la suppression de la photo.");
                                TempData["error"] = "Impossible de supprimer l'agent.\nLa suppression de cet agent entraînera également la suppression de son compte utilisateur s'il est associé à d'autres entités";
                                return RedirectToAction("Index");

                            }
                        }

                        // Réinitialiser le chemin de la photo dans l'agent
                        agent.PhotoPath = null; // Ou "" selon votre logique
                    }

                }
                TempData["error"] = "Impossible de supprimer l\' agent.";
                return RedirectToAction("Index");

            }
            catch
            {
                TempData["error"] = "Impossible de supprimer l\' agent.";
                return RedirectToAction("Index");

            }
            return View();
        }
    }
}
