using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AspNetCore.Reporting;
using Hotel.Repository;
using Hotel.Data;

namespace Hotel.Controllers
{
    public class ReportController : Controller
    {
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly IRepositoryDetailsCommande _repositoryDetailsCommande;
        private readonly ApplicationDbContext _db;



        public ReportController(IWebHostEnvironment webHostEnvironment, IRepositoryDetailsCommande repositoryDetailsCommande, ApplicationDbContext db)
        {
            _db = db;
            _webHostEnvironment = webHostEnvironment;
            _repositoryDetailsCommande = repositoryDetailsCommande;
            System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);
        }

        public IActionResult Index()
        {
            return View();
        }

        //[HttpGet]
        //public async Task<IActionResult> Print(int id=0, string? type="")
        //{
        //    return await PrintBonCommande(id, type);
        //}
        //public async Task<IActionResult> PrintBonCommande(int Id, string? Type)
        //{
        //    string mimtype = "";
        //    int extension = 1;
        //    var path = $"{this._webHostEnvironment.WebRootPath}\\ReportFiles\\Boncmd.rdlc";

        //    if (Type.ToUpper() == "ALIMENT")
        //    {
        //        path = $"{this._webHostEnvironment.WebRootPath}\\ReportFiles\\Boncmdaliment.rdlc";

        //    }
        //    else if(Type.ToUpper() == "BOISSON")
        //    {
        //        path = $"{this._webHostEnvironment.WebRootPath}\\ReportFiles\\Boncmdboisson.rdlc";
        //    }
        //    else
        //    {
        //        path = $"{this._webHostEnvironment.WebRootPath}\\ReportFiles\\Boncmdservice.rdlc";
        //    }
        //    Dictionary<string, string> parametres = new Dictionary<string, string>();
        //    parametres.Add("dcmd", Type.ToUpper());
        //    //Recuperer les données
        //    var boncmd = await _repositoryDetailsCommande.GetDetailsCmd(Id, Type);
        //    LocalReport localReport = new LocalReport(path);
        //    localReport.AddDataSource("DataSet1", boncmd);
        //    var result = localReport.Execute(RenderType.Pdf, extension, parametres, mimtype);
        //    return File(result.MainStream, "application/pdf");
        //}

        [HttpGet]
        public async Task<IActionResult> Print(int id = 0, string? type = "")
        {
            try
            {
                return await PrintBonCommande(id, type);
            }
            catch (Exception ex)
            {
                // Log l'erreur ici (utilisez un logger approprié)
                return StatusCode(500, $"Une erreur s'est produite : {ex.Message}");
            }
        }
        
        //OK
        public async Task<IActionResult> PrintBonCommande(int id, string? type)
        {
            // Vérifiez que l'ID est valide
            if (id <= 0)
            {
                return BadRequest("Un ID valide est requis.");
            }

            // Vérifiez que le type est valide
            if (string.IsNullOrEmpty(type))
            {
                return BadRequest("Le type est requis.");
            }

            string mimtype = "";
            int extension = 1;
            var path = $"{this._webHostEnvironment.WebRootPath}\\ReportFiles\\Boncmd.rdlc";

            // Déterminer le chemin du rapport en fonction du type
            switch (type.ToUpper())
            {
                case "ALIMENT":
                    path = $"{this._webHostEnvironment.WebRootPath}\\ReportFiles\\Boncmdaliment.rdlc";
                    break;
                case "BOISSON":
                    path = $"{this._webHostEnvironment.WebRootPath}\\ReportFiles\\Boncmdboisson.rdlc";
                    break;
                default:
                    path = $"{this._webHostEnvironment.WebRootPath}\\ReportFiles\\Boncmdservice.rdlc";
                    break;
            }

            try
            {
                // Récupérer les données
                var boncmd = await _repositoryDetailsCommande.GetDetailsCmd(id, type);

                // Vérifiez si les données sont nulles
                if (boncmd == null)
                {
                    return NotFound($"Aucun bon de commande trouvé pour l'ID {id} et le type {type}.");
                }

                LocalReport localReport = new LocalReport(path);
                localReport.AddDataSource("DataSet1", boncmd);

                // Aucun paramètre n'est envoyé au rapport
                var result = localReport.Execute(RenderType.Pdf, extension, null, mimtype);

                return File(result.MainStream, "application/pdf");
            }
            catch (Exception ex)
            {
                // Log l'erreur ici
                return StatusCode(500, $"Une erreur s'est produite lors du traitement du rapport : {ex.Message}");
            }
        }
        //[HttpGet]
        //public IActionResult PrintPayementFacture(int id)
        //{

