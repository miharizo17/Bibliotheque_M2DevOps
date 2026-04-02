using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LibraryBackOffice.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Admin",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nom = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Prenom = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Username = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Password = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Active = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Admin", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "modePaiement",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    mode = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_modePaiement", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "typeAbonnement",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    type_abonnement = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_typeAbonnement", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "typeLivre",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    type_livre = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_typeLivre", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "utilisateur",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    nom = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    prenom = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    telephone = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    mail = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    mdp = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    etat = table.Column<int>(type: "int", nullable: false, defaultValue: 0),
                    dateentree = table.Column<DateTime>(type: "date", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_utilisateur", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "livre",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    id_typelivre = table.Column<int>(type: "int", nullable: false),
                    titre = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    sous_titre = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    saison = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    auteur = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    date_edition = table.Column<DateTime>(type: "date", nullable: true),
                    description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    image = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    document = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    etat = table.Column<int>(type: "int", nullable: false, defaultValue: 0)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_livre", x => x.id);
                    table.ForeignKey(
                        name: "FK_livre_typeLivre_id_typelivre",
                        column: x => x.id_typelivre,
                        principalTable: "typeLivre",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "historiqueabonnement",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    date_paiement = table.Column<DateTime>(type: "date", nullable: true),
                    id_typeabonnement = table.Column<int>(type: "int", nullable: false),
                    id_modepaiement = table.Column<int>(type: "int", nullable: false),
                    id_utilisateur = table.Column<int>(type: "int", nullable: false),
                    date_expiration = table.Column<DateTime>(type: "date", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_historiqueabonnement", x => x.id);
                    table.ForeignKey(
                        name: "FK_historiqueabonnement_modePaiement_id_modepaiement",
                        column: x => x.id_modepaiement,
                        principalTable: "modePaiement",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_historiqueabonnement_typeAbonnement_id_typeabonnement",
                        column: x => x.id_typeabonnement,
                        principalTable: "typeAbonnement",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_historiqueabonnement_utilisateur_id_utilisateur",
                        column: x => x.id_utilisateur,
                        principalTable: "utilisateur",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "historiquelecture",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    date_lecture = table.Column<DateTime>(type: "date", nullable: true),
                    id_livre = table.Column<int>(type: "int", nullable: false),
                    id_utilisateur = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_historiquelecture", x => x.id);
                    table.ForeignKey(
                        name: "FK_historiquelecture_livre_id_livre",
                        column: x => x.id_livre,
                        principalTable: "livre",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_historiquelecture_utilisateur_id_utilisateur",
                        column: x => x.id_utilisateur,
                        principalTable: "utilisateur",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "historiquepaiementlivre",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    date_lecture = table.Column<DateTime>(type: "date", nullable: true),
                    id_livre = table.Column<int>(type: "int", nullable: false),
                    id_utilisateur = table.Column<int>(type: "int", nullable: false),
                    prix = table.Column<double>(type: "float", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_historiquepaiementlivre", x => x.id);
                    table.ForeignKey(
                        name: "FK_historiquepaiementlivre_livre_id_livre",
                        column: x => x.id_livre,
                        principalTable: "livre",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_historiquepaiementlivre_utilisateur_id_utilisateur",
                        column: x => x.id_utilisateur,
                        principalTable: "utilisateur",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_historiqueabonnement_id_modepaiement",
                table: "historiqueabonnement",
                column: "id_modepaiement");

            migrationBuilder.CreateIndex(
                name: "IX_historiqueabonnement_id_typeabonnement",
                table: "historiqueabonnement",
                column: "id_typeabonnement");

            migrationBuilder.CreateIndex(
                name: "IX_historiqueabonnement_id_utilisateur",
                table: "historiqueabonnement",
                column: "id_utilisateur");

            migrationBuilder.CreateIndex(
                name: "IX_historiquelecture_id_livre",
                table: "historiquelecture",
                column: "id_livre");

            migrationBuilder.CreateIndex(
                name: "IX_historiquelecture_id_utilisateur",
                table: "historiquelecture",
                column: "id_utilisateur");

            migrationBuilder.CreateIndex(
                name: "IX_historiquepaiementlivre_id_livre",
                table: "historiquepaiementlivre",
                column: "id_livre");

            migrationBuilder.CreateIndex(
                name: "IX_historiquepaiementlivre_id_utilisateur",
                table: "historiquepaiementlivre",
                column: "id_utilisateur");

            migrationBuilder.CreateIndex(
                name: "IX_livre_id_typelivre",
                table: "livre",
                column: "id_typelivre");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Admin");

            migrationBuilder.DropTable(
                name: "historiqueabonnement");

            migrationBuilder.DropTable(
                name: "historiquelecture");

            migrationBuilder.DropTable(
                name: "historiquepaiementlivre");

            migrationBuilder.DropTable(
                name: "modePaiement");

            migrationBuilder.DropTable(
                name: "typeAbonnement");

            migrationBuilder.DropTable(
                name: "livre");

            migrationBuilder.DropTable(
                name: "utilisateur");

            migrationBuilder.DropTable(
                name: "typeLivre");
        }
    }
}
