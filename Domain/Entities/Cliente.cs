namespace InventarioInteligente.Domain.Entities
{
    public class Cliente
    {
        public int ClienteId { get; set; }
        public string Ruc { get; set; } = default!;          // varchar(13)
        public string Nombre { get; set; } = default!;       // varchar(200)
        public string? Email { get; set; }                   // varchar(200)
        public string? Telefono { get; set; }                // varchar(50)
        public string? Direccion { get; set; }               // varchar(300)
        public int PaisId { get; set; }
        public bool Activo { get; set; } = true;
        public DateTime FechaCreacion { get; set; }
        public DateTime? FechaEdicion { get; set; }
        public DateTime? FechaEliminacion { get; set; }

        public Pais Pais { get; set; } = default!;
    }
}