        //    return PrintFacture(id);
        //}
        //public IActionResult PrintFacture(int Id)
        //{
        //    string mimtype = "";
        //    int extension = 1;
        //    var path = $"{this._webHostEnvironment.WebRootPath}\\ReportFiles\\FactureRestaurant.rdlc";
        //    Dictionary<string, string> parametres = new Dictionary<string, string>();
        //    parametres.Add("numfact", Id.ToString());
        //    //Recuperer les données
        //    var fact = _repositoryDetailsCommande.GetDetailsFacture(Id);
        //    LocalReport localReport = new LocalReport(path);
        //    localReport.AddDataSource("DataSetFacture", fact);
        //    var result = localReport.Execute(RenderType.Pdf, extension, parametres, mimtype);
        //    return File(result.MainStream, "application/pdf");
        //}

        //[HttpGet]
        //public async Task<IActionResult> PrintPayementFacture(int id=0)
        //{
        //    return await PrintFacture(id);
        //}
        //public async Task<IActionResult> PrintFacture(int Id)
        //{
        //    string mimtype = "";
        //    int extension = 1;
        //    var path = $"{this._webHostEnvironment.WebRootPath}\\ReportFiles\\FactureRestaurant.rdlc";
        //    Dictionary<string, string> parametres = new Dictionary<string, string>();
        //    parametres.Add("numfact", Id.ToString());
        //    //Recuperer les données
        //    var fact = await _repositoryDetailsCommande.GetDetailsFacture(Id);
        //    LocalReport localReport = new LocalReport(path);
        //    localReport.AddDataSource("DataSetFacture", fact);
        //    var result = localReport.Execute(RenderType.Pdf, extension, parametres, mimtype);
        //    return File(result.MainStream, "application/pdf");
        //}

        [HttpGet]
        public async Task<IActionResult> PrintPayementFacture(int id = 0)
        {
            try
            {
                return await PrintFacture(id);
            }
            catch (Exception ex)
            {
                // Log l'erreur ici (utilisez un logger approprié)
                return StatusCode(500, $"Une erreur s'est produite : {ex.Message}");
            }
        }
        //OK
        public async Task<IActionResult> PrintFacture(int id)
        {
            // Vérifiez que l'ID est valide
            if (id <= 0)
            {
                return BadRequest("Un ID valide est requis.");
            }

            string mimtype = "";
            int extension = 1;
            var path = $"{this._webHostEnvironment.WebRootPath}\\ReportFiles\\FactureRestaurant.rdlc";

            try
            {
                // Récupérer les données
                var fact = await _repositoryDetailsCommande.GetDetailsFacture(id);

                // Vérifiez si les données sont nulles
                if (fact == null)
                {
                    return NotFound($"Aucune facture trouvée pour l'ID {id}.");
                }

                LocalReport localReport = new LocalReport(path);
                localReport.AddDataSource("DataSetFacture", fact);

                // Aucun paramètre n'est envoyé au rapport
                var result = localReport.Execute(RenderType.Pdf, extension, null, mimtype);

                return File(result.MainStream, "application/pdf");
            }
            catch (Exception ex)
            {
                // Log l'erreur ici
                return StatusCode(500, $"Une erreur s'est produite lors du traitement du rapport : {ex.Message}");
            }
        }
        //[HttpGet]
        //public async Task<IActionResult> PrintPayementCaution(int id =0)
        //{
        //    return await PrintCaution(id);
        //}
        //public async Task<IActionResult> PrintCaution(int Id)
        //{
        //    string mimtype = "";
        //    int extension = 1;
        //    var path = $"{this._webHostEnvironment.WebRootPath}\\ReportFiles\\FactureCaution.rdlc";
        //    Dictionary<string, string> parametres = new Dictionary<string, string>();
        //    parametres.Add("numfact", Id.ToString());
        //    //Recuperer les données
        //    var fact = await _repositoryDetailsCommande.GetDetailsCaution(Id);
        //    LocalReport localReport = new LocalReport(path);
        //    localReport.AddDataSource("DataSetFactureCaution", fact);
        //    var result = localReport.Execute(RenderType.Pdf, extension, parametres, mimtype);
        //    return File(result.MainStream, "application/pdf");
        //}
        [HttpGet]
        public async Task<IActionResult> PrintPayementCaution(int id = 0)
        {
            try
            {
                return await PrintCaution(id);
            }
            catch (Exception ex)
            {
                // Log l'erreur ici (utilisez un logger approprié)
                return StatusCode(500, $"Une erreur s'est produite : {ex.Message}");
            }
        }

