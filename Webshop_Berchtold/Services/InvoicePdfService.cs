using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using Webshop_Berchtold.Models;

namespace Webshop_Berchtold.Services
{
    public class InvoicePdfService
    {
        public byte[] GenerateInvoice(Order order, User user)
        {
            // QuestPDF License (Community - für nicht-kommerzielle Nutzung)
            QuestPDF.Settings.License = LicenseType.Community;

            var document = Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(PageSizes.A4);
                    page.Margin(50);
                    page.DefaultTextStyle(x => x.FontSize(11));

                    page.Header().Element(header => ComposeHeader(header, order));
                    page.Content().Element(content => ComposeContent(content, order, user));
                    page.Footer().Element(ComposeFooter);
                });
            });

            return document.GeneratePdf();
        }

        void ComposeHeader(IContainer container, Order order)
        {
            container.Row(row =>
            {
                row.RelativeItem().Column(column =>
                {
                    column.Item().Text("BERCHTOLD HOLZBAU-PLAN")
                        .FontSize(18)
                        .SemiBold()
                        .FontColor(Colors.Green.Darken2);

                    column.Item().Text("Markt 168")
                        .FontSize(9);
                    
                    column.Item().Text("3334 Gaflenz")
                        .FontSize(9);
                    
                    column.Item().Text("0677 61621048")
                        .FontSize(9);
                    
                    column.Item().Text("berchtold@holzbau-plan.at")
                        .FontSize(9);
                });

                row.RelativeItem().Column(column =>
                {
                    column.Item().AlignRight().Text("RECHNUNG")
                        .FontSize(18)
                        .SemiBold();
                    
                    column.Item().PaddingTop(8).AlignRight().Text(text =>
                    {
                        text.Span("Nr: ").SemiBold().FontSize(10);
                        text.Span($"#{1000 + order.Id}").FontSize(10);
                    });
                    
                    column.Item().AlignRight().Text(text =>
                    {
                        text.Span("Datum: ").SemiBold().FontSize(10);
                        text.Span(order.BestellDatum.ToString("dd.MM.yyyy")).FontSize(10);
                    });
                });
            });
        }

        void ComposeContent(IContainer container, Order order, User user)
        {
            container.PaddingVertical(20).Column(column =>
            {
                column.Spacing(10);

                // Kundeninformationen
                column.Item().Row(row =>
                {
                    row.RelativeItem().Column(col =>
                    {
                        col.Item().Text("Rechnungsadresse:").SemiBold().FontSize(10);
                        col.Item().PaddingTop(3).Text($"{user.FirstName} {user.LastName}").FontSize(9);
                        col.Item().Text(user.Email ?? "").FontSize(9);
                    });

                    row.RelativeItem().Column(col =>
                    {
                        col.Item().Text("Lieferadresse:").SemiBold().FontSize(10);
                        col.Item().PaddingTop(3).Text(order.LieferAdresse ?? "").FontSize(9);
                        if (!string.IsNullOrEmpty(order.Postleitzahl) && !string.IsNullOrEmpty(order.Stadt))
                        {
                            col.Item().Text($"{order.Postleitzahl} {order.Stadt}").FontSize(9);
                        }
                        if (!string.IsNullOrEmpty(order.Land))
                        {
                            col.Item().Text(order.Land).FontSize(9);
                        }
                    });
                });

                column.Item().PaddingTop(20).Element(ComposeTable);

                void ComposeTable(IContainer container)
                {
                    container.Table(table =>
                    {
                        table.ColumnsDefinition(columns =>
                        {
                            columns.ConstantColumn(30);  // Pos
                            columns.RelativeColumn(3);   // Artikel
                            columns.ConstantColumn(50);  // Menge
                            columns.ConstantColumn(60);  // Einzelpreis
                            columns.ConstantColumn(60);  // Gesamt
                        });

                        table.Header(header =>
                        {
                            header.Cell().Element(CellStyle).Text("Pos").SemiBold().FontSize(9);
                            header.Cell().Element(CellStyle).Text("Artikel").SemiBold().FontSize(9);
                            header.Cell().Element(CellStyle).AlignRight().Text("Menge").SemiBold().FontSize(9);
                            header.Cell().Element(CellStyle).AlignRight().Text("Preis").SemiBold().FontSize(9);
                            header.Cell().Element(CellStyle).AlignRight().Text("Gesamt").SemiBold().FontSize(9);

                            static IContainer CellStyle(IContainer container)
                            {
                                return container.DefaultTextStyle(x => x.SemiBold())
                                    .PaddingVertical(5)
                                    .BorderBottom(1)
                                    .BorderColor(Colors.Grey.Lighten2);
                            }
                        });

                        var position = 1;
                        foreach (var item in order.OrderItems)
                        {
                            table.Cell().Element(CellStyle).Text(position.ToString()).FontSize(9);
                            table.Cell().Element(CellStyle).Text(text =>
                            {
                                // Falls Produkt gelöscht wurde, zeige Platzhalter an
                                var productName = item.Product?.Name ?? "[Produkt gelöscht]";
                                var productUnit = item.Product?.Einheit ?? "";
                                
                                text.Span(productName).FontSize(9);
                                if (!string.IsNullOrEmpty(productUnit))
                                {
                                    text.Span($"\n{productUnit}").FontSize(7).FontColor(Colors.Grey.Medium);
                                }
                            });
                            table.Cell().Element(CellStyle).AlignRight().Text($"{item.Anzahl}x").FontSize(9);
                            table.Cell().Element(CellStyle).AlignRight().Text($"€{item.EinzelPreis:N2}").FontSize(9);
                            table.Cell().Element(CellStyle).AlignRight().Text($"€{item.GesamtPreis:N2}").FontSize(9);

                            position++;

                            static IContainer CellStyle(IContainer container)
                            {
                                return container.BorderBottom(1)
                                    .BorderColor(Colors.Grey.Lighten3)
                                    .PaddingVertical(6);
                            }
                        }
                    });
                }

                // Summen
                column.Item().PaddingTop(15).AlignRight().Column(summaryColumn =>
                {
                    var zwischensumme = order.OrderItems.Sum(oi => oi.GesamtPreis);
                    var netto = zwischensumme / 1.20m;
                    var mwst = zwischensumme - netto;

                    summaryColumn.Item().Row(row =>
                    {
                        row.ConstantItem(120).Text("Netto:").SemiBold().FontSize(10);
                        row.ConstantItem(80).AlignRight().Text($"€{netto:N2}").FontSize(10);
                    });

                    summaryColumn.Item().Row(row =>
                    {
                        row.ConstantItem(120).Text("MwSt. (20%):").SemiBold().FontSize(10);
                        row.ConstantItem(80).AlignRight().Text($"€{mwst:N2}").FontSize(10);
                    });

                    summaryColumn.Item().PaddingTop(5).BorderTop(2).BorderColor(Colors.Grey.Medium)
                        .Row(row =>
                        {
                            row.ConstantItem(120).Text("Gesamtbetrag:").SemiBold().FontSize(11);
                            row.ConstantItem(80).AlignRight().Text($"€{order.GesamtBetrag:N2}").SemiBold().FontSize(11);
                        });
                });

                // Zahlungsinformationen
                column.Item().PaddingTop(25).Text("Zahlungsinformationen").SemiBold().FontSize(11);
                column.Item().PaddingTop(5).Text("Status: " + order.Status).FontSize(10);
                column.Item().Text("Vielen Dank für Ihre Bestellung!").FontSize(10);
            });
        }

        void ComposeFooter(IContainer container)
        {
            container.AlignCenter().Text(text =>
            {
                text.Span("Seite ").FontSize(9);
                text.CurrentPageNumber().FontSize(9);
                text.Span(" von ").FontSize(9);
                text.TotalPages().FontSize(9);
            });
        }
    }
}
