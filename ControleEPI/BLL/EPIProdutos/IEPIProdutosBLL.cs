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
        Task<IList<EPIProdutosDTO>> verificaCategorias(int idCategoria);
        Task<IList<ProdutosDTO>> getProdutosSolicitacao();        
        Task<IList<ProdutosTamanhosDTO>> produtosTamanhos(string status);
        Task<ProdutosTamanhosDTO> localizaProdutoTamanhos(int idProduto);
        Task<EPIProdutosDTO> Update(EPIProdutosDTO produto);
    }
}