        //OK
        public async Task<IActionResult> PrintCaution(int id)
        {
            // Vérifiez que l'ID est valide
            if (id <= 0)
            {
                return BadRequest("Un ID valide est requis.");
            }

            string mimtype = "";
            int extension = 1;
            var path = $"{this._webHostEnvironment.WebRootPath}\\ReportFiles\\FactureCaution.rdlc";

            try
            {
                // Récupérer les données
                var fact = await _repositoryDetailsCommande.GetDetailsCaution(id);

                // Vérifiez si les données sont nulles
                if (fact == null)
                {
                    return NotFound($"Aucune caution trouvée pour l'ID {id}.");
                }

                LocalReport localReport = new LocalReport(path);
                localReport.AddDataSource("DataSetFactureCaution", fact);

                // Aucun paramètre n'est envoyé au rapport
                var result = localReport.Execute(RenderType.Pdf, extension, null, mimtype);

                return File(result.MainStream, "application/pdf");
            }
            catch (Exception ex)
            {
                // Log l'erreur ici
                return StatusCode(500, $"Une erreur s'est produite lors du traitement du rapport : {ex.Message}");
            }
        }

        //[HttpGet]
        //public async Task<IActionResult> PrintPayementReservation(int id =0)
        //{
        //    return await PrintReservation(id);
        //}
        //public async Task<IActionResult> PrintReservation(int Id)
        //{
        //    string mimtype = "";
        //    int extension = 1;
        //    var path = $"{this._webHostEnvironment.WebRootPath}\\ReportFiles\\FactureReservation.rdlc";
        //    Dictionary<string, string> parametres = new Dictionary<string, string>();
        //    parametres.Add("numfact", Id.ToString());
        //    //Recuperer les données
        //    var fact = await _repositoryDetailsCommande.GetDetailsReservation(Id);
        //    LocalReport localReport = new LocalReport(path);
        //    localReport.AddDataSource("DataSetFactureReservation", fact);
        //    var result = localReport.Execute(RenderType.Pdf, extension, parametres, mimtype);
        //    return File(result.MainStream, "application/pdf");
        //}
        [HttpGet]
        public async Task<IActionResult> PrintPayementReservation(int id = 0)
        {
            try
            {
                return await PrintReservation(id);
            }
            catch (Exception ex)
            {
                // Log l'erreur ici (utilisez un logger approprié)
                return StatusCode(500, $"Une erreur s'est produite : {ex.Message}");
            }
        }

        //OK
        public async Task<IActionResult> PrintReservation(int id)
        {
            // Vérifiez que l'ID est valide
            if (id <= 0)
            {
                return BadRequest("Un ID valide est requis.");
            }

            string mimtype = "";
            int extension = 1;
            var path = $"{this._webHostEnvironment.WebRootPath}\\ReportFiles\\FactureReservation.rdlc";

            try
            {
                // Récupérer les données
                var fact = await _repositoryDetailsCommande.GetDetailsReservation(id);

                // Vérifiez si les données sont nulles
                if (fact == null)
                {
                    return NotFound($"Aucune réservation trouvée pour l'ID {id}.");
                }

                LocalReport localReport = new LocalReport(path);
                localReport.AddDataSource("DataSetFactureReservation", fact);

                // Aucun paramètre n'est envoyé au rapport
                var result = localReport.Execute(RenderType.Pdf, extension, null, mimtype);

                return File(result.MainStream, "application/pdf");
            }
            catch (Exception ex)
            {
                // Log l'erreur ici
                return StatusCode(500, $"Une erreur s'est produite lors du traitement du rapport : {ex.Message}");
            }
        }

        //[HttpGet]
        //public async Task<IActionResult> PrintReglementRapport(string? motif = "", string? debut="", string? fin="")
        //{
        //    return await PrintRapportVentes(motif, debut, fin);
        //}
        //public async Task<IActionResult> PrintRapportVentes(string? motif, string? debut, string? fin)
        //{

