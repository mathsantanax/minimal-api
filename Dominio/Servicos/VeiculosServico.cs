using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using minimal_api.Dominio.Entidades;
using minimal_api.Dominio.Interfaces;
using minimal_api.Infraestrutura.DB;

namespace minimal_api.Dominio.Servicos
{
    public class VeiculosServico : IVeiculosServico
    {
        private readonly DbContesto dbContesto;
        public VeiculosServico(DbContesto db)
        {
            dbContesto = db;
        }
        public void Apagar(Veiculo veiculo)
        {
            dbContesto.Veiculos.Remove(veiculo);
            dbContesto.SaveChanges();
        }

        public void Atualizar(Veiculo veiculo)
        {
            dbContesto.Veiculos.Update(veiculo);
            dbContesto.SaveChanges();
        }

        public Veiculo? BuscaPorId(int id)
        {
            return dbContesto.Veiculos.Where(x => x.Id == id).FirstOrDefault();
        }

        public void Incluir(Veiculo veiculo)
        {
            dbContesto.Veiculos.Add(veiculo);
            dbContesto.SaveChanges();
        }

        public List<Veiculo> Todos(int? pagina = 1, string? nome = null, string? marca = null)
        {
            var query = dbContesto.Veiculos.AsQueryable();

            if(!string.IsNullOrEmpty(nome))
            {
                query = query.Where(x => EF.Functions.Like(x.Nome.ToLower(), $"%{nome.ToLower()}%"));
            }

            int itensPorPagina = 10;
            
            if(pagina != null)
            {
                query = query.Skip(((int)pagina - 1) * itensPorPagina).Take(itensPorPagina);
            }

            return query.ToList();
        }
    }
}