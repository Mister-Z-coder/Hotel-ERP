using Dapper;
using Hotel.Data;
//using Hotel.DataSource;
using Hotel.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace Hotel.Repository
{
    public class DetailsCommandeRepository : IRepositoryDetailsCommande
    {
        private readonly ApplicationDbContext _db;

        public DetailsCommandeRepository(ApplicationDbContext db)
        {
            _db = db;
        }
        //OK
        public async Task<IEnumerable<DetailsCommande>> GetDetailsCmd(int CmdId, string TypeProd)
        {
            // Requête SQL avec les paramètres @CmdId et @ProdType
            var query = @"
        SELECT 
        CommandeId, 
        DateCommande,
        DetailsCommandeId,
        HeureCommande,
        NomsAgent, 
        NomsClient,
        PrixUnitaire,
        ProduitNom, 
        Quantite,
        TitreTable,
        TypeCommande,
        TypeProduit
        FROM DetailsCommandes
        WHERE CommandeId = @CmdId AND TypeProduit = @TypeProd";

            try
            {
                using (var connection = _db.GetConnection())
                {
                    await connection.OpenAsync(); // Ouvrir la connexion de manière asynchrone

                    // Exécuter la requête avec les paramètres CmdId et ProdType
                    var result = await connection.QueryAsync<DetailsCommande>(query, new { CmdId, TypeProd });

                    return result;
                }
            }
            catch (Exception ex)
            {
                // Loguer l'erreur ou la relancer
                throw new ApplicationException("Erreur lors de la récupération des détails de la commande", ex);
            }
        }
        //OK
        public async Task<IEnumerable<DetailsFacture>> GetDetailsFacture(int FactId)
        {
            // Requête SQL avec les paramètres @CmdId et @ProdType
            var query = @"
            SELECT 
            df.Caisse,
            df.Caissier, 
            df.Datefact,
            df.Designation,
            df.DetailsFactureId,
            df.Devise, 
            df.Heurefact,
            df.ModeReglement,
            df.Numcmd,
            df.Numfact,
            df.PU,
            df.Qte,
            df.TypeReglement,
            df.Montanttotal
            FROM 
                DetailsFactures df WHERE df.Numfact=@FactId";

            try
            {
                using (var connection = _db.GetConnection())
                {
                    await connection.OpenAsync(); // Ouvrir la connexion de manière asynchrone

                    // Exécuter la requête avec les paramètres CmdId et ProdType
                    var result = await connection.QueryAsync<DetailsFacture>(query, new { FactId });

                    return result;
                }
            }
            catch (Exception ex)
            {
                // Loguer l'erreur ou la relancer
                throw new ApplicationException("Erreur lors de la récupération des détails de la facture", ex);
            }
        }
        //OK
        public async Task<IEnumerable<DetailsFactureCaution>> GetDetailsCaution(int FactId)
        {
            // Requête SQL avec les paramètres @CmdId et @ProdType
            var query = @"
                SELECT 
            df.Caisse,
            df.Caissier,
            df.Chambre,
            df.Datefact, 
            df.Debut,
            df.DetailsFactureCautionId,
            df.Devise,
            df.Duree,
            df.Fin,
            df.Heurefact,
            df.ModeReglement,
            df.Montant,
            df.Numfact, 
            df.Numres, 
            df.TypeReglement
            FROM 
                DetailsFactureCautions df WHERE df.Numfact=@FactId";

            try
            {
                using (var connection = _db.GetConnection())
                {
                    await connection.OpenAsync(); // Ouvrir la connexion de manière asynchrone

                    // Exécuter la requête avec les paramètres CmdId et ProdType
                    var result = await connection.QueryAsync<DetailsFactureCaution>(query, new { FactId });

                    return result;
                }
            }
            catch (Exception ex)
            {
                // Loguer l'erreur ou la relancer
                throw new ApplicationException("Erreur lors de la récupération des détails de la facture", ex);
            }
        }
        //OK
        public async Task<IEnumerable<DetailsFactureReservation>> GetDetailsReservation(int FactId)
        {
            // Requête SQL avec les paramètres @CmdId et @ProdType
            var query = @"
            SELECT 
            df.Caisse,
            df.Caissier,
            df.Chambre, 
            df.Datefact,
            df.Debut,
            df.DetailsFactureReservationId,
            df.Devise,
            df.Duree,
            df.Fin,
            df.Heurefact,
            df.ModeReglement,
            df.Montant,
            df.Numfact, 
            df.Numres, 
            df.Service, 
            df.TypeReglement
            FROM 
                DetailsFactureReservations df WHERE df.Numfact=@FactId";

            try
            {
                using (var connection = _db.GetConnection())
                {
                    await connection.OpenAsync(); // Ouvrir la connexion de manière asynchrone

                    // Exécuter la requête avec les paramètres CmdId et ProdType
                    var result = await connection.QueryAsync<DetailsFactureReservation>(query, new { FactId });

                    return result;
                }
            }
            catch (Exception ex)
            {
                // Loguer l'erreur ou la relancer
                throw new ApplicationException("Erreur lors de la récupération des détails de la facture", ex);
            }
        }
        //OK
        public async Task<IEnumerable<DetailsReglement>> GetDetailsReglement()
        {
            const string query = @"
        SELECT 
            Agent,
            Caisse = CAST(Caisse AS NVARCHAR(100)), -- Conversion explicite
            DateReglement,
            DetailsReglementId,
            Devise,
            HeureReglement,
            ModeReglement,
            MontantReglement,
            MotifReglement,
            ReglementId,
            TypeReglement
        FROM DetailsReglements";

            try
            {
                using var connection = _db.GetConnection();
                var result = await connection.QueryAsync<DetailsReglement>(query);
                return result;
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Erreur lors de la récupération des détails du rapport", ex);
            }
        }

        public async Task<IEnumerable<DetailsReglement>> GetDetailsVentes()
        {
            const string query = @"
        SELECT 
            Agent,
            Caisse = CAST(Caisse AS NVARCHAR(100)), -- Conversion explicite
            DateReglement,
            DetailsReglementId,
            Devise,
            HeureReglement,
            ModeReglement,
            MontantReglement,
            MotifReglement,
            ReglementId,
            TypeReglement
        FROM DetailsReglements";

            try
            {
                using var connection = _db.GetConnection();
                var result = await connection.QueryAsync<DetailsReglement>(query);
                return result;
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Erreur lors de la récupération des détails du rapport", ex);
            }
        }


        //OK
        public async Task<IEnumerable<DetailsDemande>> GetDetailsDemande(string agent, string type, string etat, string date)
        {
            // Requête SQL avec les paramètres @CmdId et @ProdType
            var query = "";
            if (agent is not null && type is not null && etat is not null && date is not null)
            {
                query = @"
        SELECT 
        dd.DetailsDemandeId,
        dd.DemandeId,
        dd.Agent,
        dd.DateDemande,
        dd.TypeDemande,
        dd.EtatDemande,
        dd.ProduitDemande,
        dd.QteDemande
        FROM 
            DetailsDemandes dd WHERE dd.Agent = @agent AND dd.TypeDemande = @type AND dd.EtatDemande = @etat AND dd.DateDemande = @date";

            }
            else
            {
                query = @"
        SELECT 
        dd.DetailsDemandeId,
        dd.DemandeId,
        dd.Agent,
        dd.DateDemande,
        dd.TypeDemande,
        dd.EtatDemande,
        dd.ProduitDemande,
        dd.QteDemande
        FROM 
            DetailsDemandes dd WHERE dd.Agent = @agent OR dd.TypeDemande = @type OR dd.EtatDemande = @etat OR dd.DateDemande = @date";

            }
            try
            {
                using (var connection = _db.GetConnection())
                {
                    await connection.OpenAsync(); // Ouvrir la connexion de manière asynchrone

                    // Exécuter la requête avec les paramètres CmdId et ProdType
                    var result = await connection.QueryAsync<DetailsDemande>(query, new { agent,type,etat,date });

                    return result;
                }
            }
            catch (Exception ex)
            {
                // Loguer l'erreur ou la relancer
                throw new ApplicationException("Erreur lors de la récupération des détails du rapport", ex);
            }
        }

        //OK
        public async Task<IEnumerable<DetailsDemande>> GetDetailsDemandeOne(int demandeId)
        {
            // Requête SQL avec les paramètres @CmdId et @ProdType
            var query = @"
        SELECT 
        dd.DetailsDemandeId,
        dd.DemandeId,
        dd.Agent,
        dd.DateDemande,
        dd.TypeDemande,
        dd.EtatDemande,
        dd.ProduitDemande,
        dd.QteDemande
        FROM 
            DetailsDemandes dd WHERE dd.DemandeId=@demandeId";

            
            try
            {
                using (var connection = _db.GetConnection())
                {
                    await connection.OpenAsync(); // Ouvrir la connexion de manière asynchrone

                    // Exécuter la requête avec les paramètres CmdId et ProdType
                    var result = await connection.QueryAsync<DetailsDemande>(query, new { demandeId });

                    return result;
                }
            }
            catch (Exception ex)
            {
                // Loguer l'erreur ou la relancer
                throw new ApplicationException("Erreur lors de la récupération des détails du rapport", ex);
            }
        }

        //O
        public async Task<IEnumerable<DetailsLivraison>> GetDetailsLivraison(string agent, string etat, string date)
        {
            // Requête SQL avec les paramètres @CmdId et @ProdType
            var query = "";
            if (agent is not null && etat is not null && date is not null)
            {
                query = @"
        SELECT 
        dl.DetailsLivraisonId,
      dl.LivraisonId,
      dl.Agent,
      dl.TypeMouvement,
      dl.DateMouvement,
      dl.HeureMouvement,
      dl.NatureMouvement,
      dl.ApprovisionnementId,
      dl.Fournisseur,
      dl.MontantLivraison,
      dl.DateLivraison,
      dl.HeureLivraison,
      dl.ProduitLivre,
      dl.QteLivre
        FROM 
            DetailsLivraisons dl WHERE dl.Agent = @agent AND dl.EtatDemande = @etat AND dl.DateDemande = @date";

            }
            else
            {
                query = @"
        SELECT 
        dl.DetailsLivraisonId,
      dl.LivraisonId,
      dl.Agent,
      dl.TypeMouvement,
      dl.DateMouvement,
      dl.HeureMouvement,
      dl.NatureMouvement,
      dl.ApprovisionnementId,
      dl.Fournisseur,
      dl.MontantLivraison,
      dl.DateLivraison,
      dl.HeureLivraison,
      dl.ProduitLivre,
      dl.QteLivre
        FROM 
            DetailsLivraisons dl WHERE dl.Agent = @agent OR dl.EtatDemande = @etat OR dl.DateDemande = @date";

            }
            try
            {
                using (var connection = _db.GetConnection())
                {
                    await connection.OpenAsync(); // Ouvrir la connexion de manière asynchrone

                    // Exécuter la requête avec les paramètres CmdId et ProdType
                    var result = await connection.QueryAsync<DetailsLivraison>(query, new { agent, etat, date });

                    return result;
                }
            }
            catch (Exception ex)
            {
                // Loguer l'erreur ou la relancer
                throw new ApplicationException("Erreur lors de la récupération des détails du rapport", ex);
            }
        }

        //O
        public async Task<IEnumerable<DetailsLivraison>> GetDetailsLivraisonOne(int demandeId)
        {
            // Requête SQL avec les paramètres @CmdId et @ProdType
            var query = @"
        SELECT 
        dl.DetailsLivraisonId,
      dl.LivraisonId,
      dl.Agent,
      dl.TypeMouvement,
      dl.DateMouvement,
      dl.HeureMouvement,
      dl.NatureMouvement,
      dl.ApprovisionnementId,
      dl.Fournisseur,
      dl.MontantLivraison,
      dl.DateLivraison,
      dl.HeureLivraison,
      dl.ProduitLivre,
      dl.QteLivre
        FROM 
            DetailsLivraisons dl WHERE dl.ApprovisionnementId=@demandeId";


            try
            {
                using (var connection = _db.GetConnection())
                {
                    await connection.OpenAsync(); // Ouvrir la connexion de manière asynchrone

                    // Exécuter la requête avec les paramètres CmdId et ProdType
                    var result = await connection.QueryAsync<DetailsLivraison>(query, new { demandeId });

                    return result;
                }
            }
            catch (Exception ex)
            {
                // Loguer l'erreur ou la relancer
                throw new ApplicationException("Erreur lors de la récupération des détails du rapport", ex);
            }
        }


        //O
        public async Task<IEnumerable<DetailsSortie>> GetDetailsSortie(string agent, string etat, string date)
        {
            // Requête SQL avec les paramètres @CmdId et @ProdType
            var query = "";
            if (agent is not null && etat is not null && date is not null)
            {
                query = @"
        SELECT 
        dl.DetailsLivraisonId,
      dl.LivraisonId,
      dl.Agent,
      dl.TypeMouvement,
      dl.DateMouvement,
      dl.HeureMouvement,
      dl.NatureMouvement,
      dl.ApprovisionnementId,
      dl.Fournisseur,
      dl.MontantLivraison,
      dl.DateLivraison,
      dl.HeureLivraison,
      dl.ProduitLivre,
      dl.QteLivre
        FROM 
            DetailsLivraisons dl WHERE dl.Agent = @agent AND dl.EtatDemande = @etat AND dl.DateDemande = @date";

            }
            else
            {
                query = @" 
        SELECT 
        ds.DetailsSortieId,
        ds.SortieId,
        ds.Agent,
        ds.TypeMouvement,
        ds.DateMouvement, 
        ds.HeureMouvement,
        ds.NatureMouvement,
        ds.AjoutId,
        ds.MotifAjout,
        ds.ProduitLivre,
        ds.QteLivre
        FROM 
            DetailsSorties ds WHERE ds WHERE ds.Agent = @agent OR ds.EtatDemande = @etat OR ds.DateDemande = @date";

            }
            try
            {
                using (var connection = _db.GetConnection())
                {
                    await connection.OpenAsync(); // Ouvrir la connexion de manière asynchrone

                    // Exécuter la requête avec les paramètres CmdId et ProdType
                    var result = await connection.QueryAsync<DetailsSortie>(query, new { agent, etat, date });

                    return result;
                }
            }
            catch (Exception ex)
            {
                // Loguer l'erreur ou la relancer
                throw new ApplicationException("Erreur lors de la récupération des détails du rapport", ex);
            }
        }

        //O
        public async Task<IEnumerable<DetailsSortie>> GetDetailsSortieOne(int demandeId)
        {
            
        var query = @"
        SELECT 
        ds.DetailsSortieId,
        ds.SortieId,
        ds.Agent,
        ds.TypeMouvement,
        ds.DateMouvement, 
        ds.HeureMouvement,
        ds.NatureMouvement,
        ds.AjoutId,
        ds.MotifAjout,
        ds.ProduitLivre,
        ds.QteLivre
        FROM 
            DetailsSorties ds WHERE ds.AjoutId=@demandeId";


            try
            {
                using (var connection = _db.GetConnection())
                {
                    await connection.OpenAsync(); // Ouvrir la connexion de manière asynchrone

                    // Exécuter la requête avec les paramètres CmdId et ProdType
                    var result = await connection.QueryAsync<DetailsSortie>(query, new { demandeId });

                    return result;
                }
            }
            catch (Exception ex)
            {
                // Loguer l'erreur ou la relancer
                throw new ApplicationException("Erreur lors de la récupération des détails du rapport", ex);
            }
        }

        //OK
        public async Task<IEnumerable<DetailsDeclassement>> GetDetailsDeclassement(string agent, string motif, string date)
        {
            // Requête SQL avec les paramètres @CmdId et @ProdType
            var query = "";
            if (agent is not null && motif is not null && date is not null)
            {
                query = @"
        SELECT 
    dc.DetailsDeclassementId,
    dc.DeclassementId,
    dc.Agent,
    dc.TypeMouvement,
    dc.DateMouvement,
    dc.HeureMouvement,
    dc.NatureMouvement,
    dc.MotifDeclassement,
    dc.ProduitDeclasse,
    dc.QteDeclasse
FROM 
    DetailsDeclassements dc
WHERE 
    dc.Agent = @agent 
    AND dc.MotifDeclassement LIKE '%' + @motif + '%' 
    AND dc.DateMouvement = @date";
            }
            else
            {
                query = @"
        SELECT 
    dc.DetailsDeclassementId,
    dc.DeclassementId,
    dc.Agent,
    dc.TypeMouvement,
    dc.DateMouvement,
    dc.HeureMouvement,
    dc.NatureMouvement,
    dc.MotifDeclassement,
    dc.ProduitDeclasse,
    dc.QteDeclasse
FROM 
    DetailsDeclassements dc
WHERE 
    dc.Agent = @agent 
    OR dc.MotifDeclassement LIKE '%' + @motif + '%' 
    OR dc.DateMouvement = @date";
            }
            try
            {
                using (var connection = _db.GetConnection())
                {
                    await connection.OpenAsync(); // Ouvrir la connexion de manière asynchrone

                    // Exécuter la requête avec les paramètres CmdId et ProdType
                    var result = await connection.QueryAsync<DetailsDeclassement>(query, new { agent, motif, date });

                    return result;
                }
            }
            catch (Exception ex)
            {
                // Loguer l'erreur ou la relancer
                throw new ApplicationException("Erreur lors de la récupération des détails du rapport", ex);
            }
        }

        //OK
        public async Task<IEnumerable<DetailsDeclassement>> GetDetailsDeclassementOne(int declassementId)
        {
            // Requête SQL avec les paramètres @CmdId et @ProdType
            var query = @"
        SELECT 
        dc.DetailsDeclassementId,
      dc.DeclassementId,
      dc.Agent,
      dc.TypeMouvement,
      dc.DateMouvement,
      dc.HeureMouvement,
      dc.NatureMouvement,
      dc.MotifDeclassement,
      dc.ProduitDeclasse,
      dc.QteDeclasse
        FROM 
            DetailsDeclassements dc WHERE dc.DeclassementId=@declassementId";


            try
            {
                using (var connection = _db.GetConnection())
                {
                    await connection.OpenAsync(); // Ouvrir la connexion de manière asynchrone

                    // Exécuter la requête avec les paramètres CmdId et ProdType
                    var result = await connection.QueryAsync<DetailsDeclassement>(query, new { declassementId });

                    return result;
                }
            }
            catch (Exception ex)
            {
                // Loguer l'erreur ou la relancer
                throw new ApplicationException("Erreur lors de la récupération des détails du rapport", ex);
            }
        }


        public async Task<IEnumerable<DetailsMouvement>> GetDetailsMouvement()
        {
            // Requête SQL avec les paramètres @CmdId et @ProdType
            var query = @"
        SELECT 
      dm.DetailsMouvementId,
      dm.MouvementId,
      dm.Agent,
      dm.TypeMouvement,
      dm.DateMouvement,
      dm.HeureMouvement,
      dm.NatureMouvement,
      dm.ProduitMouvement,
      dm.QteMouvement
    FROM 
        DetailsMouvements dm";
            
            
            try
            {
                using (var connection = _db.GetConnection())
                {
                    await connection.OpenAsync(); // Ouvrir la connexion de manière asynchrone

                    // Exécuter la requête avec les paramètres CmdId et ProdType
                    var result = await connection.QueryAsync<DetailsMouvement>(query);

                    return result;
                }
            }
            catch (Exception ex)
            {
                // Loguer l'erreur ou la relancer
                throw new ApplicationException("Erreur lors de la récupération des détails du rapport", ex);
            }
        }

    }
}
