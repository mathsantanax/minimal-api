using System.Reflection;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.InMemory;
using Microsoft.Extensions.Configuration;
using minimal_api.Dominio.Entidades;
using minimal_api.Dominio.Servicos;
using minimal_api.Infraestrutura.DB;

namespace Test.Domain.Entidades
{
    [TestClass]
    public class AdministradorServicoTest
    {   
        private DbContesto CriarContextDeTeste()
        {
            var assemblyePath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            var path = Path.GetFullPath(Path.Combine(assemblyePath ?? "", "..", "..", ".."));

            var builder = new ConfigurationBuilder()
            .SetBasePath(path ?? Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .AddEnvironmentVariables();

            var Configuration = builder.Build();

            return new DbContesto(Configuration);
        }


        [TestMethod]
        public void TestandoSalvarAdministrador()
        {
            // Arrange
            var context = CriarContextDeTeste();
            context.Database.ExecuteSqlRaw("TRUNCATE TABLE Administradores");

            var adm = new Administrador();
            adm.Email = "teste@teste.com";
            adm.Senha = "teste";
            adm.Perfil = "Adm";

            var administradorServico = new AdministradorServico(context);
            // Act
            administradorServico.Incluir(adm);
            var admin = administradorServico.BuscarPorId(adm.Id);

            // Assert
            Assert.AreEqual(1, administradorServico.Todos(1).Count());
            Assert.AreEqual(1, admin.Id);
        }
        
    }
}