        //    string mimtype = "";
        //    int extension = 1;
        //    var path = $"{this._webHostEnvironment.WebRootPath}\\ReportFiles\\RapportReglements.rdlc";
        //    //Dictionary<string, string> parametres = new Dictionary<string, string>();
        //    if(motif is null)
        //    {
        //        motif = "";
        //    } 
        //    if (debut is null)
        //    {
        //        debut = "";
        //    }
        //    if (fin is null)
        //    {
        //        fin = "";
        //    }
        //    //parametres.Add("fin", fin.ToString("dd-MM-yyyy"));
        //    var parametres = new Dictionary<string, string>
        //    {
        //        ["debut"] = debut,
        //        ["fin"] = fin,
        //        ["motif"] = motif
        //    };
        //    //Recuperer les données

        //    var fact = await _repositoryDetailsCommande.GetDetailsReglement();
        //    LocalReport localReport = new LocalReport(path);
        //    localReport.AddDataSource("DatasetDetailsReglement", fact);
        //    var result = localReport.Execute(RenderType.Pdf, extension, parametres, mimtype);
        //    return File(result.MainStream, "application/pdf");
        //}

        //[HttpGet]
        //public async Task<IActionResult> PrintVenteRapport(string debut, string fin, string caissier)
        //{
        //    return await PrintVentes(debut, fin, caissier);
        //}
        //public async Task<IActionResult> PrintVentes(string debut, string fin, string caissier)
        //{
        //    // Vérifiez que tous les paramètres nécessaires sont fournis
        //    if (string.IsNullOrEmpty(debut) || string.IsNullOrEmpty(fin))
        //    {
        //        return BadRequest("Les dates de début et fin sont requises");
        //    }

        //    // Vérifiez le format des dates
        //    if (!DateTime.TryParse(debut, out var dateDebut) || !DateTime.TryParse(fin, out var dateFin))
        //    {
        //        return BadRequest("Format de date invalide");
        //    }
        //    string mimtype = "";
        //    int extension = 1;
        //    var path = $"{this._webHostEnvironment.WebRootPath}\\ReportFiles\\RapportVentes.rdlc";
        //    //Dictionary<string, string> parametres = new Dictionary<string, string>();
        //    if (debut is null)
        //    {
        //        debut = "";
        //    }
        //    if (fin is null)
        //    {
        //        fin = "";
        //    }
        //    if (caissier is null)
        //    {
        //        caissier = "";
        //    }
        //    //parametres.Add("motif", motif.ToUpper());
        //    //parametres.Add("fin", fin.ToString("dd-MM-yyyy"));
        //    var parametres = new Dictionary<string, string>
        //    {
        //        ["debut"] = debut,
        //        ["fin"] = fin,
        //        ["caissier"] = caissier
        //    };
        //    //Recuperer les données
        //    var fact = await _repositoryDetailsCommande.GetDetailsReglement();
        //    LocalReport localReport = new LocalReport(path);
        //    localReport.AddDataSource("DatasetDetailsReglement", fact);
        //    var result = localReport.Execute(RenderType.Pdf, extension, parametres, mimtype);
        //    return File(result.MainStream, "application/pdf");
        //}

        [HttpGet]
        public async Task<IActionResult> PrintVenteRapport(string? debut="", string? fin="", string? caissier="")
        {
            try
            {
                return await PrintVentes(debut, fin, caissier);
            }
            catch (Exception ex)
            {
                // Log l'erreur ici (utilisez un logger approprié)
                return StatusCode(500, $"Une erreur s'est produite : {ex.Message}");
            }
        }

        //OK
        public async Task<IActionResult> PrintVentes(string? debut, string? fin, string? caissier)
        {
            // Vérifiez que tous les paramètres nécessaires sont fournis
            if (string.IsNullOrEmpty(debut) || string.IsNullOrEmpty(fin))
            {
                return BadRequest("Les dates de début et fin sont requises.");
            }

            // Vérifiez le format des dates
            if (!DateTime.TryParse(debut, out var dateDebut) || !DateTime.TryParse(fin, out var dateFin))
            {
                return BadRequest("Format de date invalide.");
            }

            // Initialisation des paramètres
            caissier ??= "";

            string mimtype = "";
            int extension = 1;
            var path = $"{this._webHostEnvironment.WebRootPath}\\ReportFiles\\RapportVentes.rdlc";

            try
            {
                // Récupérer les données
                var fact = await _repositoryDetailsCommande.GetDetailsReglement();

                // Vérifiez si les données sont nulles ou vides
                if (fact == null || !fact.Any())
                {
                    return NotFound("Aucune donnée trouvée pour les paramètres spécifiés.");
                }

                LocalReport localReport = new LocalReport(path);
                localReport.AddDataSource("DatasetDetailsReglement", fact);

                // Aucun paramètre n'est envoyé au rapport
                var result = localReport.Execute(RenderType.Pdf, extension, null, mimtype);

                return File(result.MainStream, "application/pdf");
            }
            catch (Exception ex)
            {
                // Log l'erreur ici
                return StatusCode(500, $"Une erreur s'est produite lors du traitement du rapport : {ex.Message}");
            }
        }

