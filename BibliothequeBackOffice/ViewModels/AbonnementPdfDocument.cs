using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using LibraryBackOffice.ViewModels;

namespace LibraryBackOffice.Documents
{
    public class AbonnementPdfDocument : IDocument
    {
        private readonly List<AbonnementPdfDto> _data;
        private readonly int _mois;
        private readonly int _annee;

        public AbonnementPdfDocument(List<AbonnementPdfDto> data, int mois, int annee)
        {
            _data = data;
            _mois = mois;
            _annee = annee;
        }

        public void Compose(IDocumentContainer container)
        {
            container.Page(page =>
            {
                page.Size(PageSizes.A4);
                page.Margin(30);
                page.DefaultTextStyle(x => x.FontSize(10).FontFamily("Helvetica"));

                page.Header().Element(ComposeHeader);
                page.Content().Element(ComposeContent);
                page.Footer().Element(ComposeFooter);
            });
        }

        private void ComposeHeader(IContainer container)
        {
            var accentStart = Color.FromHex("#667eea");
            var accentEnd = Color.FromHex("#764ba2");

            container.Column(column =>
            {
                column.Item().PaddingBottom(20).Row(row =>
                {
                    row.RelativeItem().Column(col =>
                    {
                        col.Item().Text("Library BackOffice").FontSize(22).Bold().FontColor(accentStart);
                        col.Item().Text("Historique des Abonnements").FontSize(18).Bold().FontColor(Colors.Black);
                    });

                    row.ConstantItem(200).AlignRight().Text(text =>
                    {
                        text.Span("Rapport du ").FontSize(10).FontColor(Colors.Grey.Medium);
                        text.Span(DateTime.Now.ToString("dd MMMM yyyy", new System.Globalization.CultureInfo("fr-FR"))).Bold();
                    });
                });

                column.Item().PaddingBottom(15).Text($"{GetMoisNom(_mois)} {_annee}")
                    .FontSize(14).Bold().FontColor(accentEnd);
            });
        }

        private void ComposeContent(IContainer container)
        {
            container.Table(table =>
            {
                table.ColumnsDefinition(columns =>
                {
                    columns.ConstantColumn(80);
                    columns.ConstantColumn(80);
                    columns.RelativeColumn(2);
                    columns.RelativeColumn(2);
                    columns.RelativeColumn();
                });

                table.Header(header =>
                {
                    header.Cell().Element(CellStyle).Text("Paiement").Bold().FontColor(Colors.White);
                    header.Cell().Element(CellStyle).Text("Expiration").Bold().FontColor(Colors.White);
                    header.Cell().Element(CellStyle).Text("Abonnement").Bold().FontColor(Colors.White);
                    header.Cell().Element(CellStyle).Text("Client").Bold().FontColor(Colors.White);
                    header.Cell().Element(CellStyle).Text("Statut").Bold().FontColor(Colors.White);

                    static IContainer CellStyle(IContainer container) => container
                        .Padding(8)
                        .Background(Color.FromHex("#667eea"))
                        .AlignCenter()
                        .AlignMiddle();
                });

                foreach (var item in _data)
                {
                    bool estExpire = item.DateExpiration < DateTime.Today;

                    table.Cell().Element(Cell).Text(item.DatePaiement.ToString("dd/MM/yyyy"));
                    table.Cell().Element(Cell).Text(item.DateExpiration.ToString("dd/MM/yyyy"));
                    table.Cell().Element(Cell).Text(item.TypeAbonnement ?? "-");
                    table.Cell().Element(Cell).Text(item.Utilisateur ?? "-");
                    table.Cell().Element(Cell).Text(estExpire ? "Expiré" : "Actif")
                        .FontColor(estExpire ? Colors.Red.Medium : Colors.Green.Medium).Bold();

                    static IContainer Cell(IContainer container) => container
                        .Padding(8)
                        .BorderBottom(1)
                        .BorderColor(Colors.Grey.Lighten3)
                        .AlignMiddle();
                }
            });
        }

        private void ComposeFooter(IContainer container)
        {
            container.AlignCenter().PaddingTop(20).Text(text =>
            {
                text.Span($"Page ").FontSize(9);
                text.CurrentPageNumber().FontSize(9);
                text.Span(" / ").FontSize(9);
                text.TotalPages().FontSize(9);
                text.Span($" • {_data.Count} abonnement(s)").FontSize(9);
            });
        }

        private string GetMoisNom(int mois)
        {
            var noms = new[] { "Janvier", "Février", "Mars", "Avril", "Mai", "Juin", "Juillet", "Août", "Septembre", "Octobre", "Novembre", "Décembre" };
            return noms[mois - 1];
        }
    }
}