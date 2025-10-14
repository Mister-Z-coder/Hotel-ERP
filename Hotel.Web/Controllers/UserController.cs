using Hotel.Data;
using Hotel.Models;
using Hotel.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Hotel.Controllers
{
    public class UserController : Controller
    {
        private readonly UserService _userService;
        private readonly ApplicationDbContext _db;

        public UserController(UserService userService, ApplicationDbContext db)
        {
            _db = db;
            _userService = userService;
        }

        [Authorize(Roles = "ADMIN,SUPERVISEUR")]
        public IActionResult Create()
        {
            UserAgentViewModel userAgentViewModel= new()
            {
                AgentList = _db.Agents.ToList().Where(a => !_db.Users.Any(u => u.AgentId == a.AgentId)).Select(a => new SelectListItem
                {
                    Text = $"{a.NomAgent} | Prénom : {a.PrenomAgent} | Sexe : {a.SexeAgent} | Nationalité : {a.Nationalite_Agent}",
                    Value = a.AgentId.ToString()
                }).ToList()
            };
            return View(userAgentViewModel);
        }

        [Authorize(Roles = "ADMIN,SUPERVISEUR")]
        [HttpPost]
        public IActionResult Create(UserAgentViewModel userAgentViewModel)
        {
            User existuser = _db.Users.FirstOrDefault(u => u.Username == userAgentViewModel.User.Username);
            Agent agent = _db.Agents.FirstOrDefault(a => a.AgentId == userAgentViewModel.User.AgentId);
            ModelState.Remove("User.Role");
            userAgentViewModel.User.Role = agent.Fonction_Agent;
            if (ModelState.IsValid)
            {
                if (existuser is not null)
                {
                    ModelState.AddModelError("User.Username", "Le nom d'utilisateur existe déjà.");
                }
                if(ModelState.IsValid)
                {
                    //Hasher le mot de passe
                    string password = userAgentViewModel.User.Password;
                    string hashedPassword = PasswordHasher.HashPassword(password);
                    userAgentViewModel.User.Password = hashedPassword;
                    //Enregistrer le mot de passe hashé
                    _db.Users.Add(userAgentViewModel.User);
                    _db.SaveChanges();
                    TempData["success"] = "Utilisateur crée.";
                    return RedirectToAction("Index");
                }
            }
            userAgentViewModel.AgentList = _db.Agents.ToList().Select(a => new SelectListItem
            {
                Text = $"{a.NomAgent} | Prénom : {a.PrenomAgent} | Sexe : {a.SexeAgent} | Nationalité : {a.Nationalite_Agent}",
                Value = a.AgentId.ToString()
            }).ToList();

            return View(userAgentViewModel);
        }

        [Authorize(Roles = "ADMIN,SUPERVISEUR")]
        public IActionResult Update(int userId)
        {
            User? users = _db.Users.ToList().FirstOrDefault(u=>u.Id==userId);
            UserAgentViewModel userAgentViewModel = new UserAgentViewModel();
            userAgentViewModel.AgentList = _db.Agents.ToList().Select(a => new SelectListItem
                {
                    Text = $"{a.NomAgent} | Prénom : {a.PrenomAgent} | Sexe : {a.SexeAgent} | Nationalité : {a.Nationalite_Agent}",
                    Value = a.AgentId.ToString()
                }).ToList();
            if (users is not null)
            {
                userAgentViewModel.User = users;
            }
            else
            {
                return RedirectToAction("Error", "Home");
            }
            return View(userAgentViewModel);
        }
        

        [Authorize(Roles = "ADMIN,SUPERVISEUR")]
        [HttpPost]
        public IActionResult Update(UserAgentViewModel userAgentViewModel)
        {
            ModelState.Remove("User.Role");
            
            User existuser = _db.Users.Where(u => u.Username == userAgentViewModel.User.Username).FirstOrDefault(a => a.Id != userAgentViewModel.User.Id);
            
            if (ModelState.IsValid)
            {
                if (existuser is not null)
                {
                    ModelState.AddModelError("User.Username", "Le nom d'utilisateur existe déjà.");
                }
                if (ModelState.IsValid)
                {
                    if(userAgentViewModel.NewPassword is not null)
                    {
                        //Hasher le mot de passe
                        //string password = userAgentViewModel.User.Password;
                        string password = userAgentViewModel.NewPassword;
                        string hashedPassword = PasswordHasher.HashPassword(password);
                        userAgentViewModel.User.Password = hashedPassword;
                        //Enregistrer le mot de passe hashé
                    }


                    userAgentViewModel.User.Role.ToUpper();
                    _db.Users.Update(userAgentViewModel.User);
                    _db.SaveChanges();
                    TempData["success"] = "Utilisateur mis à jour.";
                    return RedirectToAction("Index");
                }
            }
            userAgentViewModel.AgentList = _db.Agents.ToList().Select(a => new SelectListItem
            {
                Text = $"{a.NomAgent} | Prénom : {a.PrenomAgent} | Sexe : {a.SexeAgent} | Nationalité : {a.Nationalite_Agent}",
                Value = a.AgentId.ToString()
            }).ToList();

            return View(userAgentViewModel);
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }


        [Authorize(Roles = "ADMIN,SUPERVISEUR")]
        public IActionResult Index(int page = 1, int size = 20, string searchString = "")
        {
            int totalusers = _db.Users.Count();
            int skipValue = (page - 1) * size;
            int totalPages = (int)Math.Ceiling((double)totalusers / size);
            var users = _db.Users.Include(r => r.Agent).OrderByDescending(r=>r.Id).ToList().Skip(skipValue).Take(size);
            var depart = (totalPages - 1) * skipValue + 1;
            ViewBag.NumAff = depart;
            ViewBag.CurrentPage = page;
            ViewBag.PageSize = size;
            ViewBag.TotalPage = totalPages;
            if (!string.IsNullOrEmpty(searchString))
            {
                users = _db.Users.Include(r => r.Agent).Where(u => u.Username!.ToUpper().Contains(searchString.ToUpper())).ToList().Skip(skipValue).Take(size);
            }
            UserViewModel userViewModel = new()
            {
                User = users
            };
            return View(userViewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(string username, string password)
        {
            var user = _userService.Authenticate(username, password);

            if (user is null)
            {
                ModelState.AddModelError("", "Nom d'utilisateur ou mot de passe incorrect.");
                TempData["error"] = "Nom d'utilisateur ou mot de passe incorrect.";
                return View();
            }

            var agent = _db.Agents.FirstOrDefault(a => a.AgentId == user.AgentId);

            // Créer les claims (informations sur l'utilisateur)
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.Username),
                new Claim("AgentId", user.AgentId.ToString()),
                new Claim("PhotoUser", agent.PhotoPath.ToString()),
                new Claim(ClaimTypes.Role, user.Role.ToUpper())
            };

            // Créer l'identité de l'utilisateur
            var identity = new ClaimsIdentity(claims, "CookieAuth");
            var principal = new ClaimsPrincipal(identity);

            // Connecter l'utilisateur
            await HttpContext.SignInAsync("CookieAuth", principal);

            TempData["login"] = $"Bienvenue {username}";
            return RedirectToAction("Index", "Home");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            // Déconnecter l'utilisateur
            await HttpContext.SignOutAsync("CookieAuth");
            TempData["login"] = "A bientôt";
            return RedirectToAction("Login", "User");
        }

        [HttpGet]
        public IActionResult AccessDenied()
        {
            return View();
        }
        public IActionResult Profile()
        {
            var username = User.Identity.Name; // Nom d'utilisateur
            var role = User.FindFirst(ClaimTypes.Role)?.Value; // Rôle de l'utilisateur

            return View();
        }

        [Authorize(Roles = "ADMIN,SUPERVISEUR")]
        public IActionResult Delete(int userId)
        {
            User? users = _db.Users.ToList().FirstOrDefault(u => u.Id == userId);
            UserAgentViewModel userAgentViewModel = new UserAgentViewModel();
            userAgentViewModel.AgentList = _db.Agents.ToList().Select(a => new SelectListItem
            {
                Text = $"{a.NomAgent} | Prénom : {a.PrenomAgent} | Sexe : {a.SexeAgent} | Nationalité : {a.Nationalite_Agent}",
                Value = a.AgentId.ToString()
            }).ToList();
            if (users is not null)
            {
                userAgentViewModel.User = users;
            }
            else
            {
                return RedirectToAction("Error", "Home");
            }
            return View(userAgentViewModel);
        }


        [Authorize(Roles = "ADMIN,SUPERVISEUR")]
        [HttpPost]
        public IActionResult Delete(UserAgentViewModel userAgentViewModel)
        {
            ModelState.Remove("User.Role");

            User existuser = _db.Users.Where(u => u.Username == userAgentViewModel.User.Username).FirstOrDefault(a => a.Id != userAgentViewModel.User.Id);

            try
            {
                if (existuser is not null)
                {
                    ModelState.AddModelError("User.Username", "Le nom d'utilisateur existe déjà.");
                }
                else
                {
                    _db.Users.Remove(userAgentViewModel.User);
                    _db.SaveChanges();
                    TempData["success"] = "Utilisateur supprimé.";
                    return RedirectToAction("Index");
                }
                
            }
            catch 
            {

                TempData["success"] = "Utilisateur non supprimé.";
            }
            userAgentViewModel.AgentList = _db.Agents.ToList().Select(a => new SelectListItem
            {
                Text = $"{a.NomAgent} | Prénom : {a.PrenomAgent} | Sexe : {a.SexeAgent} | Nationalité : {a.Nationalite_Agent}",
                Value = a.AgentId.ToString()
            }).ToList();
            return View(userAgentViewModel);
        }
    }
}
