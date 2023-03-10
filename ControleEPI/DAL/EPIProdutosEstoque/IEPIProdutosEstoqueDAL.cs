using ControleEPI.DTO;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ControleEPI.DAL.EPIProdutosEstoque
{
    public interface IEPIProdutosEstoqueDAL
    {
        Task<EPIProdutosEstoqueDTO> Insert(EPIProdutosEstoqueDTO produto);
        Task<EPIProdutosEstoqueDTO> getProdutoEstoque(int id);
        Task<EPIProdutosEstoqueDTO> getProdutoEstoqueTamanho(int id, int idTamanho);
        Task<EPIProdutosEstoqueDTO> getProdutoExistente(int idProduto);
        Task<IList<EPIProdutosEstoqueDTO>> getProdutosExistentes(int idProdutos);
        Task<IList<EPIProdutosEstoqueDTO>> getProdutosEstoque();
        Task<EPIProdutosEstoqueDTO> Update(EPIProdutosEstoqueDTO produto);
    }
}
