namespace Energift.Fiap.Domain.Entities
{
    public class GoalModel
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public int UsuarioId { get; set; }
        public int ImovelId { get; set; }
        public decimal TargetPercentReduction { get; set; } // ex: 10 (10%)
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public bool Achieved { get; set; } = false;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
