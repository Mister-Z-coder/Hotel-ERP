using Hotel.Data;
using Hotel.Models;
using Hotel.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Hotel.Controllers
{
    [Authorize]
    [Authorize(Roles = "ADMIN,ADMINISTRATEUR,SUPERVISEUR,RECEPTIONNISTE")]
    [CaisseRedirectFilter]
    public class ClientController : Controller
    {
        private readonly ApplicationDbContext _db;

        public ClientController(ApplicationDbContext db)
        {
            _db = db;
        }

        public IActionResult Index(int page=1, int size=5,string searchString="")
        {
            int totalclients = _db.Clients.Count();
            int skipValue = (page - 1) * size;
            int totalPages = (int)Math.Ceiling((double)totalclients / size);
            var clients = _db.Clients.ToList().Skip(skipValue).Take(size);
            var depart = (totalPages - 1) * skipValue + 1;
            ViewBag.NumAff = depart;
            ViewBag.CurrentPage = page;
            ViewBag.PageSize = size;
            ViewBag.TotalPage = totalPages;
            ViewBag.TotalClients = totalclients;

            if (!string.IsNullOrEmpty(searchString))
            {
                clients = _db.Clients.Where(c => c.NomClient!.ToUpper().Contains(searchString.ToUpper())).OrderByDescending(c => c.ClientId).ToList().Skip(skipValue).Take(size);
                
                
            }
            ClientViewModel clientViewModel = new()
            {
                Clients = clients
            };
            return View(clientViewModel);
        }
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Create(ClientViewModel ClientResident)
        {
            if (ClientResident.Client.TypeClient == "NORMAL")
            {
                ModelState.Remove("Resident.NomClient");
                ModelState.Remove("Resident.SexeClient");
                ModelState.Remove("Resident.TypeClient");
                ModelState.Remove("Resident.PhoneClient");
                ModelState.Remove("Resident.NumpieceId");
                ModelState.Remove("Resident.TypedocId");

                ClientResident.Normal = new Normal
                {
                    NomClient = ClientResident.Client.NomClient,
                    SexeClient = ClientResident.Client.SexeClient,
                    TypeClient = ClientResident.Client.TypeClient
                };
            }
            else if (ClientResident.Client.TypeClient == "RESIDENT")
            {
                ModelState.Remove("Resident.NomClient");
                ModelState.Remove("Resident.SexeClient");
                ModelState.Remove("Resident.TypeClient");

                ClientResident.Resident.NomClient = ClientResident.Client.NomClient;
                ClientResident.Resident.SexeClient = ClientResident.Client.SexeClient;
                ClientResident.Resident.TypeClient = ClientResident.Client.TypeClient;
            }

            if (ModelState.IsValid)
            {
                if (ClientResident.Client.TypeClient == "RESIDENT")
                {
                    _db.Residents.Add(ClientResident.Resident);
                }
                else if(ClientResident.Client.TypeClient == "NORMAL")
                {
                    _db.Normals.Add(ClientResident.Normal);
                }
                _db.SaveChanges();
                TempData["success"] = "Client crée.";
                return RedirectToAction("Index");
            }
            return View();
            
        }

        public IActionResult Update(int clientId)
        {
            Client? client = _db.Clients.FirstOrDefault(c => c.ClientId == clientId);
            Resident? resident = _db.Residents.FirstOrDefault(r => r.ClientId == clientId);
            Normal? normal = _db.Normals.FirstOrDefault(n => n.ClientId == clientId);
            ClientViewModel ClientResident = new ClientViewModel();

            if (client is not null && (resident is not null || normal is not null))
            {
                ClientResident.Client = client;
                if (resident is not null)
                {
                    ClientResident.Resident = resident;
                }
                else if (normal is not null)
                {
                    ClientResident.Normal = normal;
                }
            }
            else
            {
                return RedirectToAction("Error", "Home");
            }
            
            return View(ClientResident);
        }
        public IActionResult Details(int clientId)
        {
            Client? client = _db.Clients.FirstOrDefault(c => c.ClientId == clientId);
            Resident? resident = _db.Residents.FirstOrDefault(r => r.ClientId == clientId);
            Normal? normal = _db.Normals.FirstOrDefault(n => n.ClientId == clientId);
            ClientViewModel ClientResident = new ClientViewModel();

            if (client is not null && (resident is not null || normal is not null))
            {
                ClientResident.Client = client;
                if (resident is not null)
                {
                    ClientResident.Resident = resident;
                }
                else if (normal is not null)
                {
                    ClientResident.Normal = normal;
                }
            }
            else
            {
                return RedirectToAction("Error", "Home");
            }

            return View(ClientResident);
        }

        [HttpPost]
        public IActionResult Update(ClientViewModel ClientResident)
        {
            if (ClientResident.Client.TypeClient == "NORMAL")
            {
                ModelState.Remove("Resident.NomClient");
                ModelState.Remove("Resident.SexeClient");
                ModelState.Remove("Resident.TypeClient");
                ModelState.Remove("Resident.PhoneClient");
                ModelState.Remove("Resident.NumpieceId");
                ModelState.Remove("Resident.TypedocId");

                ClientResident.Normal = new Normal
                {
                    ClientId = ClientResident.Client.ClientId,
                    NomClient = ClientResident.Client.NomClient,
                    SexeClient = ClientResident.Client.SexeClient,
                    TypeClient = ClientResident.Client.TypeClient
                };
            }
            else if (ClientResident.Client.TypeClient == "RESIDENT")
            {
                ModelState.Remove("Resident.NomClient");
                ModelState.Remove("Resident.SexeClient");
                ModelState.Remove("Resident.TypeClient");

                ClientResident.Resident.ClientId = ClientResident.Client.ClientId;
                ClientResident.Resident.NomClient = ClientResident.Client.NomClient;
                ClientResident.Resident.SexeClient = ClientResident.Client.SexeClient;
                ClientResident.Resident.TypeClient = ClientResident.Client.TypeClient;
            }

            if (ModelState.IsValid)
            {
                if (ClientResident.Client.TypeClient == "RESIDENT")
                {
                    _db.Residents.Update(ClientResident.Resident);
                }
                else if (ClientResident.Client.TypeClient == "NORMAL")
                {
                    _db.Normals.Update(ClientResident.Normal);
                }
                _db.SaveChanges();
                TempData["success"] = "Client mis à jour.";
                return RedirectToAction("Index");
            }
            return View();
        }

        [Authorize]
        [Authorize(Roles = "ADMIN,SUPERVISEUR")]
        public IActionResult Delete(int clientId)
        {
            Client? client = _db.Clients.FirstOrDefault(c => c.ClientId == clientId);
            Resident? resident = _db.Residents.FirstOrDefault(r => r.ClientId == clientId);
            Normal? normal = _db.Normals.FirstOrDefault(n => n.ClientId == clientId);
            ClientViewModel ClientResident = new ClientViewModel();

            if(client is not null && (resident is not null || normal is not null))
            {
                ClientResident.Client = client;
                if (resident is not null)
                {
                    ClientResident.Resident = resident;
                }
                else if (normal is not null)
                {
                    ClientResident.Normal = normal;
                }
            }
            else
            {
                return RedirectToAction("Error", "Home");
            }
            return View(ClientResident);
        }

        [Authorize]
        [Authorize(Roles = "ADMIN,SUPERVISEUR")]
        [HttpPost]
        public IActionResult Delete(ClientViewModel ClientResident)
        {
            try
            {
                Client? client = _db.Clients.FirstOrDefault(c => c.ClientId == ClientResident.Client.ClientId);
                Resident? resident = _db.Residents.FirstOrDefault(r => r.ClientId == ClientResident.Client.ClientId);
                Normal? normal = _db.Normals.FirstOrDefault(n => n.ClientId == ClientResident.Client.ClientId);

                if (client is not null && (resident is not null || normal is not null))
                {
                    if (ClientResident.Client.TypeClient == "RESIDENT")
                    {
                        _db.Residents.Remove(resident);
                    }
                    else if (ClientResident.Client.TypeClient == "NORMAL")
                    {
                        _db.Normals.Remove(normal);
                    }
                    _db.SaveChanges();
                    TempData["success"] = "Client supprimé.";
                    return RedirectToAction("Index");
                }
                TempData["error"] = "Impossible de supprimer le client.";
            }
            catch
            {
                TempData["error"] = "Impossible de supprimer le client.";

            }
            return View();
        }
    }
}