        [HttpGet]
        public async Task<IActionResult> PrintReglementRapport(string? motif = "", string? debut = "", string? fin = "")
        {
            try
            {
                return await PrintRapportVentes(motif, debut, fin);
            }
            catch (Exception ex)
            {
                // Log l'erreur ici (utilisez un logger approprié)
                return StatusCode(500, $"Une erreur s'est produite : {ex.Message}");
            }
        }

        //OK
        public async Task<IActionResult> PrintRapportVentes(string? motif, string? debut, string? fin)
        {
            string mimtype = "";
            int extension = 1;
            var path = $"{this._webHostEnvironment.WebRootPath}\\ReportFiles\\RapportReglements.rdlc";

            try
            {
                // Récupérer les données
                var fact = await _repositoryDetailsCommande.GetDetailsReglement();

                // Vérifiez si les données sont nulles ou vides
                if (fact == null || !fact.Any())
                {
                    return NotFound("Aucune donnée trouvée pour les paramètres spécifiés.");
                }

                LocalReport localReport = new LocalReport(path);
                localReport.AddDataSource("DatasetDetailsReglement", fact);

                // Aucun paramètre n'est envoyé au rapport
                var result = localReport.Execute(RenderType.Pdf, extension, null, mimtype);

                return File(result.MainStream, "application/pdf");
            }
            catch (NullReferenceException nullEx)
            {
                // Gérer les exceptions de référence nulle
                return BadRequest($"Erreur de données : {nullEx.Message}");
            }
            catch (Exception ex)
            {
                // Log l'erreur ici
                return StatusCode(500, $"Une erreur s'est produite lors du traitement du rapport : {ex.Message}");
            }
        }


        [HttpGet]
        public async Task<IActionResult> PrintDemandes(string? agent = "", string? type = "", string? etat = "", string? date = "")
        {

            try
            {
                return await PrintRapportDemandes(agent,type, etat, date);
            }
            catch (Exception ex)
            {
                // Log l'erreur ici (utilisez un logger approprié)
                return StatusCode(500, $"Une erreur s'est produite : {ex.Message}");
            }
        }
        
        //OK
        public async Task<IActionResult> PrintRapportDemandes(string? agent, string? type, string? etat, string? date)
        {
            string mimtype = "";
            int extension = 1;
            var path = $"{this._webHostEnvironment.WebRootPath}\\ReportFiles\\RapportDemandes.rdlc";

            try
            {
                // Récupérer les données
                var fact = await _repositoryDetailsCommande.GetDetailsDemande(agent, type, etat,  date);

                // Vérifiez si les données sont nulles ou vides
                if (fact == null || !fact.Any())
                {
                    return NotFound("Aucune donnée trouvée pour les paramètres spécifiés.");
                }

                LocalReport localReport = new LocalReport(path);
                localReport.AddDataSource("DatasetDetailsDemande", fact);

                // Aucun paramètre n'est envoyé au rapport
                var result = localReport.Execute(RenderType.Pdf, extension, null, mimtype);

                return File(result.MainStream, "application/pdf");
            }
            catch (NullReferenceException nullEx)
            {
                // Gérer les exceptions de référence nulle
                return BadRequest($"Erreur de données : {nullEx.Message}");
            }
            catch (Exception ex)
            {
                // Log l'erreur ici
                return StatusCode(500, $"Une erreur s'est produite lors du traitement du rapport : {ex.Message}");
            }
        }
        //OK
        [HttpGet]
        public async Task<IActionResult> PrintDemandesOne(int demandeId = 0)
        {
            try
            {
                return await PrintRapportDemandesOne(demandeId);
            }
            catch (Exception ex)
            {
                // Log l'erreur ici (utilisez un logger approprié)
                return StatusCode(500, $"Une erreur s'est produite : {ex.Message}");
            }
        }
        //OK
        public async Task<IActionResult> PrintRapportDemandesOne(int demandeId)
        {
            string mimtype = "";
            int extension = 1;
            var path = $"{this._webHostEnvironment.WebRootPath}\\ReportFiles\\RapportDemandesOne.rdlc";

            try
            {
                // Récupérer les données
                var fact = await _repositoryDetailsCommande.GetDetailsDemandeOne(demandeId);

                // Vérifiez si les données sont nulles ou vides
                if (fact == null || !fact.Any())
                {
                    return NotFound("Aucune donnée trouvée pour les paramètres spécifiés.");
                }

                LocalReport localReport = new LocalReport(path);
                localReport.AddDataSource("DatasetDetailsDemande", fact);

                // Aucun paramètre n'est envoyé au rapport
                var result = localReport.Execute(RenderType.Pdf, extension, null, mimtype);

                return File(result.MainStream, "application/pdf");
            }
            catch (NullReferenceException nullEx)
            {
                // Gérer les exceptions de référence nulle
                return BadRequest($"Erreur de données : {nullEx.Message}");
            }
            catch (Exception ex)
            {
                // Log l'erreur ici
                return StatusCode(500, $"Une erreur s'est produite lors du traitement du rapport : {ex.Message}");
            }
        }


