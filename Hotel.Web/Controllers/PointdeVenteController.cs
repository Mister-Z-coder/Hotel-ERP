using Hotel.Data;
using Hotel.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Data.SqlClient;
using System.Threading.Tasks;
using Hotel.Services;
using Microsoft.EntityFrameworkCore;

namespace Hotel.Controllers
{
    [Authorize]
    //[Authorize(Roles = "ADMIN,SUPERVISEUR,GESTIONNAIRE,CAISSIER")]
    [Authorize(Roles = "ADMIN,ADMINISTRATEUR,CAISSIER")]
    public class PointdeVenteController : Controller
    {
        //Roles pour interface caissier
        private readonly ApplicationDbContext _db;


        public PointdeVenteController(ApplicationDbContext db)
        {
            _db = db;
        }


        public IActionResult Index()
        {
            var agentIdClaim = User.Claims.FirstOrDefault(c => c.Type == "AgentId");
            var agentId = Convert.ToInt32(agentIdClaim.Value);

            var caisseDetails = (
            from fst in _db.FinShiftTravails
            join a in _db.Agents on fst.AgentId equals a.AgentId
            join c in _db.Caisses on fst.PosteTravailId equals c.PosteTravailId
            where fst.AgentId == agentId && fst.DateFinShift == null
            orderby fst.DateDebutShift descending // Trier par date de début décroissante
            select new CaisseDetail // Utilisez la classe CaisseDetail
            {
                CaisseId = c.PosteTravailId, // Inclut l'id de la caisse
                TitreCaisse = c.TitreCaisse,  // Inclut le titre de la caisse
                BarCaisse = c.BarCaisse       // Inclut le bar de la caisse
            }
            ).ToList();


            var dateDuJour = DateTime.Today;

            //var commandesDuJour = from commande in _db.Commandes // Assurez-vous que cela inclut les deux types de commandes
            //                      join agent in _db.Agents on commande.AgentId equals agent.AgentId
            //                      join client in _db.Clients on commande.ClientId equals client.ClientId
            //                      join commandeService in _db.CommandesServices on commande.CommandeId equals commandeService.CommandeId into cs
            //                      from commandeService in cs.DefaultIfEmpty() // Jointure optionnelle
            //                      join tableClient in _db.CommandeTableClients on commande.CommandeId equals tableClient.CommandeId into tc
            //                      from tableClient in tc.DefaultIfEmpty() // Jointure optionnelle
            //                      join tableClientInfo in _db.TableClients on tableClient.TableClientId equals tableClientInfo.TableClientId into tci
            //                      from tableClientInfo in tci.DefaultIfEmpty() // Jointure optionnelle
            //                                                                   //where commande.DateCommande.Date == dateDuJour && agent.AgentId == agentId// Commandes du jour du caissier
            //                      where agent.AgentId == agentId
            //                      && !_db.ReglementCommandes.Any(r => r.CommandeId == commande.CommandeId)// Filtre pour les commandes du jour
            //                      orderby commande.CommandeId descending, tableClientInfo.TitreTableClient, client.NomClient // Tri par ID de commande
            //                      select new CommandeJour
            //                      {
            //                          CommandeId = commande.CommandeId,
            //                          DtHrCommande = commande.DateCommande.ToShortDateString() + " " + commande.HeureCommande.ToShortTimeString(),
            //                          ClientNom = client.NomClient,
            //                          TableTitre = tableClientInfo.TitreTableClient,
            //                          TypeCommande = commande.TypeCommande, // Utilisation de l'info de table
            //                          AgentNom = agent.NomAgent + " " + agent.PrenomAgent
            //                      };

            var commandesDuJour = (from detailscmd in _db.DetailsCommandes
                                   join cmd in _db.Commandes on detailscmd.CommandeId equals cmd.CommandeId
                                   where
                                   !_db.ReglementCommandes.Any(r => r.CommandeId == detailscmd.CommandeId)
                                   orderby detailscmd.CommandeId descending, detailscmd.TitreTable, detailscmd.NomsClient
                                   select new CommandeJour
                                   {
                                       CommandeId = detailscmd.CommandeId,
                                       DtHrCommande = detailscmd.DateCommande.ToShortDateString() + " " + detailscmd.HeureCommande.ToShortTimeString(),
                                       ClientNom = detailscmd.NomsClient,
                                       TableTitre = detailscmd.TitreTable,
                                       TypeCommande = detailscmd.TypeCommande,
                                       AgentNom = detailscmd.NomsAgent
                                   })
                       .AsEnumerable() // Force l'évaluation côté client
                       .GroupBy(c => new { c.CommandeId, c.ClientNom })
                       .Select(g => g.FirstOrDefault())
                       .ToList();

            PointdeVenteViewModel pointdeVenteViewModel = new();
            pointdeVenteViewModel.CommandeJours = commandesDuJour;
            //pointdeVenteViewModel.Aliments = _db.Aliments.Where(a => a.QuantiteStock > 0).ToList();
            //pointdeVenteViewModel.Boissons = _db.Boissons.Where(b => b.QuantiteStock > 0).ToList();
            pointdeVenteViewModel.Aliments = _db.Aliments.ToList();
            pointdeVenteViewModel.Boissons = _db.Boissons.ToList();
            pointdeVenteViewModel.Services = _db.Services.ToList();
            ViewData["ServeurList"] = _db.Agents.Where(s => s.Fonction_Agent == "Serveur").ToList();
            ViewData["ClientList"] = _db.Clients.OrderByDescending(c=>c.ClientId).ThenBy(c=>c.NomClient).ToList();
            ViewData["TableList"] = _db.TableClients.OrderBy(t=>t.TitreTableClient).ToList();

            ViewData["ClientListDefaut"] = _db.Clients.Where(c => c.NomClient == "OCCASIONNEL").FirstOrDefault();
            ViewData["ModeReglementListDefaut"] = _db.ModeReglements.Where(m=>m.TitreModeReglement == "EN UNE SEULE FOIS").FirstOrDefault();
            ViewData["DeviseListDefaut"] = (from devises in _db.Devises
                                            join taux in _db.Tauxes on devises.DeviseId equals taux.DeviseId
                                            where devises.SigleDevise == "$"
                                            orderby taux.TauxId descending
                                            group devises by new
                                            {
                                                devises.DeviseId,
                                                devises.SigleDevise,
                                                devises.TitreDevise
                                            } into g
                                            select new Devise
                                            {
                                                DeviseId = g.Key.DeviseId,
                                                SigleDevise = g.Key.SigleDevise,
                                                TitreDevise = g.Key.TitreDevise
                                            }).FirstOrDefault();

            ViewData["TypeReglementListDefaut"] = _db.TypeReglements.Where(t=>t.TitreTypeReglement == "EN ESPECE").FirstOrDefault();


            ViewData["ModeReglementList"] = _db.ModeReglements.ToList();
            ViewData["DeviseList"] = (from devises in _db.Devises
                                      join taux in _db.Tauxes on devises.DeviseId equals taux.DeviseId
                                      orderby taux.TauxId descending
                                      group devises by new
                                      {
                                          devises.DeviseId,
                                          devises.SigleDevise,
                                          devises.TitreDevise
                                      } into g
                                      select new Devise
                                      {
                                          DeviseId = g.Key.DeviseId,
                                          SigleDevise = g.Key.SigleDevise,
                                          TitreDevise = g.Key.TitreDevise
                                      }).ToList();
            ViewData["TypeReglementList"] = _db.TypeReglements.ToList();
            
            
            ViewData["Caisse"] = caisseDetails;

            //pointdeVenteViewModel.TableClients = _db.TableClients.ToList();
            if (caisseDetails is not null && caisseDetails.Count() > 0)
            {
                
                foreach (var caisse in caisseDetails)
                {
                    ViewBag.TitreCaisse = caisse.TitreCaisse;
                    ViewBag.BarCaisse = caisse.BarCaisse;
                    //Console.WriteLine(caisse.BarCaisse +"|"+caisse.TitreCaisse);
                }
            }
            else
            {
                TempData["error"] = "Aucune caisse connectée.";
                ViewBag.TitreCaisse = "";
                ViewBag.BarCaisse = "";
            }
            return View(pointdeVenteViewModel);
        }

