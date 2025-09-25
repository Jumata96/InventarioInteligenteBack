using InventarioInteligenteBack.Domain.Entities;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace InventarioInteligenteBack.Application.Services
{
    public interface IFacturaPdfService
    {
        byte[] GenerarPdf(Factura factura, Pedido pedido);
    }

    public class FacturaPdfService : IFacturaPdfService
    {
        public byte[] GenerarPdf(Factura factura, Pedido pedido)
        {
            var doc = Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(PageSizes.A4);
                    page.Margin(30);
                    page.DefaultTextStyle(x => x.FontSize(12));

                    // Header
                    page.Header().Text($"Factura N° {factura.NumeroFactura}")
                        .FontSize(20).SemiBold().FontColor(Colors.Blue.Medium);

                    // Datos del cliente y factura
                    page.Content().Column(col =>
                    {
                        col.Item().Text($"Fecha: {factura.FechaEmision:dd/MM/yyyy}");
                        col.Item().Text($"Cliente ID: {pedido.ClienteId}");
                        col.Item().Text($"País ID: {pedido.PaisId}");
                        col.Item().Text($"Estado: {factura.Estado}");
                    });

                    // Detalle
                    page.Content().Table(table =>
                    {
                        table.ColumnsDefinition(c =>
                        {
                            c.RelativeColumn(3);
                            c.RelativeColumn(1);
                            c.RelativeColumn(1);
                            c.RelativeColumn(1);
                        });

                        table.Header(h =>
                        {
                            h.Cell().Text("Producto").SemiBold();
                            h.Cell().Text("Cantidad").SemiBold();
                            h.Cell().Text("Precio").SemiBold();
                            h.Cell().Text("Subtotal").SemiBold();
                        });

                        foreach (var d in pedido.Detalles)
                        {
                            table.Cell().Text(d.Producto.Nombre);
                            table.Cell().Text(d.Cantidad.ToString());
                            table.Cell().Text($"S/ {d.PrecioUnitario:F2}");
                            table.Cell().Text($"S/ {d.Subtotal:F2}");
                        }
                    });

                    // Totales
                    page.Content().PaddingTop(20).AlignRight().Column(c =>
                    {
                        c.Item().Text($"Subtotal: S/ {factura.Subtotal:F2}");
                        c.Item().Text($"Descuento: -S/ {factura.Descuento:F2}");
                        c.Item().Text($"Impuesto: S/ {factura.Impuesto:F2}");
                        c.Item().Text($"Total: S/ {factura.Total:F2}").Bold();
                    });

                    page.Footer().AlignCenter().Text("Gracias por su compra").FontSize(10);
                });
            });

            return doc.GeneratePdf();
        }
    }
}
