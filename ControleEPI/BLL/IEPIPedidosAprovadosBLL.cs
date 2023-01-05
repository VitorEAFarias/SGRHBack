using ControleEPI.DTO;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ControleEPI.BLL
{
    public interface IEPIPedidosAprovadosBLL
    {
        Task<EPIPedidosAprovadosDTO> Insert(EPIPedidosAprovadosDTO produtoAprovado);
        Task<EPIPedidosAprovadosDTO> getProdutoAprovado(int Id, string status);
        Task<EPIPedidosAprovadosDTO> verificaProdutoAprovado(int idProduto, int idPedido, int idTamanho);
        Task<IList<EPIPedidosAprovadosDTO>> getProdutosAprovados(string status);
        Task Update(EPIPedidosAprovadosDTO produtoAprovado);
    }
}
