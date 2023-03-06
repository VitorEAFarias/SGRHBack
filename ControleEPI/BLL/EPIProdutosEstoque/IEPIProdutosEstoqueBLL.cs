using System.Collections.Generic;
using System.Threading.Tasks;
using ControleEPI.DTO;

namespace ControleEPI.BLL.EPIProdutosEstoque
{
    public interface IEPIProdutosEstoqueBLL
    {
        Task<EPIProdutosEstoqueDTO> Insert(EPIProdutosEstoqueDTO produto);
        Task<TodosProdutosEstoqueDTO> getProdutoEstoque(int id);
        Task<EPIProdutosEstoqueDTO> ativaDesativaProdutoEstoque(int idEstoque);
        Task<IList<TodosProdutosEstoqueDTO>> getProdutosEstoque();
        Task<EPIProdutosEstoqueDTO> Update(EPIProdutosEstoqueDTO produto);
    }
}