        [HttpPost]
        [Route("ValiderCommande")]

        public IActionResult ValiderCommande([FromBody] CommandeData data)
        {
            List<object> detailsList = new List<object>();

            try
            {

                // Vérifier si les données sont nulles ou vides
                if (data == null || data.Products == null || !data.Products.Any())
                {
                    return Json(new { success = false, message = "Aucune donnée de commande reçue." });
                }

                //// Convertir la date et l'heure
                //if (!DateTime.TryParse(data.DateOccupation, out DateTime dateOccupation))
                //{
                //    return Json(new { success = false, message = "Format de date invalide." });
                //}

                //if (!DateTime.TryParse(data.HeureOccupation, out DateTime heureOccupation))
                //{
                //    return Json(new { success = false, message = "Format d'heure invalide." });
                //}

                // Récupérer l'ID de l'agent à partir des claims
                var agentIdClaim = User.Claims.FirstOrDefault(c => c.Type == "AgentId");
                if (agentIdClaim == null || !int.TryParse(agentIdClaim.Value, out int agentId))
                {
                    return Json(new { success = false, message = "Agent non identifié." });
                }

                // Vérifier les données obligatoires
                if (string.IsNullOrEmpty(data.ClientId) || string.IsNullOrEmpty(data.TableId) || string.IsNullOrEmpty(data.TypeCommande))
                {
                    return Json(new { success = false, message = "Données de commande incomplètes." });
                }

                //Verifier l'existance d'une boisson ou aliment dans la liste des commandes
                var existprod = 0;
                decimal montantTotal=0;
                foreach (var p in data.Products)
                {
                    if (p.Type == "aliment" || p.Type == "boisson")
                    {
                        existprod++;
                        montantTotal += (int.Parse(p.Pu) * p.Quantity);
                    }
                    
                }
                //Si aliment ou boisson existe, alors créer commande pour ces derniers
                var newCommand = new CommandeTableClient();
                if (existprod > 0)
                {

                    // Créer une nouvelle commande pour les produits
                    newCommand = new()
                    {
                        AgentId = agentId,
                        //AgentId = int.Parse(data.ServeurId),
                        ClientId = int.Parse(data.ClientId),
                        TypeCommande = data.TypeCommande,
                        TableClientId = int.Parse(data.TableId),
                        MontantTotal = montantTotal,
                        DateOccupation = DateTime.Today,
                        HeureOccupation = DateTime.Now
                    };

                    _db.CommandeTableClients.Add(newCommand);
                    _db.SaveChanges();
                    existprod = 0;
                }

                //Créer une commande de service
                var commandesservice = new CommandesService();

                //Parcourir les produits pour trouver service
                foreach (var product in data.Products)
                {
                    montantTotal = 0;

                    //Trouver service
                    if (product.Type == "service")
                    {
                        //Insertion de chaque services dans la table commande
                        
                        montantTotal += (int.Parse(product.Pu) * product.Quantity);

                        //Trouver le service
                        var serviceToCommandesService = _db.Services.FirstOrDefault(s => s.ServiceId == int.Parse(product.Id) && s.NomService == product.Name);
                        if (serviceToCommandesService == null)
                        {
                            return Json(new { success = false, message = $"Service introuvable : {product.Name}" });
                        }

                        //Créer une nouvelle commande service
                        commandesservice = new()
                        {
                            //AgentId = int.Parse(data.ServeurId),
                            AgentId = agentId,
                            ClientId = int.Parse(data.ClientId),
                            TypeCommande = data.TypeCommande,
                            MontantTotal = montantTotal,
                            ServiceId = serviceToCommandesService.ServiceId
                        };
                        _db.CommandesServices.Add(commandesservice);
                        _db.SaveChanges();


                        //// Créer une nouvelle commande
                        //var newCommandService = new CommandeTableClient
                        //{
                        //    CommandeId = commandesservice.CommandeId,
                        //    TableClientId = int.Parse(data.TableId),
                        //    DateOccupation = dateOccupation,
                        //    HeureOccupation = heureOccupation
                        //};

                        //_db.CommandeTableClients.Add(newCommandService);
                        //_db.SaveChanges();

                        //Enregistrer le service dans la table details commande
                        DetailsCommande detailsCommande = new()
                        {
                            PrixUnitaire = float.Parse(product.Pu),
                            ProduitNom = product.Name,
                            Quantite = product.Quantity,
                            CommandeId = commandesservice.CommandeId,
                            TypeCommande = data.TypeCommande,
                            NomsClient = TrouverDonneesBydata(int.Parse(data.ClientId), "Client").ToString(),
                            NomsAgent = TrouverDonneesBydata(agentId, "Agent").ToString(),
                            TitreTable = TrouverDonneesBydata(int.Parse(data.TableId), "TableClient").ToString(),
                            DateCommande = DateTime.Today,
                            HeureCommande = DateTime.Now,
                            TypeProduit = product.Type
                        };
                        _db.DetailsCommandes.Add(detailsCommande);
                        _db.SaveChanges();
                        // Ajouter les détails au tableau
                        detailsList.Add(new { commandeId = commandesservice.CommandeId, typeProd = product.Type });

                        ///Enregistrement détails facture tout juste apres l'élaboration de la commande XXXX

                        DetailsFacture detailsFacture = new()
                        {
                            Numcmd = detailsCommande.CommandeId,
                            //Numfact
                            //Datefact
                            //Heurefact
                            //Caissier
                            //Caisse
                            Designation = detailsCommande.ProduitNom,
                            Qte = detailsCommande.Quantite,
                            PU = (decimal)detailsCommande.PrixUnitaire,
                            Montanttotal = (decimal)detailsCommande.PrixUnitaire * detailsCommande.Quantite
                            //ModeReglement
                            //TypeReglement
                            //Devise
                        };
                        _db.DetailsFactures.Add(detailsFacture);
                        _db.SaveChanges();



                    }
                }

                // Traiter chaque produit de la commande
                foreach (var product in data.Products)
                {

                    if (product.Type == "aliment")
                    {
                        var alimentToUpdate = _db.Aliments.FirstOrDefault(a => a.AlimentId == int.Parse(product.Id) && a.TitreAliment == product.Name);
                        if (alimentToUpdate == null)
                        {
                            return Json(new { success = false, message = $"Aliment introuvable : {product.Name}" });
                        }

                        //Update Qte stock
                        alimentToUpdate.QuantiteStock -= product.Quantity;
                        _db.Aliments.Update(alimentToUpdate);

                        //Sauvegarde dans la table concerner
                        var concerner = new Concerner
                        {
                            AlimentId = int.Parse(product.Id),
                            CommandeId = newCommand.CommandeId,
                            QuantiteCom = product.Quantity
                        };
                        _db.Concerners.Add(concerner);
                        _db.SaveChanges();

                        //Enregistrer aliment dans la table details commande
                        DetailsCommande detailsCommande = new()
                        {
                            PrixUnitaire = float.Parse(product.Pu),
                            ProduitNom = product.Name,
                            Quantite = product.Quantity,
                            CommandeId = newCommand.CommandeId,
                            TypeCommande = data.TypeCommande,
                            NomsClient = TrouverDonneesBydata(int.Parse(data.ClientId), "Client").ToString(),
                            NomsAgent = TrouverDonneesBydata(agentId, "Agent").ToString(),
                            TitreTable = TrouverDonneesBydata(int.Parse(data.TableId), "TableClient").ToString(),
                            DateCommande = DateTime.Today,
                            HeureCommande = DateTime.Now,
                            TypeProduit = product.Type
                        };
                        _db.DetailsCommandes.Add(detailsCommande);
                        _db.SaveChanges();
                        detailsList.Add(new { commandeId = newCommand.CommandeId, typeProd = product.Type });

                        //Enregistrement de la facture tout juste après la commande aliement XXX

                        DetailsFacture detailsFacture = new()
                        {
                            Numcmd = detailsCommande.CommandeId,
                            Designation = detailsCommande.ProduitNom,
                            Qte = detailsCommande.Quantite,
                            PU = (decimal)detailsCommande.PrixUnitaire,
                            Montanttotal = (decimal)detailsCommande.PrixUnitaire * detailsCommande.Quantite
                        };
                        _db.DetailsFactures.Add(detailsFacture);
                        _db.SaveChanges();
                    }
                    else if (product.Type == "boisson")
                    {
                        var boissonToUpdate = _db.Boissons.FirstOrDefault(b => b.BoissonId == int.Parse(product.Id) && b.TitreBoisson == product.Name);
                        if (boissonToUpdate == null)
                        {
                            return Json(new { success = false, message = $"Boisson introuvable : {product.Name}" });
                        }
                        //Update Qte stock
                        boissonToUpdate.QuantiteStock -= product.Quantity;
                        _db.Boissons.Update(boissonToUpdate);

                        //Sauvegarde dans la table contenir
                        var contenir = new Contenir
                        {
                            BoissonId = int.Parse(product.Id),
                            CommandeId = newCommand.CommandeId,
                            QuantiteCom = product.Quantity
                        };
                        _db.Contenirs.Add(contenir);
                        _db.SaveChanges();


                        //Enregistrer aliment dans la table details commande
                        DetailsCommande detailsCommande = new()
                        {
                            PrixUnitaire = float.Parse(product.Pu),
                            ProduitNom = product.Name,
                            Quantite = product.Quantity,
                            CommandeId = newCommand.CommandeId,
                            TypeCommande = data.TypeCommande,
                            NomsClient = TrouverDonneesBydata(int.Parse(data.ClientId), "Client"),
                            NomsAgent = TrouverDonneesBydata(agentId, "Agent"),
                            TitreTable = TrouverDonneesBydata(int.Parse(data.TableId), "TableClient"),
                            DateCommande = DateTime.Today,
                            HeureCommande = DateTime.Now,
                            TypeProduit = product.Type
                        };
                        _db.DetailsCommandes.Add(detailsCommande);
                        detailsList.Add(new { commandeId = newCommand.CommandeId, typeProd = product.Type });
                        _db.SaveChanges();

                        //Enregistrement de la facture tout juste après la commande boisson XXX

                        DetailsFacture detailsFacture = new()
                        {
                            Numcmd = detailsCommande.CommandeId,
                            Designation = detailsCommande.ProduitNom,
                            Qte = detailsCommande.Quantite,
                            PU = (decimal)detailsCommande.PrixUnitaire,
                            Montanttotal = (decimal)detailsCommande.PrixUnitaire * detailsCommande.Quantite
                        };
                        _db.DetailsFactures.Add(detailsFacture);
                        _db.SaveChanges();
                    }
                    else if (product.Type == "service")
                    {
                    }
                    else
                    {
                        return Json(new { success = false, message = $"Type de produit non reconnu : {product.Type}" });
                    }


                }

                //return Json(new { success = true, message = "Commande validée avec succès." });
                // Retourner le tableau de détails avec un message
                return Json(new { success = true, message = "Commande validée avec succès.", details = detailsList });
            }
            catch (Exception ex)
            {
                // Log l'erreur
                Console.Error.WriteLine("Erreur lors de la validation de la commande : " + ex.Message);
                return Json(new { success = false, message = "Une erreur s'est produite lors de la validation de la commande." });
            }
        }
        String TrouverDonneesBydata(int donneId, string context)
        {
            string valeur = "";
            if (context == "Agent")
            {
                Agent agent = _db.Agents.FirstOrDefault(a => a.AgentId == donneId);
                valeur = agent.NomAgent + " " + agent.PrenomAgent;
            }
            else if (context == "Client")
            {
                Client client = _db.Clients.FirstOrDefault(c => c.ClientId == donneId);
                //valeur = client.NomClient;
                valeur = client.NomClient + " " + client.TypeClient;
            }
            else if (context == "TableClient")
            {
                TableClient tableClient = _db.TableClients.FirstOrDefault(t => t.TableClientId == donneId);
                valeur = tableClient.TitreTableClient;
            }
            else if (context == "Commande")
            {
                Commande commande = _db.Commandes.FirstOrDefault(c => c.CommandeId == donneId);
                valeur = commande.MontantTotal.ToString();
            }
            return valeur;
        }
        //Pour representer une commande
        public class CommandeData
        {
            //public string ServeurId { get; set; }
            //public string AgentId { get; set; }
            public string TableId { get; set; }
            public string ClientId { get; set; }
            public string DateOccupation { get; set; } // Changé en string
            public string HeureOccupation { get; set; } // Changé en string
            public string TypeCommande { get; set; }
            public List<Product> Products { get; set; }
        }

