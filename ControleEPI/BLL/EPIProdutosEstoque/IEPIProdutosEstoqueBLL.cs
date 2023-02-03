using System.Collections.Generic;
using System.Threading.Tasks;
using ControleEPI.DTO;

namespace ControleEPI.BLL.EPIProdutosEstoque
{
    public interface IEPIProdutosEstoqueBLL
    {
        Task<EPIProdutosEstoqueDTO> Insert(EPIProdutosEstoqueDTO produto);
        Task<TodosProdutosEstoqueDTO> getProdutoEstoque(int id);
        Task<EPIProdutosEstoqueDTO> getProdutoEstoqueTamanho(int id, int idTamanho);
        Task<EPIProdutosEstoqueDTO> ativaDesativaProdutoEstoque(int idEstoque, string status);        
        Task<EPIProdutosEstoqueDTO> getProdutoExistente(int idProduto);
        Task<IList<EPIProdutosEstoqueDTO>> getProdutosExistentes(int idProdutos);
        Task<IList<TodosProdutosEstoqueDTO>> getProdutosEstoque();
        Task<EPIProdutosEstoqueDTO> Update(EPIProdutosEstoqueDTO produto);
    }
}
