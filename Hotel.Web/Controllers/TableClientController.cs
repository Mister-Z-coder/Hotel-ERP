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
    public class TableClientController : Controller
    {
        private readonly ApplicationDbContext _db;

        public TableClientController(ApplicationDbContext db)
        {
            _db = db;
        }

        public IActionResult Index(int page = 1, int size = 20,string searchString="")
        {
            int totaltableClient = _db.TableClients.Count();
            int skipValue = (page - 1) * size;
            int totalPages = (int)Math.Ceiling((double)totaltableClient / size);
            var tableClient = _db.TableClients.OfType<TableClient>().OrderByDescending(t=>t.TableClientId).Skip(skipValue).Take(size).ToList();
            var depart = (totalPages - 1) * skipValue + 1;
            ViewBag.NumAff = depart;
            ViewBag.CurrentPage = page;
            ViewBag.PageSize = size;
            ViewBag.TotalPage = totalPages;
            if (!string.IsNullOrEmpty(searchString))
            {
                tableClient = _db.TableClients.OfType<TableClient>().Where(t => t.TitreTableClient!.ToUpper().Contains(searchString.ToUpper())).OrderByDescending(t=>t.TableClientId).Skip(skipValue).Take(size).ToList();
            }
            TableClientViewModel tableClientViewModel = new()
            {
                TableClient = tableClient
            };
            return View(tableClientViewModel);
        }
        
        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        public IActionResult Create(TableClient tableCient)
        {
            TableClient? exist = _db.TableClients.FirstOrDefault(t => t.TitreTableClient == tableCient.TitreTableClient);
            if (ModelState.IsValid)
            {
                if (exist is not null)
                {
                    ModelState.AddModelError("TitreTableClient", "Cette table Client existe déjà");
                }
                if (ModelState.IsValid)
                {
                    _db.TableClients.Add(tableCient);
                    _db.SaveChanges();
                    TempData["success"] = "Table Client crée.";
                    return RedirectToAction("Index");
                }
            }

            return View();
        }
        public IActionResult Details(int tableCientId)
        {
            TableClient tableClient = _db.TableClients.FirstOrDefault(t => t.TableClientId == tableCientId);

            if (tableClient is null)
            {
                return RedirectToAction("Error", "Home");
            }

            return View(tableClient);
        }
        public IActionResult Update(int tableCientId)
        {
            TableClient tableClient = _db.TableClients.FirstOrDefault(t => t.TableClientId == tableCientId);

            if (tableClient is null)
            {
                return RedirectToAction("Error", "Home");
            }
            
            return View(tableClient);
        }//Vérifier le code

        [HttpPost]
        public IActionResult Update(TableClient tableClient)
        {
            TableClient? exist = _db.TableClients.FirstOrDefault(t => t.TitreTableClient == tableClient.TitreTableClient);
            if (ModelState.IsValid)
            {
                if (exist is not null)
                {
                    ModelState.AddModelError("TitreTableClient", "Cette table Client existe déjà");
                }
                if (ModelState.IsValid)
                {
                    _db.TableClients.Update(tableClient);
                    _db.SaveChanges();
                    TempData["success"] = "Table cient mise à jour.";
                    return RedirectToAction("Index");
                }
            }

            return View(tableClient);
        }
        public IActionResult Delete(int tableCientId)
        {
            TableClient tableClient = _db.TableClients.FirstOrDefault(c => c.TableClientId == tableCientId);


            if (tableClient is null)
            {
                return RedirectToAction("Error", "Home");
            }
            return View(tableClient);
        }

        [HttpPost]
        public IActionResult Delete(TableClient tableClient)
        {
            try
            {
                TableClient mytableClient = _db.TableClients.FirstOrDefault(c => c.TableClientId == tableClient.TableClientId);
                if (mytableClient is not null)
                {
                    _db.TableClients.Remove(mytableClient);
                    _db.SaveChanges();
                    TempData["success"] = "TableClient supprimée.";
                    return RedirectToAction("Index");
                }
                TempData["error"] = "Impossible de supprimer la tableClient.";
            }
            catch
            {
                TempData["error"] = "Impossible de supprimer la tableClient.";
            }
            return View(tableClient);
        }
    }
}