        //O
        [HttpGet]
        public async Task<IActionResult> PrintLivraisons(string? agent = "", string? etat = "", string? date = "")
        {

            try
            {
                return await PrintRapportLivraisons(agent, etat, date);
            }
            catch (Exception ex)
            {
                // Log l'erreur ici (utilisez un logger approprié)
                return StatusCode(500, $"Une erreur s'est produite : {ex.Message}");
            }
        }

        //O
        public async Task<IActionResult> PrintRapportLivraisons(string? agent, string? etat, string? date)
        {
            string mimtype = "";
            int extension = 1;
            var path = $"{this._webHostEnvironment.WebRootPath}\\ReportFiles\\RapportLivraisons.rdlc";

            try
            {
                // Récupérer les données
                var fact = await _repositoryDetailsCommande.GetDetailsLivraison(agent, etat, date);

                // Vérifiez si les données sont nulles ou vides
                if (fact == null || !fact.Any())
                {
                    return NotFound("Aucune donnée trouvée pour les paramètres spécifiés.");
                }

                LocalReport localReport = new LocalReport(path);
                localReport.AddDataSource("DatasetDetailsLivraison", fact);

                // Aucun paramètre n'est envoyé au rapport
                var result = localReport.Execute(RenderType.Pdf, extension, null, mimtype);

                return File(result.MainStream, "application/pdf");
            }
            catch (NullReferenceException nullEx)
            {
                // Gérer les exceptions de référence nulle
                return BadRequest($"Erreur de données : {nullEx.Message}");
            }
            catch (Exception ex)
            {
                // Log l'erreur ici
                return StatusCode(500, $"Une erreur s'est produite lors du traitement du rapport : {ex.Message}");
            }
        }

        //OK
        [HttpGet]
        public async Task<IActionResult> PrintLivraisonOne(int demandeId = 0)
        {
            try
            {
                return await PrintRapportLivraisonOne(demandeId);
            }
            catch (Exception ex)
            {
                // Log l'erreur ici (utilisez un logger approprié)
                return StatusCode(500, $"Une erreur s'est produite : {ex.Message}");
            }
        }
        //OK
        public async Task<IActionResult> PrintRapportLivraisonOne(int demandeId)
        {
            string mimtype = "";
            int extension = 1;
            var path = $"{this._webHostEnvironment.WebRootPath}\\ReportFiles\\RapportLivraisonsOne.rdlc";

            try
            {
                // Récupérer les données
                var fact = await _repositoryDetailsCommande.GetDetailsLivraisonOne(demandeId);

                // Vérifiez si les données sont nulles ou vides
                if (fact == null || !fact.Any())
                {
                    return NotFound("Aucune donnée trouvée pour les paramètres spécifiés.");
                }

                LocalReport localReport = new LocalReport(path);
                localReport.AddDataSource("DatasetDetailsLivraison", fact);

                // Aucun paramètre n'est envoyé au rapport
                var result = localReport.Execute(RenderType.Pdf, extension, null, mimtype);

                return File(result.MainStream, "application/pdf");
            }
            catch (NullReferenceException nullEx)
            {
                // Gérer les exceptions de référence nulle
                return BadRequest($"Erreur de données : {nullEx.Message}");
            }
            catch (Exception ex)
            {
                // Log l'erreur ici
                return StatusCode(500, $"Une erreur s'est produite lors du traitement du rapport : {ex.Message}");
            }
        }



