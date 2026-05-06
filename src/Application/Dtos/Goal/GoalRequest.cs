namespace Energift.Fiap.Application.Dtos.Goal
{
    public class GoalRequest
    {
        public int UsuarioId { get; set; }
        public int ImovelId { get; set; }
        public decimal TargetPercentReduction { get; set; } // ex 10 => 10%
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
    }
}
