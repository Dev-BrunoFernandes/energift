namespace Energift.Fiap.Application.Dtos.Consumo
{
    public class ConsumoResponse
    {
        public Guid Id { get; set; }
        public int UsuarioId { get; set; }
        public int ImovelId { get; set; }
        public DateTime Referencia { get; set; }
        public decimal Kwh { get; set; }
        public decimal Valor { get; set; }
    }
}
