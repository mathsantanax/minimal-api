using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Threading.Tasks;
using minimal_api.Dominio.DTOs;
using minimal_api.Dominio.Entidades;
using minimal_api.Dominio.Interfaces;
using minimal_api.Infraestrutura.DB;

namespace minimal_api.Dominio.Servicos
{
    public class AdministradorServico : IAdministradorServicos
    { 
        private readonly DbContesto dbContesto;
        public AdministradorServico(DbContesto db)
        {
            dbContesto = db;
        }
        public Administrador? Login(LoginDTO loginDTO)
        {
            var adm  = dbContesto.Administradores.Where(a => a.Email == loginDTO.Email && a.Senha == loginDTO.Senha).FirstOrDefault();
            return adm;
        }
    }
}