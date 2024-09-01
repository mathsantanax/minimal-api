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

        public Administrador? BuscarPorId(int? id)
        {
            return dbContesto.Administradores.Where(x => x.Id == id).FirstOrDefault();
        }

        public Administrador Incluir(Administrador administrador)
        {
            dbContesto.Administradores.Add(administrador);
            dbContesto.SaveChanges();

            return administrador;
        }

        public Administrador? Login(LoginDTO loginDTO)
        {
            var adm  = dbContesto.Administradores.Where(a => a.Email == loginDTO.Email && a.Senha == loginDTO.Senha).FirstOrDefault();
            return adm;
        }

        public List<Administrador> Todos(int? pagina)
        {
            var query = dbContesto.Administradores.AsQueryable();

            int itensPorPagina = 10;
            
            if(pagina != null)
            {
                query = query.Skip(((int)pagina - 1) * itensPorPagina).Take(itensPorPagina);
            }

            return query.ToList();
        }
    }
}