        public class ReglementData
        {
            public List<string> CommandeId { get; set; } // Liste des identifiants de commandes
            public string ModeReglementId { get; set; }      // Identifiant du mode de règlement
            public string DeviseId { get; set; }              // Identifiant de la devise
            public string TypeReglementId { get; set; }       // Identifiant du type de règlement
            public string PosteTravailId { get; set; }        // Identifiant du poste de travail
            public string MontantReglement { get; set; }   // Montant du règlement
            public string MotifReglement { get; set; }     // Motif du règlement
        }

        // Classe pour représenter un produit
        public class Product
        {
            public string Id { get; set; }
            public string Name { get; set; }
            public string Pu { get; set; }
            public int Quantity { get; set; }
            public string Type { get; set; }
        }
        public class CommandeRequest
        {
            public List<int> CommandeIds { get; set; }
        }

        //Recuperer les details de la commande
        //[HttpGet("GetDetailsCommande")]
        //public IActionResult GetDetailsCommande(int commandeId)
        //{
        //    try
        //    {
        //        // Récupérer les détails de la commande depuis la base de données
        //        var details = _db.DetailsCommandes
        //            .Where(d => d.CommandeId == commandeId)
        //            .Select(d => new
        //            {
        //                datecommande = d.DateCommande.ToShortDateString() + " " + d.HeureCommande.ToShortTimeString(),
        //                table = d.TitreTable,
        //                client = d.NomsClient,
        //                agent = d.NomsAgent,
        //                nom = d.ProduitNom,
        //                quantite = d.Quantite,
        //                prixUnitaire = d.PrixUnitaire,
        //                total = d.MontantTotal
        //            })
        //            .ToList();

