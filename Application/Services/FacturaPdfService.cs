using InventarioInteligenteBack.Api.DTOs;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using System.Text.Json;

namespace InventarioInteligenteBack.Application.Services
{
    public interface IFacturaPdfService
    {
        byte[] GenerarPdf2(FacturaReadPdfDto facturaReadPdfDto);
    }

    public class FacturaPdfService : IFacturaPdfService
    {
        public byte[] GenerarPdf2(FacturaReadPdfDto factura)
        {
            var doc = Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(PageSizes.A4);
                    page.Margin(30);
                    page.DefaultTextStyle(x => x.FontSize(10));

                    // ------------------ HEADER ------------------
                    page.Header().Row(row =>
                    {
                        // Empresa
                        row.RelativeItem().AlignLeft().Column(s =>
                        {
                            s.Item().Text("Apptelink S.A.C.")
                                .FontSize(14).SemiBold().FontColor(Colors.Black);
                            s.Item().Text("RUC: 20612517143");
                            s.Item().Text("Av. Javier Prado Oeste Nro. 757 Magdalena del Mar, Lima - Perú");

                        });

                        // Info Factura
                        row.ConstantItem(220).Border(1).Padding(10).Column(s =>
                        {
                            s.Item().AlignCenter().Text("FACTURA ELECTRÓNICA")
                                .FontSize(12).SemiBold();
                            s.Item().AlignCenter().Text($"F001 - {factura.NumeroFactura}")
                                .FontSize(14).Bold().FontColor(Colors.Red.Medium);
                        });
                    });

                    // ------------------ CONTENT ------------------
                    page.Content().PaddingVertical(15).Column(col =>
                    {
                        // Cliente
                        col.Item().Border(1).Padding(10).Column(c =>
                        {
                            c.Item().Text($"Cliente: {factura.Cliente?.Nombre ?? "Cliente desconocido"}");
                            c.Item().Text($"RUC/DNI: {factura.Cliente?.Documento ?? "00000000000"}");
                            c.Item().Text($"Dirección: {factura.Cliente?.Direccion ?? "Sin dirección"}");
                            c.Item().Text($"Fecha de emisión: {factura.FechaEmision:dd/MM/yyyy}");
                        });

                        col.Item().PaddingTop(10);

                        // Tabla detalle
                        col.Item().Table(table =>
                        {
                            table.ColumnsDefinition(c =>
                            {
                                c.RelativeColumn(4); // Descripción
                                c.RelativeColumn(1); // Cantidad
                                c.RelativeColumn(2); // Precio Unit.
                                c.RelativeColumn(2); // Importe
                            });

                            // Encabezado
                            table.Header(h =>
                            {
                                h.Cell().Background(Colors.Grey.Lighten3).Padding(5).Text("Descripción").SemiBold();
                                h.Cell().Background(Colors.Grey.Lighten3).Padding(5).AlignCenter().Text("Cantidad").SemiBold();
                                h.Cell().Background(Colors.Grey.Lighten3).Padding(5).AlignRight().Text("P. Unitario").SemiBold();
                                h.Cell().Background(Colors.Grey.Lighten3).Padding(5).AlignRight().Text("Importe").SemiBold();
                            });

                            // Detalles dinámicos
                            if (factura.Detalles != null && factura.Detalles.Any())
                            {
                                foreach (var d in factura.Detalles)
                                {
                                    table.Cell().Padding(5).Text(d.Nombre ?? "Producto genérico");
                                    table.Cell().Padding(5).AlignCenter().Text(d.Cantidad.ToString());
                                    table.Cell().Padding(5).AlignRight().Text($"S/ {d.PrecioUnitario:F2}");
                                    table.Cell().Padding(5).AlignRight().Text($"S/ {d.Subtotal:F2}");
                                }
                            }
                            else
                            {
                                // Dummy si no hay detalles
                                table.Cell().Padding(5).Text("Producto de prueba");
                                table.Cell().Padding(5).AlignCenter().Text("1");
                                table.Cell().Padding(5).AlignRight().Text("S/ 100.00");
                                table.Cell().Padding(5).AlignRight().Text("S/ 100.00");
                            }
                        });

                        // Totales
                        col.Item().PaddingTop(20).AlignRight().Column(c =>
                        {
                            c.Item().Text($"Op. Gravada: S/ {factura.Subtotal:F2}");
                            c.Item().Text($"Descuento: -S/ {factura.Descuento:F2}");
                            c.Item().Text($"IGV (18%): S/ {factura.Impuesto:F2}");
                            c.Item().Text($"TOTAL: S/ {factura.Total:F2}")
                                .FontSize(12).Bold().FontColor(Colors.Red.Medium);
                        });
                    });

                    // ------------------ FOOTER ------------------
                    page.Footer().AlignCenter().Column(c =>
                    {
                        c.Item().Text("Representación impresa de la Factura Electrónica")
                            .FontSize(9).Italic();
                        c.Item().Text("Gracias por su compra")
                            .FontSize(9).Bold();
                    });
                });
            });

            return doc.GeneratePdf();
        }
    }
}