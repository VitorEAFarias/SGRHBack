using ControleEPI.DTO._DbContext;
using ControleEPI.DTO;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using ControleEPI.DTO.FromBody;

namespace ControleEPI.DAL.EPIProdutos
{
    public class EPIProdutosDAL : IEPIProdutosDAL
    {
        private readonly AppDbContext _context;

        public EPIProdutosDAL(AppDbContext context)
        {
            _context = context;
        }

        public async Task<EPIProdutosDTO> localizaProduto(int id)
        {
            return await _context.EPIProdutos.FindAsync(id);
        }        

        public async Task<EPIProdutosDTO> getCertificadoProduto(int idCertificado)
        {
            return await _context.EPIProdutos.FromSqlRaw("SELECT * FROM EPIProdutos WHERE idCertificadoAprovacao = '" + idCertificado + "'").OrderBy(x => x.id).FirstOrDefaultAsync();
        }

        public async Task<EPIProdutosDTO> getNomeProduto(string nome)
        {
            return await _context.EPIProdutos.FromSqlRaw("SELECT * FROM EPIProdutos WHERE nome = '" + nome + "' AND ativo = 'S'").OrderBy(x => x.id).FirstOrDefaultAsync();
        }        

        public async Task<IList<EPIProdutosDTO>> produtosStatus(string status)
        {
            return await _context.EPIProdutos.FromSqlRaw("SELECT * FROM EPIProdutos WHERE ativo = '" + status + " '").OrderBy(p => p.id).ToListAsync();
        }

        public async Task<EPIProdutosDTO> verificaCategoria(int idCategoria)
        {
            return await _context.EPIProdutos.FromSqlRaw("SELECT * FROM EPIProdutos WHERE idCategoria = '" + idCategoria + "'").OrderBy(c => c.id).FirstOrDefaultAsync();
        }

        public async Task<IList<EPIProdutosDTO>> getProdutosSolicitacao()
        {
            return await _context.EPIProdutos.ToListAsync();
        }

        public async Task<EPIProdutosDTO> ativaDesativaProduto(int id)
        {
            return await _context.EPIProdutos.FindAsync(id);
        }

        public async Task<EPIProdutosDTO> Insert(EPIProdutosDTO produto)
        {
            _context.EPIProdutos.Add(produto);
            await _context.SaveChangesAsync();

            return produto;
        }

        public async Task<EPIProdutosDTO> Update(EPIProdutosDTO produto)
        {
            _context.ChangeTracker.Clear();

            _context.Entry(produto).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return produto;
        }
    }
}