        //        return Json(details);
        //    }
        //    catch (Exception ex)
        //    {
        //        return Json(new { error = ex.Message });
        //    }
        //}
        [HttpPost("GetDetailsCommande")]
        public IActionResult GetDetailsCommande([FromBody] CommandeRequest request)
        {
            try
            {
                // Récupérer les détails de toutes les commandes depuis la base de données
                var details = _db.DetailsCommandes
                    .Where(d => request.CommandeIds.Contains(d.CommandeId))
                    .Select(d => new
                    {
                        datecommande = d.DateCommande.ToShortDateString() + " " + d.HeureCommande.ToShortTimeString(),
                        table = d.TitreTable,
                        client = d.NomsClient,
                        agent = d.NomsAgent,
                        nom = d.ProduitNom,
                        quantite = d.Quantite,
                        prixUnitaire = d.PrixUnitaire,
                        total = d.MontantTotal
                    })
                    .ToList();

                return Json(details);
            }
            catch (Exception ex)
            {
                return Json(new { error = ex.Message });
            }
        }

        [HttpGet("GetHistoriqueReglement")]
        public IActionResult GetHistoriqueReglement()
        {
            try
            {
                var dateDuJour = DateTime.Today;

                var agentIdClaim = User.Claims.FirstOrDefault(c => c.Type == "AgentId");
                if (agentIdClaim == null || !int.TryParse(agentIdClaim.Value, out int agentId))
                {
                    return Json(new { success = false, message = "Agent non identifié." });
                }
                //A verifier
                // Récupérer l'historique des reglements depuis la base de données
                var historique = (
                    from histo in _db.ReglementCommandes
                    join cmd in _db.Commandes on histo.CommandeId equals cmd.CommandeId
                    join regl in _db.Reglements on histo.ReglementId equals regl.ReglementId
                    join devise in _db.Devises on regl.DeviseId equals devise.DeviseId
                    join client in _db.Clients on cmd.ClientId equals client.ClientId
                    join agent in _db.Agents on cmd.AgentId equals agent.AgentId
                    //where regl.AgentId == agentId && regl.DateReglement == dateDuJour
                    where regl.AgentId == agentId//Historique des ventes du jour
                    orderby regl.ReglementId descending
                    select new HistoriqueReglement
                    {
                        Numfact = histo.ReglementId.ToString(),
                        Numcmd = histo.CommandeId.ToString(),
                        DateRegl = regl.DateReglement.ToShortDateString() + " " + regl.HeureReglement.ToShortTimeString(),
                        //Client = TrouverDonneesBydata(cmd.ClientId, "Client"),
                        //Agent = TrouverDonneesBydata(agentId, "Agent"),
                        Devise = devise.SigleDevise,
                        Client = client.NomClient,
                        Agent = agent.NomAgent + " " + agent.PrenomAgent,
                        Montant = histo.MontantAffecte
                    }).Distinct().ToList();
                return Json(historique);
            }
            catch (Exception ex)
            {
                return Json(new { error = ex.Message });
            }
        }
        public class HistoriqueReglement
        {
            public string Numfact { get; set; }
            public string Numcmd { get; set; }
            public string DateRegl { get; set; }
            public string Devise { get; set; }
            public string Client { get; set; }
            public string Agent { get; set; }
            public decimal Montant { get; set; }
        }

