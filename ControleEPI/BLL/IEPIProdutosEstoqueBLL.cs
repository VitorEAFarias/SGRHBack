using System.Collections.Generic;
using System.Threading.Tasks;
using ControleEPI.DTO;

namespace ControleEPI.BLL
{
    public interface IEPIProdutosEstoqueBLL
    {
        Task<EPIProdutosEstoqueDTO> Insert(EPIProdutosEstoqueDTO produto);
        Task<EPIProdutosEstoqueDTO> getProdutoEstoque(int id);
        Task<EPIProdutosEstoqueDTO> getProdutoEstoqueTamanho(int id, int idTamanho);
        Task<EPIProdutosEstoqueDTO> getProdutoExistente(int idProduto);
        Task<IList<EPIProdutosEstoqueDTO>> getProdutosExistentes(int idProdutos);
        Task<IList<EPIProdutosEstoqueDTO>> getProdutosEstoque();        
        Task Update(EPIProdutosEstoqueDTO produto);
    }
}
