namespace InventarioInteligente.Domain.Entities
{
    public class ReglaDescuento
    {
        public int ReglaId { get; set; }
        public string Nombre { get; set; } = default!;      // varchar(100)
        public string Tipo { get; set; } = default!;        // 'Porcentaje'|'Fijo'
        public decimal Valor { get; set; }                  // >= 0
        public decimal MinimoSubtotal { get; set; }         // >= 0
        public bool Activo { get; set; } = true;
        public DateTime FechaCreacion { get; set; }
        public DateTime? FechaEdicion { get; set; }
        public DateTime? FechaEliminacion { get; set; }
    }
}
