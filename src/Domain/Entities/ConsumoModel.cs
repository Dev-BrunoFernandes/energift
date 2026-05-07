namespace Energift.Fiap.Domain.Entities
{
    public class ConsumoModel
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public int UsuarioId { get; set; }
        public int ImovelId { get; set; }
        public DateTime Referencia { get; set; } // data do mês (ex: 2025-11-01)
        public decimal Kwh { get; set; }
        public decimal Valor { get; set; } // valor da conta em moeda
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
