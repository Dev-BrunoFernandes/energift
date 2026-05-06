namespace Energift.Fiap.Domain.Entities
{
    public class RankingModel
    {
        public int Id { get; set; }

        // Nome do usuário que aparece no ranking
        public string? Nome { get; set; }

        // Economia total (kWh, percentual, ou ponto gamificado)
        public decimal Pontos { get; set; }
    }
}
