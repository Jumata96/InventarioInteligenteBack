using InventarioInteligenteBack.Application.Interfaces;

namespace InventarioInteligenteBack.Application.Services
{
    public class DescuentoService : IDescuentoService
    {
        public decimal CalcularDescuento(decimal subtotal, int totalUnidades)
        {
            decimal descuento = 0;

            // 🔹 Reglas de volumen
            if (totalUnidades > 10)
                descuento += subtotal * 0.10m; // 10%
            else if (totalUnidades > 5)
                descuento += subtotal * 0.05m; // 5%

            // 🔹 Regla de monto
            if (subtotal > 1000)
                descuento += subtotal * 0.15m; // 15%

            return descuento;
        }
    }
}
