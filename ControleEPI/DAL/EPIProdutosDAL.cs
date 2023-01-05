using ControleEPI.DTO._DbContext;
using ControleEPI.DTO;
using ControleEPI.BLL;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using ControleEPI.DTO.FromBody;

namespace ControleEPI.DAL
{
    public class EPIProdutosDAL : IEPIProdutosBLL
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

        public async Task<CertificadoProdutoDTO> getProduto(int id)
        {
            var query = await (from EPIProdutos in _context.EPIProdutos
                               join EPICertificadoAprovacao in _context.EPICertificadoAprovacao on EPIProdutos.idCertificadoAprovacao equals EPICertificadoAprovacao.id
                               join EPICategoria in _context.EPICategoria on EPIProdutos.idCategoria equals EPICategoria.id
                               where EPIProdutos.id == id
                               select new
                               {
                                   id = EPIProdutos.id,
                                   nome = EPIProdutos.nome,
                                   categoria = EPICategoria.nome,
                                   idCertificado = EPICertificadoAprovacao.id,
                                   ca = EPICertificadoAprovacao.numero,
                                   preco = EPIProdutos.preco,
                                   ativo = EPIProdutos.ativo,
                                   validadeEmUso = EPIProdutos.validadeEmUso
                               }).OrderBy(x => x.id).FirstOrDefaultAsync();

            CertificadoProdutoDTO resultado = new CertificadoProdutoDTO();
            
            resultado = new CertificadoProdutoDTO
            {
                id = query.id,
                nomeProduto = query.nome,
                categoria = query.categoria,
                idCertificado = query.idCertificado,
                ca = query.ca,
                preco = query.preco,
                ativo = query.ativo,
                validadeEmUso = query.validadeEmUso
            };            

            return resultado;
        }

        public async Task<EPIProdutosDTO> getCertificadoProduto(int idCertificado)
        {
            return await _context.EPIProdutos.FromSqlRaw("SELECT * FROM EPIProdutos WHERE idCertificado = '" + idCertificado + "'").OrderBy(x => x.id).FirstOrDefaultAsync();
        }

        public async Task<EPIProdutosDTO> getNomeProduto(string nome)
        {
            return await _context.EPIProdutos.FromSqlRaw("SELECT * FROM EPIProdutos WHERE nome = '" + nome + "' AND ativo = 'S'").OrderBy(x => x.id).FirstOrDefaultAsync();
        }

        public async Task<IList<CertificadoProdutoDTO>> getProdutos()
        {
            var query = await (from EPIProdutos in _context.EPIProdutos
                               join EPICertificadoAprovacao in _context.EPICertificadoAprovacao on EPIProdutos.idCertificadoAprovacao equals EPICertificadoAprovacao.id
                               join EPICategoria in _context.EPICategoria on EPIProdutos.idCategoria equals EPICategoria.id
                               select new
                               {
                                   id = EPIProdutos.id,
                                   nome = EPIProdutos.nome,
                                   categoria = EPICategoria.nome,
                                   idCertificado = EPICertificadoAprovacao.id,
                                   ca = EPICertificadoAprovacao.numero,
                                   preco = EPIProdutos.preco,
                                   ativo = EPIProdutos.ativo,
                                   validadeEmUso = EPIProdutos.validadeEmUso
                               }).ToListAsync();

            List<CertificadoProdutoDTO> resultado = new List<CertificadoProdutoDTO>();

            foreach (var item in query)
            {
                resultado.Add(new CertificadoProdutoDTO
                {
                    id = item.id,
                    nomeProduto = item.nome,
                    categoria = item.categoria,
                    idCertificado = item.idCertificado,
                    ca = item.ca,
                    preco = item.preco,
                    ativo = item.ativo,
                    validadeEmUso = item.validadeEmUso
                });
            }

            return resultado;
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

        public async Task Update(EPIProdutosDTO produto)
        {
            _context.ChangeTracker.Clear();

            _context.Entry(produto).State = EntityState.Modified;
            await _context.SaveChangesAsync();
        }
    }
}
