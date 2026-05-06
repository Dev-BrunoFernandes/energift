using Energift.Fiap.Domain.Entities;
using Xunit;

namespace Energift.Tests
{
    public class UsuarioTests
    {
        [Fact]
        public void UsuarioModel_ShouldSetPropertiesCorrectly()
        {
            // Arrange
            var usuario = new UsuarioModel
            {
                Id = 1,
                Nome = "Teste User",
                Email = "teste@exemplo.com",
                WattCoinsBalance = 100
            };

            // Assert
            Assert.Equal(1, usuario.Id);
            Assert.Equal("Teste User", usuario.Nome);
            Assert.Equal("teste@exemplo.com", usuario.Email);
            Assert.Equal(100, usuario.WattCoinsBalance);
        }
    }
}
