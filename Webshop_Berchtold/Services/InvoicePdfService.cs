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
                        .FontSize(20)
                        .SemiBold()
                        .FontColor(Colors.Green.Darken2);

                    column.Item().Text("Markt 168")
                        .FontSize(10);
                    
                    column.Item().Text("3334 Gaflenz")
                        .FontSize(10);
                    
                    column.Item().Text("0677 61621048")
                        .FontSize(10);
                    
                    column.Item().Text("berchtold@holzbau-plan.at")
                        .FontSize(10);
                });

                row.RelativeItem().Column(column =>
                {
                    column.Item().AlignRight().Text("RECHNUNG")
                        .FontSize(20)
                        .SemiBold();
                    
                    column.Item().PaddingTop(10).AlignRight().Text(text =>
                    {
                        text.Span("Rechnungsnummer: ").SemiBold();
                        text.Span($"#{1000 + order.Id}");
                    });
                    
                    column.Item().AlignRight().Text(text =>
                    {
                        text.Span("Datum: ").SemiBold();
                        text.Span(order.BestellDatum.ToString("dd.MM.yyyy"));
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
                        col.Item().Text("Rechnungsadresse:").SemiBold();
                        col.Item().PaddingTop(5).Text($"{user.FirstName} {user.LastName}");
                        col.Item().Text(user.Email ?? "");
                    });

                    row.RelativeItem().Column(col =>
                    {
                        col.Item().Text("Lieferadresse:").SemiBold();
                        col.Item().PaddingTop(5).Text(order.LieferAdresse ?? "");
                        if (!string.IsNullOrEmpty(order.Postleitzahl) && !string.IsNullOrEmpty(order.Stadt))
                        {
                            col.Item().Text($"{order.Postleitzahl} {order.Stadt}");
                        }
                        if (!string.IsNullOrEmpty(order.Land))
                        {
                            col.Item().Text(order.Land);
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
                            columns.RelativeColumn(3);
                            columns.RelativeColumn(4);
                            columns.RelativeColumn(2);
                            columns.RelativeColumn(2);
                            columns.RelativeColumn(2);
                        });

                        table.Header(header =>
                        {
                            header.Cell().Element(CellStyle).Text("Pos").SemiBold();
                            header.Cell().Element(CellStyle).Text("Artikel").SemiBold();
                            header.Cell().Element(CellStyle).AlignRight().Text("Einheit").SemiBold();
                            header.Cell().Element(CellStyle).AlignRight().Text("Menge").SemiBold();
                            header.Cell().Element(CellStyle).AlignRight().Text("Einzelpreis").SemiBold();
                            header.Cell().Element(CellStyle).AlignRight().Text("Gesamt").SemiBold();

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
                            table.Cell().Element(CellStyle).Text(position.ToString());
                            table.Cell().Element(CellStyle).Text(item.Product.Name);
                            table.Cell().Element(CellStyle).AlignRight().Text(item.Product.Einheit);
                            table.Cell().Element(CellStyle).AlignRight().Text(item.Anzahl.ToString());
                            table.Cell().Element(CellStyle).AlignRight().Text($"€{item.EinzelPreis:N2}");
                            table.Cell().Element(CellStyle).AlignRight().Text($"€{item.GesamtPreis:N2}");

                            position++;

                            static IContainer CellStyle(IContainer container)
                            {
                                return container.BorderBottom(1)
                                    .BorderColor(Colors.Grey.Lighten3)
                                    .PaddingVertical(8);
                            }
                        }
                    });
                }

                // Summen
                column.Item().PaddingTop(20).AlignRight().Column(summaryColumn =>
                {
                    var zwischensumme = order.OrderItems.Sum(oi => oi.GesamtPreis);
                    var netto = zwischensumme / 1.20m;
                    var mwst = zwischensumme - netto;

                    summaryColumn.Item().Row(row =>
                    {
                        row.ConstantItem(150).Text("Netto:").SemiBold();
                        row.ConstantItem(100).AlignRight().Text($"€{netto:N2}");
                    });

                    summaryColumn.Item().Row(row =>
                    {
                        row.ConstantItem(150).Text("MwSt. (20%):").SemiBold();
                        row.ConstantItem(100).AlignRight().Text($"€{mwst:N2}");
                    });

                    summaryColumn.Item().PaddingTop(5).BorderTop(2).BorderColor(Colors.Grey.Medium)
                        .Row(row =>
                        {
                            row.ConstantItem(150).Text("Gesamtbetrag:").SemiBold().FontSize(12);
                            row.ConstantItem(100).AlignRight().Text($"€{order.GesamtBetrag:N2}").SemiBold().FontSize(12);
                        });
                });

                // Zahlungsinformationen
                column.Item().PaddingTop(30).Text("Zahlungsinformationen").SemiBold().FontSize(12);
                column.Item().PaddingTop(5).Text("Status: " + order.Status);
                column.Item().Text("Vielen Dank für Ihre Bestellung!");
            });
        }

        void ComposeFooter(IContainer container)
        {
            container.AlignCenter().Text(text =>
            {
                text.Span("Seite ");
                text.CurrentPageNumber();
                text.Span(" von ");
                text.TotalPages();
            });
        }
    }
}
