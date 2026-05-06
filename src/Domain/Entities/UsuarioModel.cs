namespace Energift.Fiap.Domain.Entities
{
    public class UsuarioModel
    {
        public int Id { get; set; } // simplificado: int id
        public string Nome { get; set; }
        public string Email { get; set; }
        public int WattCoinsBalance { get; set; }
    }
}