        //O
        [HttpGet]
        public async Task<IActionResult> PrintSorties(string? agent = "", string? etat = "", string? date = "")
        {

            try
            {
                return await PrintRapportorties(agent, etat, date);
            }
            catch (Exception ex)
            {
                // Log l'erreur ici (utilisez un logger approprié)
                return StatusCode(500, $"Une erreur s'est produite : {ex.Message}");
            }
        }

        //O
        public async Task<IActionResult> PrintRapportorties(string? agent, string? etat, string? date)
        {
            string mimtype = "";
            int extension = 1;
            var path = $"{this._webHostEnvironment.WebRootPath}\\ReportFiles\\RapportSorties.rdlc";

            try
            {
                // Récupérer les données
                var fact = await _repositoryDetailsCommande.GetDetailsLivraison(agent, etat, date);

                // Vérifiez si les données sont nulles ou vides
                if (fact == null || !fact.Any())
                {
                    return NotFound("Aucune donnée trouvée pour les paramètres spécifiés.");
                }

                LocalReport localReport = new LocalReport(path);
                localReport.AddDataSource("DatasetDetailsSortie", fact);

                // Aucun paramètre n'est envoyé au rapport
                var result = localReport.Execute(RenderType.Pdf, extension, null, mimtype);

                return File(result.MainStream, "application/pdf");
            }
            catch (NullReferenceException nullEx)
            {
                // Gérer les exceptions de référence nulle
                return BadRequest($"Erreur de données : {nullEx.Message}");
            }
            catch (Exception ex)
            {
                // Log l'erreur ici
                return StatusCode(500, $"Une erreur s'est produite lors du traitement du rapport : {ex.Message}");
            }
        }
        //OK
        [HttpGet]
        public async Task<IActionResult> PrintSortieOne(int demandeId = 0)
        {
            try
            {
                return await PrintRapportSortieOne(demandeId);
            }
            catch (Exception ex)
            {
                // Log l'erreur ici (utilisez un logger approprié)
                return StatusCode(500, $"Une erreur s'est produite : {ex.Message}");
            }
        }
        //OK
        public async Task<IActionResult> PrintRapportSortieOne(int demandeId)
        {
            string mimtype = "";
            int extension = 1;
            var path = $"{this._webHostEnvironment.WebRootPath}\\ReportFiles\\RapportSortiesOne.rdlc";

            try
            {
                // Récupérer les données
                var fact = await _repositoryDetailsCommande.GetDetailsSortieOne(demandeId);

                // Vérifiez si les données sont nulles ou vides
                if (fact == null || !fact.Any())
                {
                    return NotFound("Aucune donnée trouvée pour les paramètres spécifiés.");
                }

                LocalReport localReport = new LocalReport(path);
                localReport.AddDataSource("DatasetDetailsSortie", fact);

                // Aucun paramètre n'est envoyé au rapport
                var result = localReport.Execute(RenderType.Pdf, extension, null, mimtype);

                return File(result.MainStream, "application/pdf");
            }
            catch (NullReferenceException nullEx)
            {
                // Gérer les exceptions de référence nulle
                return BadRequest($"Erreur de données : {nullEx.Message}");
            }
            catch (Exception ex)
            {
                // Log l'erreur ici
                return StatusCode(500, $"Une erreur s'est produite lors du traitement du rapport : {ex.Message}");
            }
        }


        //OK
        [HttpGet]
        public async Task<IActionResult> PrintDeclassements(string? agent = "", string? motif = "", string? date = "")
        {

            try
            {
                return await PrintRapportDeclassements(agent, motif, date);
            }
            catch (Exception ex)
            {
                // Log l'erreur ici (utilisez un logger approprié)
                return StatusCode(500, $"Une erreur s'est produite : {ex.Message}");
            }
        }

