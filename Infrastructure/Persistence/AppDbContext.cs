using InventarioInteligenteBack.Domain.Entities;
using InventarioInteligenteBack.Infrastructure.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace InventarioInteligenteBack.Infrastructure.Persistence
{
    public class AppDbContext : IdentityDbContext<ApplicationUser>
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<Pais> Paises => Set<Pais>();
        public DbSet<Cliente> Clientes => Set<Cliente>();
        public DbSet<Impuesto> Impuestos => Set<Impuesto>();
        public DbSet<Producto> Productos => Set<Producto>();
        public DbSet<Pedido> Pedidos => Set<Pedido>();
        public DbSet<DetallePedido> DetallesPedido => Set<DetallePedido>();
        public DbSet<ReglaDescuento> ReglasDescuento => Set<ReglaDescuento>();
        public DbSet<Factura> Facturas => Set<Factura>(); // ✅ Nuevo DbSet

        protected override void OnModelCreating(ModelBuilder mb)
        {
            base.OnModelCreating(mb); // ⚠️ Incluye las tablas de Identity

            // ===== Paises =====
            mb.Entity<Pais>(e =>
            {
                e.ToTable("Paises");
                e.HasKey(x => x.PaisId);
                e.Property(x => x.Codigo).HasColumnType("varchar(2)").IsRequired();
                e.HasIndex(x => x.Codigo).IsUnique();
                e.Property(x => x.Nombre).HasColumnType("varchar(100)").IsRequired();
                e.Property(x => x.Estado).HasDefaultValue(1);
                e.Property(x => x.FechaCreacion).HasColumnType("datetime2").HasDefaultValueSql("SYSUTCDATETIME()");
            });

            // ===== Clientes =====
            mb.Entity<Cliente>(e =>
            {
                e.ToTable("Clientes");
                e.HasKey(x => x.ClienteId);
                e.Property(x => x.Ruc).HasColumnType("varchar(13)").IsRequired();
                e.HasIndex(x => x.Ruc).IsUnique();
                e.Property(x => x.Nombre).HasColumnType("varchar(200)").IsRequired();
                e.HasOne(x => x.Pais)
                 .WithMany(x => x.Clientes)
                 .HasForeignKey(x => x.PaisId)
                 .OnDelete(DeleteBehavior.NoAction);
            });

            // ===== Impuestos =====
            mb.Entity<Impuesto>(e =>
            {
                e.ToTable("Impuestos");
                e.HasKey(x => x.ImpuestoId);
                e.Property(x => x.Nombre).HasColumnType("varchar(50)").IsRequired();
                e.Property(x => x.Porcentaje).HasColumnType("decimal(5,2)");
                e.HasOne(x => x.Pais)
                 .WithMany(x => x.Impuestos)
                 .HasForeignKey(x => x.PaisId)
                 .OnDelete(DeleteBehavior.NoAction);
            });

            // ===== Productos =====
            mb.Entity<Producto>(e =>
            {
                e.ToTable("Productos");
                e.HasKey(x => x.ProductoId);
                e.Property(x => x.Nombre).HasColumnType("varchar(150)").IsRequired();
                e.HasIndex(x => x.Nombre).IsUnique();
                e.Property(x => x.Precio).HasColumnType("decimal(18,2)");
                e.ToTable(t =>
                {
                    t.HasCheckConstraint("CK_Productos_Precio_Positive", "[Precio] > 0");
                    t.HasCheckConstraint("CK_Productos_Stock_NonNegative", "[Stock] >= 0");
                });
            });

            // ===== Pedidos =====
            mb.Entity<Pedido>(e =>
            {
                e.ToTable("Pedidos");
                e.HasKey(x => x.PedidoId);
                e.Property(x => x.Estado).HasColumnType("varchar(20)").HasDefaultValue("Emitido");
                e.Property(x => x.Subtotal).HasColumnType("decimal(18,2)");
                e.Property(x => x.Total).HasColumnType("decimal(18,2)");
                e.Property(x => x.TotalFinal).HasColumnType("decimal(18,2)");
                e.HasOne(x => x.Cliente)
                 .WithMany()
                 .HasForeignKey(x => x.ClienteId)
                 .OnDelete(DeleteBehavior.NoAction);
                e.HasOne(x => x.Pais)
                 .WithMany()
                 .HasForeignKey(x => x.PaisId)
                 .OnDelete(DeleteBehavior.NoAction);
            });

            // ===== DetallesPedido =====
            mb.Entity<DetallePedido>(e =>
            {
                e.ToTable("DetallesPedido");
                e.HasKey(x => x.DetalleId);
                e.Property(x => x.PrecioUnitario).HasColumnType("decimal(18,2)");
                e.Property(x => x.Subtotal).HasColumnType("decimal(18,2)");
                e.HasOne(x => x.Pedido)
                 .WithMany(x => x.Detalles)
                 .HasForeignKey(x => x.PedidoId)
                 .OnDelete(DeleteBehavior.NoAction);
                e.HasOne(x => x.Producto)
                 .WithMany(x => x.Detalles)
                 .HasForeignKey(x => x.ProductoId)
                 .OnDelete(DeleteBehavior.NoAction);
                e.ToTable(t =>
                {
                    t.HasCheckConstraint("CK_Detalles_Cantidad_Positive", "[Cantidad] > 0");
                    t.HasCheckConstraint("CK_Detalles_PrecioUnitario_Positive", "[PrecioUnitario] > 0");
                });
            });

            // ===== ReglasDescuento =====
            mb.Entity<ReglaDescuento>(e =>
            {
                e.ToTable("ReglasDescuento");
                e.HasKey(x => x.ReglaId);
                e.Property(x => x.Nombre).HasColumnType("varchar(100)").IsRequired();
                e.Property(x => x.Tipo).HasColumnType("varchar(10)").IsRequired();
                e.Property(x => x.Valor).HasColumnType("decimal(18,2)");
                e.ToTable(t =>
                {
                    t.HasCheckConstraint("CK_Reglas_Tipo", "[Tipo] IN ('Porcentaje','Fijo')");
                });
            });

            // ===== Facturas =====
            mb.Entity<Factura>(e =>
            {
                e.ToTable("Facturas");
                e.HasKey(x => x.FacturaId);
                e.Property(x => x.NumeroFactura).HasColumnType("varchar(30)").IsRequired();
                e.Property(x => x.Subtotal).HasColumnType("decimal(18,2)");
                e.Property(x => x.Descuento).HasColumnType("decimal(18,2)");
                e.Property(x => x.Impuesto).HasColumnType("decimal(18,2)");
                e.Property(x => x.Total).HasColumnType("decimal(18,2)");
                e.Property(x => x.UrlPdf).HasColumnType("varchar(300)");
                e.HasOne(f => f.Pedido)
                    .WithOne(p => p.Factura)
                    .HasForeignKey<Factura>(f => f.PedidoId)
                    .OnDelete(DeleteBehavior.NoAction);
            });
        }
    }
}
