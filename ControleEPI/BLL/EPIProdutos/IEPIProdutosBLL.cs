using ControleEPI.DTO;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ControleEPI.BLL.EPIProdutos
{
    public interface IEPIProdutosBLL
    {
        Task<EPIProdutosDTO> Insert(EPIProdutosDTO produto);        
        Task<ProdutosDTO> localizaProduto(int id);
        Task<EPIProdutosDTO> ativaDesativaProduto(string status, int id);
        Task<EPIProdutosDTO> getCertificadoProduto(int idCertificado);
        Task<IList<EPIProdutosDTO>> verificaCategoria(int idCategoria);
        Task<EPIProdutosDTO> getNomeProduto(string nome);
        Task<IList<ProdutosDTO>> getProdutosSolicitacao();        
        Task<IList<ProdutosTamanhosDTO>> produtosTamanhos(string status);
        Task<EPIProdutosDTO> Update(EPIProdutosDTO produto);
    }
}
