namespace Energift.Fiap.Application.Dtos.CalculateCoins
{
    public class CalculateCoinsRequest
    {
        public int UsuarioId { get; set; }
        public int ImovelId { get; set; }
        public DateTime Referencia { get; set; } // novo registro
        public decimal Kwh { get; set; }
        public decimal Valor { get; set; }
        // opcional: previous month kWh se já conhecido; se não, service buscará histórico
        public decimal? PreviousKwh { get; set; }
    }
}
