namespace InventarioInteligente.Domain.Entities
{
    public class DetallePedido
    {
        public int DetalleId { get; set; }
        public int PedidoId { get; set; }
        public int ProductoId { get; set; }
        public int Cantidad { get; set; }                  // > 0
        public decimal PrecioUnitario { get; set; }        // > 0
        public decimal Subtotal { get; set; }              // decimal(18,2)
        public bool Activo { get; set; } = true;
        public DateTime FechaCreacion { get; set; }
        public DateTime? FechaEdicion { get; set; }
        public DateTime? FechaEliminacion { get; set; }

        public Pedido Pedido { get; set; } = default!;
        public Producto Producto { get; set; } = default!;
    }
}
