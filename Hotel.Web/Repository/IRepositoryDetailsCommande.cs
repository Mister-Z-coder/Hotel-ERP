using Hotel.Data;
//using Hotel.DataSource;
using Hotel.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;



namespace Hotel.Repository
{
    public interface IRepositoryDetailsCommande
    {
        Task<IEnumerable<DetailsCommande>> GetDetailsCmd(int CmdId, string TypeProd);
        Task<IEnumerable<DetailsFacture>> GetDetailsFacture(int FactId);
        Task<IEnumerable<DetailsFactureCaution>> GetDetailsCaution(int CautionId);
        Task<IEnumerable<DetailsFactureReservation>> GetDetailsReservation(int CautionId);
        Task<IEnumerable<DetailsReglement>> GetDetailsReglement();
        Task<IEnumerable<DetailsDemande>> GetDetailsDemande(string agent, string type, string etat, string date);
        Task<IEnumerable<DetailsDemande>> GetDetailsDemandeOne(int demandeId);
        Task<IEnumerable<DetailsLivraison>> GetDetailsLivraison(string agent, string etat, string date);
        Task<IEnumerable<DetailsLivraison>> GetDetailsLivraisonOne(int demandeId);
        Task<IEnumerable<DetailsSortie>> GetDetailsSortie(string agent, string etat, string date);
        Task<IEnumerable<DetailsSortie>> GetDetailsSortieOne(int demandeId);
        Task<IEnumerable<DetailsDeclassement>> GetDetailsDeclassement(string agent, string motif, string date);
        Task<IEnumerable<DetailsDeclassement>> GetDetailsDeclassementOne(int declassementId);
        Task<IEnumerable<DetailsMouvement>> GetDetailsMouvement();

        Task<IEnumerable<DetailsReglement>> GetDetailsVentes();
        //FactureDataSet GetFactureDataSet(int FactId);
    }
}
