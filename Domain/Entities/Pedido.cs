using InventarioInteligenteBack.Infrastructure.Identity;

namespace InventarioInteligenteBack.Domain.Entities
{
    public class Pedido
    {
        public int PedidoId { get; set; }
        public int ClienteId { get; set; }
        public string UsuarioId { get; set; } = null!;
        public DateTime Fecha { get; set; }
        public int PaisId { get; set; }

        public decimal Subtotal { get; set; }
        public decimal Descuento { get; set; }  // ✅ descuento aplicado
        public decimal Impuesto { get; set; }
        public decimal Total { get; set; }
        public decimal TotalFinal { get; set; } // ✅ total con impuestos y descuentos

        public string? Secuencial { get; set; }
        public string Estado { get; set; } = "Emitido";

        public bool Activo { get; set; } = true;
        public DateTime FechaCreacion { get; set; }
        public DateTime? FechaEdicion { get; set; }
        public DateTime? FechaEliminacion { get; set; }

        // 🔹 Relaciones
        public Cliente Cliente { get; set; } = default!;
        public Pais Pais { get; set; } = default!;
        public ApplicationUser Usuario { get; set; } = default!;
        public ICollection<DetallePedido> Detalles { get; set; } = new List<DetallePedido>();

        // Relación 1:1 con Factura
        public Factura? Factura { get; set; }
    }
}
