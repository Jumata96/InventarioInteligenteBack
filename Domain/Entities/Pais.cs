namespace InventarioInteligente.Domain.Entities
{
    public class Pais
    {
        public int PaisId { get; set; }
        public string Codigo { get; set; } = default!;     // varchar(2)
        public string Nombre { get; set; } = default!;     // varchar(100)
        public bool Activo { get; set; } = true;
        public DateTime FechaCreacion { get; set; }
        public DateTime? FechaEdicion { get; set; }
        public DateTime? FechaEliminacion { get; set; }

        public ICollection<Cliente> Clientes { get; set; } = new List<Cliente>();
        public ICollection<Impuesto> Impuestos { get; set; } = new List<Impuesto>();
    }
}
