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
        public decimal Subtotal { get; set; }              // decimal(18,2)
        public decimal Descuento { get; set; }             // decimal(18,2) default 0
        public decimal Impuesto { get; set; }              // decimal(18,2)
        public decimal Total { get; set; }                 // decimal(18,2)
        public string? Secuencial { get; set; }            // varchar(30) unique
        public string Estado { get; set; } = "Emitido";    // varchar(20)
        public bool Activo { get; set; } = true;
        public DateTime FechaCreacion { get; set; }
        public DateTime? FechaEdicion { get; set; }
        public DateTime? FechaEliminacion { get; set; }

        public Cliente Cliente { get; set; } = default!;
        public Pais Pais { get; set; } = default!;
        public ApplicationUser Usuario { get; set; } = default!;
        public ICollection<DetallePedido> Detalles { get; set; } = new List<DetallePedido>();
    }
}
