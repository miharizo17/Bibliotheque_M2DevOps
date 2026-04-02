using CsvHelper.Configuration;
using LibraryBackOffice.Models;
using System.Globalization;

public class LivreImportMap : ClassMap<LivreImportDto>
{
    public LivreImportMap()
    {
        Map(m => m.Titre).Name("Titre");
        Map(m => m.Sous_Titre).Name("SousTitre");
        Map(m => m.Saison).Name("Saison");
        Map(m => m.Auteur).Name("Auteur");
        Map(m => m.Date_Edition).Name("DateEdition")
            .TypeConverterOption.Format("yyyy-MM-dd");
        Map(m => m.Description).Name("Description");
        Map(m => m.Image).Name("Image");
        Map(m => m.Document).Name("Document");
        Map(m => m.TypeLivreNom).Name("TypeLivre");   // Nom du type dans le CSV
    }
}