        //OK
        public async Task<IActionResult> PrintRapportDeclassements(string? agent, string? motif, string? date)
        {
            string mimtype = "";
            int extension = 1;
            var path = $"{this._webHostEnvironment.WebRootPath}\\ReportFiles\\RapportDeclassements.rdlc";

            try
            {
                // Récupérer les données
                var fact = await _repositoryDetailsCommande.GetDetailsDeclassement(agent, motif, date);

                // Vérifiez si les données sont nulles ou vides
                if (fact == null || !fact.Any())
                {
                    return NotFound("Aucune donnée trouvée pour les paramètres spécifiés.");
                }

                LocalReport localReport = new LocalReport(path);
                localReport.AddDataSource("DataSetDetailsDeclassement", fact);

                // Aucun paramètre n'est envoyé au rapport
                var result = localReport.Execute(RenderType.Pdf, extension, null, mimtype);

                return File(result.MainStream, "application/pdf");
            }
            catch (NullReferenceException nullEx)
            {
                // Gérer les exceptions de référence nulle
                return BadRequest($"Erreur de données : {nullEx.Message}");
            }
            catch (Exception ex)
            {
                // Log l'erreur ici
                return StatusCode(500, $"Une erreur s'est produite lors du traitement du rapport : {ex.Message}");
            }
        }
        //OK
        [HttpGet]
        public async Task<IActionResult> PrintDeclassementsOne(int declassementId = 0)
        {
            try
            {
                return await PrintRapportDeclassementsOne(declassementId);
            }
            catch (Exception ex)
            {
                // Log l'erreur ici (utilisez un logger approprié)
                return StatusCode(500, $"Une erreur s'est produite : {ex.Message}");
            }
        }
        //OK
        public async Task<IActionResult> PrintRapportDeclassementsOne(int declassementId)
        {
            string mimtype = "";
            int extension = 1;
            var path = $"{this._webHostEnvironment.WebRootPath}\\ReportFiles\\RapportDeclassementsOne.rdlc";

            try
            {
                // Récupérer les données
                var fact = await _repositoryDetailsCommande.GetDetailsDeclassementOne(declassementId);

                // Vérifiez si les données sont nulles ou vides
                if (fact == null || !fact.Any())
                {
                    return NotFound("Aucune donnée trouvée pour les paramètres spécifiés.");
                }

                LocalReport localReport = new LocalReport(path);
                localReport.AddDataSource("DataSetDetailsDeclassement", fact);

                // Aucun paramètre n'est envoyé au rapport
                var result = localReport.Execute(RenderType.Pdf, extension, null, mimtype);

                return File(result.MainStream, "application/pdf");
            }
            catch (NullReferenceException nullEx)
            {
                // Gérer les exceptions de référence nulle
                return BadRequest($"Erreur de données : {nullEx.Message}");
            }
            catch (Exception ex)
            {
                // Log l'erreur ici
                return StatusCode(500, $"Une erreur s'est produite lors du traitement du rapport : {ex.Message}");
            }
        }


        //OK
        [HttpGet]
        public async Task<IActionResult> PrintMouvementRapport()
        {

            try
            {
                return await PrintRapportMouvements();
            }
            catch (Exception ex)
            {
                // Log l'erreur ici (utilisez un logger approprié)
                return StatusCode(500, $"Une erreur s'est produite : {ex.Message}");
            }
        }

        //OK
        public async Task<IActionResult> PrintRapportMouvements()
        {
            string mimtype = "";
            int extension = 1;
            var path = $"{this._webHostEnvironment.WebRootPath}\\ReportFiles\\RapportMouvements.rdlc";

            try
            {
                // Récupérer les données
                var fact = await _repositoryDetailsCommande.GetDetailsMouvement();

                // Vérifiez si les données sont nulles ou vides
                if (fact == null || !fact.Any())
                {
                    return NotFound("Aucune donnée trouvée pour les paramètres spécifiés.");
                }

                LocalReport localReport = new LocalReport(path);
                localReport.AddDataSource("DataSetDetailsMouvement", fact);

                // Aucun paramètre n'est envoyé au rapport
                var result = localReport.Execute(RenderType.Pdf, extension, null, mimtype);

                return File(result.MainStream, "application/pdf");
            }
            catch (NullReferenceException nullEx)
            {
                // Gérer les exceptions de référence nulle
                return BadRequest($"Erreur de données : {nullEx.Message}");
            }
            catch (Exception ex)
            {
                // Log l'erreur ici
                return StatusCode(500, $"Une erreur s'est produite lors du traitement du rapport : {ex.Message}");
            }
        }
    }
}
