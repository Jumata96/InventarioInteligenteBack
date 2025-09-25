namespace InventarioInteligenteBack.Application.Interfaces
{
    public interface IDescuentoService
    {
        decimal CalcularDescuento(decimal subtotal, int totalUnidades);
    }
}

