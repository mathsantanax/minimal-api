using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using minimal_api.Dominio.DTOs;
using minimal_api.Dominio.Entidades;
using minimal_api.Dominio.Servicos;

namespace minimal_api.Dominio.Interfaces
{
    public interface IAdministradorServicos
    {
        Administrador? Login(LoginDTO loginDTO);
        Administrador Incluir(Administrador administrador);
        List<Administrador> Todos(int? pagina);

        Administrador? BuscarPorId(int? id);
    }
}