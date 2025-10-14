using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Hotel.Migrations
{
    public partial class Bingahouse : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Agents",
                columns: table => new
                {
                    AgentId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    NomAgent = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    PrenomAgent = table.Column<string>(type: "nvarchar(15)", maxLength: 15, nullable: false),
                    SexeAgent = table.Column<string>(type: "nvarchar(1)", maxLength: 1, nullable: false),
                    Lieu_nais_Agent = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Date_nais_Agent = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Nationalite_Agent = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Fonction_Agent = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PhotoPath = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Agents", x => x.AgentId);
                });

            migrationBuilder.CreateTable(
                name: "Aliments",
                columns: table => new
                {
                    AlimentId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TitreAliment = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    UniteAliment = table.Column<int>(type: "int", nullable: false),
                    PrixUnitAliment = table.Column<float>(type: "real", nullable: false),
                    QuantiteStock = table.Column<int>(type: "int", nullable: false),
                    PhotoPath = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Aliments", x => x.AlimentId);
                });

            migrationBuilder.CreateTable(
                name: "Brasseries",
                columns: table => new
                {
                    BrasserieId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TitreBrasserie = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Brasseries", x => x.BrasserieId);
                });

            migrationBuilder.CreateTable(
                name: "CategorieBoissons",
                columns: table => new
                {
                    CategorieBoissonId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TitreCategorieBoisson = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CategorieBoissons", x => x.CategorieBoissonId);
                });

            migrationBuilder.CreateTable(
                name: "Chambres",
                columns: table => new
                {
                    ChambreId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    NumeroChambre = table.Column<int>(type: "int", nullable: false),
                    TypeChambre = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PrixNuit = table.Column<double>(type: "float", nullable: false),
                    PrixHeure = table.Column<double>(type: "float", nullable: false),
                    StatutChambre = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CapaciteMaxChambre = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Chambres", x => x.ChambreId);
                });

            migrationBuilder.CreateTable(
                name: "Clients",
                columns: table => new
                {
                    ClientId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    NomClient = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    SexeClient = table.Column<string>(type: "nvarchar(1)", maxLength: 1, nullable: false),
                    TypeClient = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Clients", x => x.ClientId);
                });

            migrationBuilder.CreateTable(
                name: "Configurations",
                columns: table => new
                {
                    ConfigurationId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserDashboardUrl = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Configurations", x => x.ConfigurationId);
                });

            migrationBuilder.CreateTable(
                name: "DetailsCommandes",
                columns: table => new
                {
                    DetailsCommandeId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CommandeId = table.Column<int>(type: "int", nullable: false),
                    ProduitNom = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Quantite = table.Column<int>(type: "int", nullable: false),
                    PrixUnitaire = table.Column<float>(type: "real", nullable: false),
                    TypeCommande = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    NomsClient = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    NomsAgent = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TitreTable = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DateCommande = table.Column<DateTime>(type: "datetime2", nullable: false),
                    HeureCommande = table.Column<DateTime>(type: "datetime2", nullable: false),
                    TypeProduit = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DetailsCommandes", x => x.DetailsCommandeId);
                });

            migrationBuilder.CreateTable(
                name: "DetailsFactureCautions",
                columns: table => new
                {
                    DetailsFactureCautionId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Numres = table.Column<int>(type: "int", nullable: false),
                    Numfact = table.Column<int>(type: "int", nullable: false),
                    Datefact = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Heurefact = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Debut = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Fin = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Caissier = table.Column<int>(type: "int", nullable: false),
                    Caisse = table.Column<int>(type: "int", nullable: false),
                    Chambre = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Duree = table.Column<int>(type: "int", nullable: false),
                    Montant = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    ModeReglement = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TypeReglement = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Devise = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DetailsFactureCautions", x => x.DetailsFactureCautionId);
                });

            migrationBuilder.CreateTable(
                name: "DetailsFactureReservations",
                columns: table => new
                {
                    DetailsFactureReservationId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Numres = table.Column<int>(type: "int", nullable: false),
                    Numfact = table.Column<int>(type: "int", nullable: false),
                    Datefact = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Heurefact = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Debut = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Fin = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Caissier = table.Column<int>(type: "int", nullable: false),
                    Caisse = table.Column<int>(type: "int", nullable: false),
                    Chambre = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Duree = table.Column<int>(type: "int", nullable: false),
                    Montant = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    ModeReglement = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Service = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TypeReglement = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Devise = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DetailsFactureReservations", x => x.DetailsFactureReservationId);
                });

            migrationBuilder.CreateTable(
                name: "DetailsFactures",
                columns: table => new
                {
                    DetailsFactureId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Numcmd = table.Column<int>(type: "int", nullable: false),
                    Numfact = table.Column<int>(type: "int", nullable: false),
                    Datefact = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Heurefact = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Caissier = table.Column<int>(type: "int", nullable: false),
                    Caisse = table.Column<int>(type: "int", nullable: false),
                    Designation = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Qte = table.Column<int>(type: "int", nullable: false),
                    PU = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    ModeReglement = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TypeReglement = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Devise = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DetailsFactures", x => x.DetailsFactureId);
                });

            migrationBuilder.CreateTable(
                name: "Devises",
                columns: table => new
                {
                    DeviseId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TitreDevise = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SigleDevise = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Devises", x => x.DeviseId);
                });

            migrationBuilder.CreateTable(
                name: "Fournisseurs",
                columns: table => new
                {
                    FournisseurId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    NomFournisseur = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    PhoneFournisseur = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    AdresseFournisseur = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DomaineActivite = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Fournisseurs", x => x.FournisseurId);
                });

            migrationBuilder.CreateTable(
                name: "ModeReglements",
                columns: table => new
                {
                    ModeReglementId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TitreModeReglement = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ModeReglements", x => x.ModeReglementId);
                });

            migrationBuilder.CreateTable(
                name: "PosteTravails",
                columns: table => new
                {
                    PosteTravailId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TitrePosteTravail = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PosteTravails", x => x.PosteTravailId);
                });

            migrationBuilder.CreateTable(
                name: "Services",
                columns: table => new
                {
                    ServiceId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    NomService = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DescriptionService = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PrixService = table.Column<float>(type: "real", nullable: false),
                    PhotoPath = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Services", x => x.ServiceId);
                });

            migrationBuilder.CreateTable(
                name: "TableClients",
                columns: table => new
                {
                    TableClientId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TitreTableClient = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TableClients", x => x.TableClientId);
                });

            migrationBuilder.CreateTable(
                name: "TypeMouvements",
                columns: table => new
                {
                    TypeMouvementId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TitreTypeMouvement = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TypeMouvements", x => x.TypeMouvementId);
                });

            migrationBuilder.CreateTable(
                name: "TypeReglements",
                columns: table => new
                {
                    TypeReglementId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TitreTypeReglement = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TypeReglements", x => x.TypeReglementId);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Username = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Password = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Role = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    AgentId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Users_Agents_AgentId",
                        column: x => x.AgentId,
                        principalTable: "Agents",
                        principalColumn: "AgentId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "hPrixUnitAliments",
                columns: table => new
                {
                    DateHisto = table.Column<DateTime>(type: "datetime2", nullable: false),
                    AlimentId = table.Column<int>(type: "int", nullable: false),
                    PrixUnitAliment = table.Column<float>(type: "real", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_hPrixUnitAliments", x => new { x.AlimentId, x.DateHisto });
                    table.ForeignKey(
                        name: "FK_hPrixUnitAliments_Aliments_AlimentId",
                        column: x => x.AlimentId,
                        principalTable: "Aliments",
                        principalColumn: "AlimentId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Boissons",
                columns: table => new
                {
                    BoissonId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    BrasserieId = table.Column<int>(type: "int", nullable: false),
                    CategorieBoissonId = table.Column<int>(type: "int", nullable: false),
                    TitreBoisson = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    UniteBoisson = table.Column<int>(type: "int", nullable: false),
                    PrixUnitSB = table.Column<float>(type: "real", nullable: false),
                    PrixUnitLB = table.Column<float>(type: "real", nullable: false),
                    QuantiteStock = table.Column<int>(type: "int", nullable: false),
                    PhotoPath = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Boissons", x => x.BoissonId);
                    table.ForeignKey(
                        name: "FK_Boissons_Brasseries_BrasserieId",
                        column: x => x.BrasserieId,
                        principalTable: "Brasseries",
                        principalColumn: "BrasserieId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Boissons_CategorieBoissons_CategorieBoissonId",
                        column: x => x.CategorieBoissonId,
                        principalTable: "CategorieBoissons",
                        principalColumn: "CategorieBoissonId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Commandes",
                columns: table => new
                {
                    CommandeId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AgentId = table.Column<int>(type: "int", nullable: false),
                    ClientId = table.Column<int>(type: "int", nullable: false),
                    DateCommande = table.Column<DateTime>(type: "datetime2", nullable: false),
                    HeureCommande = table.Column<DateTime>(type: "datetime2", nullable: false),
                    TypeCommande = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Commandes", x => x.CommandeId);
                    table.ForeignKey(
                        name: "FK_Commandes_Agents_AgentId",
                        column: x => x.AgentId,
                        principalTable: "Agents",
                        principalColumn: "AgentId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Commandes_Clients_ClientId",
                        column: x => x.ClientId,
                        principalTable: "Clients",
                        principalColumn: "ClientId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Normals",
                columns: table => new
                {
                    ClientId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Normals", x => x.ClientId);
                    table.ForeignKey(
                        name: "FK_Normals_Clients_ClientId",
                        column: x => x.ClientId,
                        principalTable: "Clients",
                        principalColumn: "ClientId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Residents",
                columns: table => new
                {
                    ClientId = table.Column<int>(type: "int", nullable: false),
                    PhoneClient = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    NumpieceId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TypedocId = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Residents", x => x.ClientId);
                    table.ForeignKey(
                        name: "FK_Residents_Clients_ClientId",
                        column: x => x.ClientId,
                        principalTable: "Clients",
                        principalColumn: "ClientId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Tauxes",
                columns: table => new
                {
                    TauxId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DeviseId = table.Column<int>(type: "int", nullable: false),
                    DateTaux = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ValeurTaux = table.Column<float>(type: "real", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tauxes", x => x.TauxId);
                    table.ForeignKey(
                        name: "FK_Tauxes_Devises_DeviseId",
                        column: x => x.DeviseId,
                        principalTable: "Devises",
                        principalColumn: "DeviseId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Fournirs",
                columns: table => new
                {
                    FournisseurId = table.Column<int>(type: "int", nullable: false),
                    AlimentId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Fournirs", x => new { x.AlimentId, x.FournisseurId });
                    table.ForeignKey(
                        name: "FK_Fournirs_Aliments_AlimentId",
                        column: x => x.AlimentId,
                        principalTable: "Aliments",
                        principalColumn: "AlimentId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Fournirs_Fournisseurs_FournisseurId",
                        column: x => x.FournisseurId,
                        principalTable: "Fournisseurs",
                        principalColumn: "FournisseurId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Caisses",
                columns: table => new
                {
                    PosteTravailId = table.Column<int>(type: "int", nullable: false),
                    TitreCaisse = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    BarCaisse = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Caisses", x => x.PosteTravailId);
                    table.ForeignKey(
                        name: "FK_Caisses_PosteTravails_PosteTravailId",
                        column: x => x.PosteTravailId,
                        principalTable: "PosteTravails",
                        principalColumn: "PosteTravailId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ShiftTravails",
                columns: table => new
                {
                    ShiftTravailId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AgentId = table.Column<int>(type: "int", nullable: false),
                    PosteTravailId = table.Column<int>(type: "int", nullable: false),
                    DateDebutShift = table.Column<DateTime>(type: "datetime2", nullable: false),
                    HeureDebutShift = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ObservationDebutShift = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ShiftTravails", x => x.ShiftTravailId);
                    table.ForeignKey(
                        name: "FK_ShiftTravails_Agents_AgentId",
                        column: x => x.AgentId,
                        principalTable: "Agents",
                        principalColumn: "AgentId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ShiftTravails_PosteTravails_PosteTravailId",
                        column: x => x.PosteTravailId,
                        principalTable: "PosteTravails",
                        principalColumn: "PosteTravailId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "HPrixUnitLBBoissons",
                columns: table => new
                {
                    DateHisto = table.Column<DateTime>(type: "datetime2", nullable: false),
                    BoissonId = table.Column<int>(type: "int", nullable: false),
                    PrixUnitLB = table.Column<float>(type: "real", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HPrixUnitLBBoissons", x => new { x.BoissonId, x.DateHisto });
                    table.ForeignKey(
                        name: "FK_HPrixUnitLBBoissons_Boissons_BoissonId",
                        column: x => x.BoissonId,
                        principalTable: "Boissons",
                        principalColumn: "BoissonId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "HPrixUnitSBBoissons",
                columns: table => new
                {
                    DateHisto = table.Column<DateTime>(type: "datetime2", nullable: false),
                    BoissonId = table.Column<int>(type: "int", nullable: false),
                    PrixUnitSB = table.Column<float>(type: "real", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HPrixUnitSBBoissons", x => new { x.BoissonId, x.DateHisto });
                    table.ForeignKey(
                        name: "FK_HPrixUnitSBBoissons_Boissons_BoissonId",
                        column: x => x.BoissonId,
                        principalTable: "Boissons",
                        principalColumn: "BoissonId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Procurers",
                columns: table => new
                {
                    FournisseurId = table.Column<int>(type: "int", nullable: false),
                    BoissonId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Procurers", x => new { x.BoissonId, x.FournisseurId });
                    table.ForeignKey(
                        name: "FK_Procurers_Boissons_BoissonId",
                        column: x => x.BoissonId,
                        principalTable: "Boissons",
                        principalColumn: "BoissonId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Procurers_Fournisseurs_FournisseurId",
                        column: x => x.FournisseurId,
                        principalTable: "Fournisseurs",
                        principalColumn: "FournisseurId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CommandesServices",
                columns: table => new
                {
                    CommandeId = table.Column<int>(type: "int", nullable: false),
                    ServiceId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CommandesServices", x => x.CommandeId);
                    table.ForeignKey(
                        name: "FK_CommandesServices_Commandes_CommandeId",
                        column: x => x.CommandeId,
                        principalTable: "Commandes",
                        principalColumn: "CommandeId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "CommandeTableClients",
                columns: table => new
                {
                    CommandeId = table.Column<int>(type: "int", nullable: false),
                    TableClientId = table.Column<int>(type: "int", nullable: false),
                    DateOccupation = table.Column<DateTime>(type: "datetime2", nullable: false),
                    HeureOccupation = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CommandeTableClients", x => x.CommandeId);
                    table.ForeignKey(
                        name: "FK_CommandeTableClients_Commandes_CommandeId",
                        column: x => x.CommandeId,
                        principalTable: "Commandes",
                        principalColumn: "CommandeId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Concerners",
                columns: table => new
                {
                    AlimentId = table.Column<int>(type: "int", nullable: false),
                    CommandeId = table.Column<int>(type: "int", nullable: false),
                    QuantiteCom = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Concerners", x => new { x.AlimentId, x.CommandeId });
                    table.ForeignKey(
                        name: "FK_Concerners_Aliments_AlimentId",
                        column: x => x.AlimentId,
                        principalTable: "Aliments",
                        principalColumn: "AlimentId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Concerners_Commandes_CommandeId",
                        column: x => x.CommandeId,
                        principalTable: "Commandes",
                        principalColumn: "CommandeId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Contenirs",
                columns: table => new
                {
                    BoissonId = table.Column<int>(type: "int", nullable: false),
                    CommandeId = table.Column<int>(type: "int", nullable: false),
                    QuantiteCom = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Contenirs", x => new { x.BoissonId, x.CommandeId });
                    table.ForeignKey(
                        name: "FK_Contenirs_Boissons_BoissonId",
                        column: x => x.BoissonId,
                        principalTable: "Boissons",
                        principalColumn: "BoissonId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Contenirs_Commandes_CommandeId",
                        column: x => x.CommandeId,
                        principalTable: "Commandes",
                        principalColumn: "CommandeId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Reservations",
                columns: table => new
                {
                    ReservationId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AgentId = table.Column<int>(type: "int", nullable: false),
                    ChambreId = table.Column<int>(type: "int", nullable: false),
                    ResidentId = table.Column<int>(type: "int", nullable: false),
                    DateReservation = table.Column<DateTime>(type: "datetime2", nullable: false),
                    HeureReservation = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DateDebutOccupation = table.Column<DateTime>(type: "datetime2", nullable: false),
                    HeureDebutOccupation = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DateFinOccupation = table.Column<DateTime>(type: "datetime2", nullable: false),
                    HeureFinOccupation = table.Column<DateTime>(type: "datetime2", nullable: false),
                    StatutReservation = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CautionReservation = table.Column<double>(type: "float", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Reservations", x => x.ReservationId);
                    table.ForeignKey(
                        name: "FK_Reservations_Agents_AgentId",
                        column: x => x.AgentId,
                        principalTable: "Agents",
                        principalColumn: "AgentId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Reservations_Chambres_ChambreId",
                        column: x => x.ChambreId,
                        principalTable: "Chambres",
                        principalColumn: "ChambreId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Reservations_Residents_ResidentId",
                        column: x => x.ResidentId,
                        principalTable: "Residents",
                        principalColumn: "ClientId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Reglements",
                columns: table => new
                {
                    ReglementId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ModeReglementId = table.Column<int>(type: "int", nullable: false),
                    DeviseId = table.Column<int>(type: "int", nullable: false),
                    TypeReglementId = table.Column<int>(type: "int", nullable: false),
                    AgentId = table.Column<int>(type: "int", nullable: false),
                    CaisseId = table.Column<int>(type: "int", nullable: false),
                    DateReglement = table.Column<DateTime>(type: "datetime2", nullable: false),
                    HeureReglement = table.Column<DateTime>(type: "datetime2", nullable: false),
                    MontantReglement = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    MotifReglement = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Reglements", x => x.ReglementId);
                    table.ForeignKey(
                        name: "FK_Reglements_Agents_AgentId",
                        column: x => x.AgentId,
                        principalTable: "Agents",
                        principalColumn: "AgentId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Reglements_Caisses_CaisseId",
                        column: x => x.CaisseId,
                        principalTable: "Caisses",
                        principalColumn: "PosteTravailId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Reglements_Devises_DeviseId",
                        column: x => x.DeviseId,
                        principalTable: "Devises",
                        principalColumn: "DeviseId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Reglements_ModeReglements_ModeReglementId",
                        column: x => x.ModeReglementId,
                        principalTable: "ModeReglements",
                        principalColumn: "ModeReglementId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Reglements_TypeReglements_TypeReglementId",
                        column: x => x.TypeReglementId,
                        principalTable: "TypeReglements",
                        principalColumn: "TypeReglementId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "FinShiftTravails",
                columns: table => new
                {
                    ShiftTravailId = table.Column<int>(type: "int", nullable: false),
                    DateFinShift = table.Column<DateTime>(type: "datetime2", nullable: true),
                    HeureFinShift = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ObservationFinShift = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FinShiftTravails", x => x.ShiftTravailId);
                    table.ForeignKey(
                        name: "FK_FinShiftTravails_ShiftTravails_ShiftTravailId",
                        column: x => x.ShiftTravailId,
                        principalTable: "ShiftTravails",
                        principalColumn: "ShiftTravailId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Inclures",
                columns: table => new
                {
                    ReservationId = table.Column<int>(type: "int", nullable: false),
                    ServiceId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Inclures", x => new { x.ServiceId, x.ReservationId });
                    table.ForeignKey(
                        name: "FK_Inclures_Reservations_ReservationId",
                        column: x => x.ReservationId,
                        principalTable: "Reservations",
                        principalColumn: "ReservationId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Inclures_Services_ServiceId",
                        column: x => x.ServiceId,
                        principalTable: "Services",
                        principalColumn: "ServiceId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ReglementCommandes",
                columns: table => new
                {
                    ReglementId = table.Column<int>(type: "int", nullable: false),
                    CommandeId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ReglementCommandes", x => x.ReglementId);
                    table.ForeignKey(
                        name: "FK_ReglementCommandes_Reglements_ReglementId",
                        column: x => x.ReglementId,
                        principalTable: "Reglements",
                        principalColumn: "ReglementId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ReglementReservations",
                columns: table => new
                {
                    ReglementId = table.Column<int>(type: "int", nullable: false),
                    ReservationId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ReglementReservations", x => x.ReglementId);
                    table.ForeignKey(
                        name: "FK_ReglementReservations_Reglements_ReglementId",
                        column: x => x.ReglementId,
                        principalTable: "Reglements",
                        principalColumn: "ReglementId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.InsertData(
                table: "Agents",
                columns: new[] { "AgentId", "Date_nais_Agent", "Fonction_Agent", "Lieu_nais_Agent", "Nationalite_Agent", "NomAgent", "PhotoPath", "PrenomAgent", "SexeAgent" },
                values: new object[] { 1, new DateTime(2001, 11, 23, 0, 0, 0, 0, DateTimeKind.Unspecified), "ADMIN", "KINSHASA", "CONGOLAISE", "Bakatuasa", "Aucunephotopour l'agent", "Jeremie", "M" });

            migrationBuilder.InsertData(
                table: "Agents",
                columns: new[] { "AgentId", "Date_nais_Agent", "Fonction_Agent", "Lieu_nais_Agent", "Nationalite_Agent", "NomAgent", "PhotoPath", "PrenomAgent", "SexeAgent" },
                values: new object[] { 2, new DateTime(2001, 11, 23, 0, 0, 0, 0, DateTimeKind.Unspecified), "CAISSIER", "KINSHASA", "CONGOLAISE", "Bankoledi", "Aucunephotopour l'agent", "Jean", "M" });

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "Id", "AgentId", "Password", "Role", "Username" },
                values: new object[] { 1, 1, "$2a$12$ymt1HjInRK183f8BGGhWoOpEvUsBdaedcM.xzE0si/gjJpbyzRldu", "ADMIN", "admin" });

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "Id", "AgentId", "Password", "Role", "Username" },
                values: new object[] { 2, 2, "$2a$12$QgVJ460uUfYDkTa/LsgsoOf0GvBnv8EO8iAUWK7yep3OHJz8UBX72", "CAISSIER", "user" });

            migrationBuilder.CreateIndex(
                name: "IX_Boissons_BrasserieId",
                table: "Boissons",
                column: "BrasserieId");

            migrationBuilder.CreateIndex(
                name: "IX_Boissons_CategorieBoissonId",
                table: "Boissons",
                column: "CategorieBoissonId");

            migrationBuilder.CreateIndex(
                name: "IX_Commandes_AgentId",
                table: "Commandes",
                column: "AgentId");

            migrationBuilder.CreateIndex(
                name: "IX_Commandes_ClientId",
                table: "Commandes",
                column: "ClientId");

            migrationBuilder.CreateIndex(
                name: "IX_Concerners_CommandeId",
                table: "Concerners",
                column: "CommandeId");

            migrationBuilder.CreateIndex(
                name: "IX_Contenirs_CommandeId",
                table: "Contenirs",
                column: "CommandeId");

            migrationBuilder.CreateIndex(
                name: "IX_Fournirs_FournisseurId",
                table: "Fournirs",
                column: "FournisseurId");

            migrationBuilder.CreateIndex(
                name: "IX_Inclures_ReservationId",
                table: "Inclures",
                column: "ReservationId");

            migrationBuilder.CreateIndex(
                name: "IX_Procurers_FournisseurId",
                table: "Procurers",
                column: "FournisseurId");

            migrationBuilder.CreateIndex(
                name: "IX_Reglements_AgentId",
                table: "Reglements",
                column: "AgentId");

            migrationBuilder.CreateIndex(
                name: "IX_Reglements_CaisseId",
                table: "Reglements",
                column: "CaisseId");

            migrationBuilder.CreateIndex(
                name: "IX_Reglements_DeviseId",
                table: "Reglements",
                column: "DeviseId");

            migrationBuilder.CreateIndex(
                name: "IX_Reglements_ModeReglementId",
                table: "Reglements",
                column: "ModeReglementId");

            migrationBuilder.CreateIndex(
                name: "IX_Reglements_TypeReglementId",
                table: "Reglements",
                column: "TypeReglementId");

            migrationBuilder.CreateIndex(
                name: "IX_Reservations_AgentId",
                table: "Reservations",
                column: "AgentId");

            migrationBuilder.CreateIndex(
                name: "IX_Reservations_ChambreId",
                table: "Reservations",
                column: "ChambreId");

            migrationBuilder.CreateIndex(
                name: "IX_Reservations_ResidentId",
                table: "Reservations",
                column: "ResidentId");

            migrationBuilder.CreateIndex(
                name: "IX_ShiftTravails_AgentId",
                table: "ShiftTravails",
                column: "AgentId");

            migrationBuilder.CreateIndex(
                name: "IX_ShiftTravails_PosteTravailId",
                table: "ShiftTravails",
                column: "PosteTravailId");

            migrationBuilder.CreateIndex(
                name: "IX_Tauxes_DeviseId",
                table: "Tauxes",
                column: "DeviseId");

            migrationBuilder.CreateIndex(
                name: "IX_Users_AgentId",
                table: "Users",
                column: "AgentId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CommandesServices");

            migrationBuilder.DropTable(
                name: "CommandeTableClients");

            migrationBuilder.DropTable(
                name: "Concerners");

            migrationBuilder.DropTable(
                name: "Configurations");

            migrationBuilder.DropTable(
                name: "Contenirs");

            migrationBuilder.DropTable(
                name: "DetailsCommandes");

            migrationBuilder.DropTable(
                name: "DetailsFactureCautions");

            migrationBuilder.DropTable(
                name: "DetailsFactureReservations");

            migrationBuilder.DropTable(
                name: "DetailsFactures");

            migrationBuilder.DropTable(
                name: "FinShiftTravails");

            migrationBuilder.DropTable(
                name: "Fournirs");

            migrationBuilder.DropTable(
                name: "hPrixUnitAliments");

            migrationBuilder.DropTable(
                name: "HPrixUnitLBBoissons");

            migrationBuilder.DropTable(
                name: "HPrixUnitSBBoissons");

            migrationBuilder.DropTable(
                name: "Inclures");

            migrationBuilder.DropTable(
                name: "Normals");

            migrationBuilder.DropTable(
                name: "Procurers");

            migrationBuilder.DropTable(
                name: "ReglementCommandes");

            migrationBuilder.DropTable(
                name: "ReglementReservations");

            migrationBuilder.DropTable(
                name: "TableClients");

            migrationBuilder.DropTable(
                name: "Tauxes");

            migrationBuilder.DropTable(
                name: "TypeMouvements");

            migrationBuilder.DropTable(
                name: "Users");

            migrationBuilder.DropTable(
                name: "Commandes");

            migrationBuilder.DropTable(
                name: "ShiftTravails");

            migrationBuilder.DropTable(
                name: "Aliments");

            migrationBuilder.DropTable(
                name: "Reservations");

            migrationBuilder.DropTable(
                name: "Services");

            migrationBuilder.DropTable(
                name: "Boissons");

            migrationBuilder.DropTable(
                name: "Fournisseurs");

            migrationBuilder.DropTable(
                name: "Reglements");

            migrationBuilder.DropTable(
                name: "Chambres");

            migrationBuilder.DropTable(
                name: "Residents");

            migrationBuilder.DropTable(
                name: "Brasseries");

            migrationBuilder.DropTable(
                name: "CategorieBoissons");

            migrationBuilder.DropTable(
                name: "Agents");

            migrationBuilder.DropTable(
                name: "Caisses");

            migrationBuilder.DropTable(
                name: "Devises");

            migrationBuilder.DropTable(
                name: "ModeReglements");

            migrationBuilder.DropTable(
                name: "TypeReglements");

            migrationBuilder.DropTable(
                name: "Clients");

            migrationBuilder.DropTable(
                name: "PosteTravails");
        }
    }
}
