namespace InventarioInteligenteBack.Domain.Entities
{
    public class Impuesto
    {
        public int ImpuestoId { get; set; }
        public int PaisId { get; set; }
        public string Nombre { get; set; } = default!;    // varchar(50)
        public decimal Porcentaje { get; set; }           // decimal(5,2) >= 0
        public bool Activo { get; set; } = true;
        public DateTime FechaCreacion { get; set; }
        public DateTime? FechaEdicion { get; set; }
        public DateTime? FechaEliminacion { get; set; }

        public Pais Pais { get; set; } = default!;
    }
}