        [HttpPost]
        [Route("ValiderReglement")]
        public IActionResult ValiderReglement([FromBody] ReglementData data)
        {
            try
            {
                // Vérifier chaque commande dans la liste
                foreach (var cmndId in data.CommandeId)
                {
                    var reglementpayexist = _db.ReglementCommandes.FirstOrDefault(r => r.CommandeId == int.Parse(cmndId));
                    if (reglementpayexist != null)
                    {
                        return Json(new { success = false, message = $"La commande {cmndId} a déjà été réglée." });
                    }
                }

                var agentIdClaim = User.Claims.FirstOrDefault(c => c.Type == "AgentId");
                if (agentIdClaim == null || !int.TryParse(agentIdClaim.Value, out int agentId))
                {
                    return Json(new { success = false, message = "Agent non identifié." });
                }

                var titrecaisse = _db.Caisses.FirstOrDefault(c => c.PosteTravailId == int.Parse(data.PosteTravailId));

                //Enregistreer le reglememnt dans la base
                Reglement reglement = new()
                {
                    //CommandeId = int.Parse(commandeId), // Ajouter l'ID de la commande ici
                    ModeReglementId = int.Parse(data.ModeReglementId),
                    DeviseId = int.Parse(data.DeviseId),
                    TypeReglementId = int.Parse(data.TypeReglementId),
                    AgentId = agentId,
                    CaisseId = int.Parse(data.PosteTravailId),
                    DateReglement = DateTime.Today,
                    HeureReglement = DateTime.Now,
                    MontantReglement = decimal.Parse(data.MontantReglement),
                    MotifReglement = $"Vente ({titrecaisse.TitreCaisse})"
                };

                _db.Reglements.Add(reglement);
                _db.SaveChanges();

                //Parcourrir les commandes et les enregistrer chacun dans la base avec leurs montant
                foreach (var cmndId in data.CommandeId)
                {
                    ReglementCommande reglementcommande = new()
                    {
                        //CommandeId = int.Parse(commandeId), // Ajouter l'ID de la commande ici
                        CommandeId = int.Parse(cmndId),
                        ReglementId = reglement.ReglementId,
                        MontantAffecte = decimal.Parse(TrouverDonneesBydata(int.Parse(cmndId), "Commande").ToString())
                    };

                    _db.ReglementCommandes.Add(reglementcommande);
                    _db.SaveChanges();

                    //Enregistrer les détails de la facture à cette étape : 
                    //DetailsFacture detailsFacture = new()
                    //{
                    //    Numcmd = int.Parse(commandeId),
                    //    Numfact = reglement.ReglementId,
                    //    Datefact = reglement.DateReglement,
                    //    Heurefact = reglement.HeureReglement,
                    //    Caissier = reglement.AgentId,
                    //    Caisse = reglement.CaisseId,
                    //    //Designation = detailsCommande.ProduitNom,
                    //    //Qte = detailsCommande.Quantite,
                    //    //PU = (decimal)detailsCommande.PrixUnitaire,
                    //    //Montanttotal = reglementcommande.MontantAffecte,
                    //    ModeReglement = _db.ModeReglements.Where(m => m.ModeReglementId == reglement.ModeReglementId).Select(m => m.TitreModeReglement).FirstOrDefault(),
                    //    TypeReglement = _db.TypeReglements.Where(t => t.TypeReglementId == reglement.TypeReglementId).Select(t => t.TitreTypeReglement).FirstOrDefault(),
                    //    Devise = _db.Devises.Where(d => d.DeviseId == reglement.DeviseId).Select(d => d.SigleDevise).FirstOrDefault()
                    //};
                    //_db.DetailsFactures.Add(detailsFacture);
                    //_db.SaveChanges();

                    // Mettre à jour les détails de facture XXX
                    var detailsFactures = _db.DetailsFactures.Where(d => d.Numcmd == reglementcommande.CommandeId).ToList();
                    if (detailsFactures.Any())
                    {
                        foreach (var detailsFacture in detailsFactures)
                        {
                            detailsFacture.Numcmd = reglementcommande.CommandeId;
                            detailsFacture.Numfact = reglementcommande.ReglementId;
                            detailsFacture.Datefact = reglement.DateReglement;
                            detailsFacture.Heurefact = reglement.HeureReglement;
                            detailsFacture.Caissier = reglement.AgentId;
                            detailsFacture.Caisse = reglement.CaisseId;
                            //Designation = detailsCommande.ProduitNom,
                            //Qte = detailsCommande.Quantite,
                            //PU = (decimal)detailsCommande.PrixUnitaire,
                            //Montanttotal = reglementcommande.MontantAffecte,
                            detailsFacture.ModeReglement = _db.ModeReglements.Where(m => m.ModeReglementId == reglement.ModeReglementId).Select(m => m.TitreModeReglement).FirstOrDefault();
                            detailsFacture.TypeReglement = _db.TypeReglements.Where(t => t.TypeReglementId == reglement.TypeReglementId).Select(t => t.TitreTypeReglement).FirstOrDefault();
                            detailsFacture.Devise = _db.Devises.Where(d => d.DeviseId == reglement.DeviseId).Select(d => d.SigleDevise).FirstOrDefault();
                        }

                        _db.DetailsFactures.UpdateRange(detailsFactures);
                        _db.SaveChanges();
                    }
                    else
                    {
                        return Json(new { success = false, message = $"Erreur lors de la mise à jour pour la commande {cmndId}." });
                    }
                }

                TempData["success"] = "Règlement validé avec succès.";
                return Json(new { success = true, id = reglement.ReglementId });
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return Json(new { success = false, message = ex.Message });
            }
        }
        public class RepportData
        {
            public string CommandeId { get; set; }
            public string Client { get; set; }
            public DateTime Datefin { get; set; }
            public DateTime Datedebut { get; set; }
        }
        public IActionResult GenererDonnesRapport([FromBody] RepportData data)
        {
            try
            {
                var agentIdClaim = User.Claims.FirstOrDefault(c => c.Type == "AgentId");
                var agentId = Convert.ToInt32(agentIdClaim.Value);

                //using var transaction = _db.Database.BeginTransaction();
                var reglements = _db.Reglements
                    .Where(r => r.DateReglement >= data.Datedebut
                             && r.DateReglement <= data.Datefin
                             && r.AgentId ==agentId)
                    .Include(r => r.Agent)
                    .Include(r => r.ModeReglement)
                    .Include(r => r.TypeReglement)
                    .Include(r => r.Caisse)
                    .Include(r => r.Devise)
                    .AsNoTracking()
                    .ToList();

                _db.DetailsReglements.RemoveRange(_db.DetailsReglements);
                _db.SaveChanges();

                _db.DetailsReglements.AddRange(reglements.Select(r => new DetailsReglement
                {
                    ReglementId = r.ReglementId,
                    Agent = r.Agent.NomAgent,
                    Caisse = r.Caisse.TitreCaisse,
                    Devise = r.Devise.SigleDevise,
                    DateReglement = r.DateReglement,
                    HeureReglement = r.HeureReglement,
                    ModeReglement = r.ModeReglement.TitreModeReglement,
                    TypeReglement = r.TypeReglement.TitreTypeReglement,
                    MontantReglement = r.MontantReglement,
                    MotifReglement = r.MotifReglement
                }));

                _db.SaveChanges();

                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                // Ajoutez ceci pour voir l'erreur réelle
                var innerException = ex.InnerException?.Message;
                Console.WriteLine($"Erreur interne : {innerException}");
                return Json(new
                {
                    success = false,
                    message = ex.Message
                });
            }
        }
    }
}
