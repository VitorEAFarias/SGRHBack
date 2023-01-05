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
    public class EPICertificadoAprovacaoDAL : IEPICertificadoAprovacaoBLL
    {
        private readonly AppDbContext _context;

        public EPICertificadoAprovacaoDAL(AppDbContext context)
        {
            _context = context;
        }

        public async Task<EPICertificadoAprovacaoDTO> getCertificado(int id)
        {
            return await _context.EPICertificadoAprovacao.FindAsync(id);
        }

        public async Task<CertificadoProdutoDTO> getCertificadoProduto(int idCertificadoAprovacao)
        {
            var query = await (from EPICertificadoAprovacao in _context.EPICertificadoAprovacao
                               join EPIProdutos in _context.EPIProdutos on EPICertificadoAprovacao.id equals EPIProdutos.idCertificadoAprovacao
                               join EPICategoria in _context.EPICategoria on EPIProdutos.idCategoria equals EPICategoria.id
                               where EPICertificadoAprovacao.id == idCertificadoAprovacao
                               select new
                               {
                                   id = EPIProdutos.id,
                                   nome = EPIProdutos.nome,
                                   categoria = EPICategoria.nome,
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
                ca = query.ca,
                preco = query.preco,
                ativo = query.ativo,
                validadeEmUso = query.validadeEmUso
            };            

            return resultado;
        }

        public async Task<IList<EPICertificadoAprovacaoDTO>> getCertificadosNumero()
        {
            return await _context.EPICertificadoAprovacao.ToListAsync();
        }

        public async Task<IList<CertificadoProdutoDTO>> getCertificados()
        {
            var query = await (from EPICertificadoAprovacao in _context.EPICertificadoAprovacao
                               join EPIProdutos in _context.EPIProdutos on EPICertificadoAprovacao.id equals EPIProdutos.idCertificadoAprovacao
                               join EPICategoria in _context.EPICategoria on EPIProdutos.idCategoria equals EPICategoria.id
                               select new
                               {
                                   id = EPIProdutos.id,
                                   nome = EPIProdutos.nome,
                                   categoria = EPICategoria.nome,
                                   ca = EPICertificadoAprovacao.numero,
                                   preco = EPIProdutos.preco
                               }).ToListAsync();

            List<CertificadoProdutoDTO> resultado = new List<CertificadoProdutoDTO>();

            foreach (var item in query)
            {
                resultado.Add(new CertificadoProdutoDTO
                { 
                    id = item.id,
                    nomeProduto = item.nome,
                    categoria = item.categoria,
                    ca = item.ca,
                    preco = item.preco
                });
            }

            return resultado;
        }

        public async Task<EPICertificadoAprovacaoDTO> getValorCertificado(string valor)
        {
            return await _context.EPICertificadoAprovacao.FromSqlRaw("SELECT * FROM EPICertificadoAprovacao WHERE numero = '" + valor + "'")
                .OrderBy(x => x.id).FirstOrDefaultAsync();
        }

        public async Task<EPICertificadoAprovacaoDTO> Insert(EPICertificadoAprovacaoDTO certificado)
        {
            _context.EPICertificadoAprovacao.Add(certificado);
            await _context.SaveChangesAsync();

            return certificado;
        }

        public async Task Update(EPICertificadoAprovacaoDTO certificado)
        {
            _context.ChangeTracker.Clear();
            _context.Entry(certificado).State = EntityState.Modified;

            await _context.SaveChangesAsync();
        }
    }
}
