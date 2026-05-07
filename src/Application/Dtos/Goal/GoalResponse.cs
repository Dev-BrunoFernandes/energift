namespace Energift.Fiap.Application.Dtos.Goal
{
    public class GoalResponse
    {
        public Guid Id { get; set; }
        public int UsuarioId { get; set; }
        public int ImovelId { get; set; }
        public decimal TargetPercentReduction { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public bool Achieved { get; set; }
    }
}
