using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using LibraryBackOffice.ViewModels;

public class AbonnementPdfDocument : IDocument
{
    private readonly List<AbonnementPdfDto> _data;
    private readonly int _mois;
    private readonly int _annee;

    public AbonnementPdfDocument(
        List<AbonnementPdfDto> data,
        int mois,
        int annee)
    {
        _data = data;
        _mois = mois;
        _annee = annee;
    }

    public DocumentMetadata GetMetadata() => DocumentMetadata.Default;

    public void Compose(IDocumentContainer container)
    {
        container.Page(page =>
        {
            page.Size(PageSizes.A4);
            page.Margin(20);
            page.DefaultTextStyle(x => x.FontSize(10));

            page.Header().Text(
                $"Historique des abonnements – {_mois:D2}/{_annee}")
                .FontSize(16)
                .Bold()
                .AlignCenter();

            page.Content().PaddingVertical(10).Table(table =>
            {
                table.ColumnsDefinition(columns =>
                {
                    columns.ConstantColumn(80);
                    columns.RelativeColumn();
                    columns.RelativeColumn();
                    columns.ConstantColumn(80);
                });

                table.Header(header =>
                {
                    header.Cell().Text("Date").Bold();
                    header.Cell().Text("Utilisateur").Bold();
                    header.Cell().Text("Abonnement").Bold();
                    header.Cell().Text("Expiration").Bold();
                });

                foreach (var item in _data)
                {
                    table.Cell().Text(item.DatePaiement.ToString("dd/MM/yyyy"));
                    table.Cell().Text(item.Utilisateur);
                    table.Cell().Text(item.TypeAbonnement);
                    table.Cell().Text(item.DateExpiration.ToString("dd/MM/yyyy"));
                }
            });

            page.Footer().AlignRight()
                .Text($"Généré le {DateTime.Now:dd/MM/yyyy HH:mm}");
        });
    }
}
