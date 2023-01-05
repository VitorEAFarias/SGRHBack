using ControleEPI.DTO;
using ControleEPI.DTO.FromBody;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ControleEPI.BLL
{
    public interface IEPIProdutosBLL
    {
        Task<EPIProdutosDTO> Insert(EPIProdutosDTO produto);
        Task<CertificadoProdutoDTO> getProduto(int id);
        Task<EPIProdutosDTO> localizaProduto(int id);
        Task<EPIProdutosDTO> ativaDesativaProduto(int id);
        Task<EPIProdutosDTO> getCertificadoProduto(int idCertificado);
        Task<EPIProdutosDTO> getNomeProduto(string nome);
        Task<IList<EPIProdutosDTO>> getProdutosSolicitacao();
        Task<IList<CertificadoProdutoDTO>> getProdutos();
        Task Update(EPIProdutosDTO produto);
    }
}
