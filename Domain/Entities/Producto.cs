namespace InventarioInteligenteBack.Domain.Entities
{
    public class Producto
    {
        public int ProductoId { get; set; }
        public string Nombre { get; set; } = default!;
        public string? Descripcion { get; set; }
        public decimal Precio { get; set; }
        public int Stock { get; set; }
        public bool Activo { get; set; } = true;

        public DateTime FechaCreacion { get; set; }
        public DateTime? FechaEdicion { get; set; }
        public DateTime? FechaEliminacion { get; set; }
        public ICollection<DetallePedido> Detalles { get; set; } = new List<DetallePedido>();
    }
}
