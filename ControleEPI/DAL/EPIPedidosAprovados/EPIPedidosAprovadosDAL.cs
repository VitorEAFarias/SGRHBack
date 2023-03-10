using ControleEPI.DTO._DbContext;
using ControleEPI.DTO;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;

namespace ControleEPI.DAL.EPIPedidosAprovados
{
    public class EPIPedidosAprovadosDAL : IEPIPedidosAprovadosDAL
    {
        private readonly AppDbContext _context;
        public EPIPedidosAprovadosDAL(AppDbContext context)
        {
            _context = context;
        }
        public async Task<EPIPedidosAprovadosDTO> getProdutoAprovado(int Id, string status)
        {
            return await _context.EPIPedidosAprovados.FromSqlRaw("SELECT * FROM EPIPedidosAprovados WHERE enviadoCompra = '" + status + "' AND id = '" + Id + "'" +
                "").OrderBy(x => x.id).FirstOrDefaultAsync();
        }

        public async Task<IList<EPIPedidosAprovadosDTO>> getProdutosAprovados(string statusCompra, string statusVinculo)
        {
            return await _context.EPIPedidosAprovados.FromSqlRaw("SELECT * FROM EPIPedidosAprovados WHERE liberadoVinculo = '" +statusVinculo + "' AND enviadoCompra = '" + statusCompra + "'").ToListAsync();
        }

        public async Task<EPIPedidosAprovadosDTO> Insert(EPIPedidosAprovadosDTO produtoAprovado)
        {
            _context.ChangeTracker.Clear();

            _context.EPIPedidosAprovados.Add(produtoAprovado);
            await _context.SaveChangesAsync();

            return produtoAprovado;
        }

        public async Task<EPIPedidosAprovadosDTO> Update(EPIPedidosAprovadosDTO produtoAprovado)
        {
            _context.ChangeTracker.Clear();

            _context.Entry(produtoAprovado).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return produtoAprovado;
        }

        public async Task<EPIPedidosAprovadosDTO> verificaProdutoAprovado(int idProduto, int idPedido, int idTamanho)
        {
            return await _context.EPIPedidosAprovados.FromSqlRaw("SELECT idProduto, idPedido FROM EPIPedidosAprovados WHERE idProduto = '" + idProduto + "' AND" +
                "idPedido = '" + idPedido + "' AND idTamanho = '" + idTamanho + "' AND enviadoCompra = 'S'").OrderBy(x => x.id).FirstOrDefaultAsync();
        }
    }
}
