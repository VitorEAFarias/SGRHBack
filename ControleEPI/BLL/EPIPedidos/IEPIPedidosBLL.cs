using System.Collections.Generic;
using System.Threading.Tasks;
using ControleEPI.DTO;

namespace ControleEPI.BLL.EPIPedidos
{
    public interface IEPIPedidosBLL
    {
        Task<EPIPedidosDTO> Insert(EPIPedidosDTO pedido);
        Task<IList<PedidosUsuarioDTO>> getPedidos();
        Task<PedidosDTO> getPedidoProduto(int Id);
        Task<IList<EPIPedidosDTO>> getTodosPedidos(int status);
        Task<IList<PedidosUsuarioDTO>> localizaPedidosUsuarioStatus(int idUsuario, int idStatus);
        Task<IList<PedidosUsuarioDTO>> getPedidosUsuario(int Id);
        Task<EPIPedidosDTO> aprovaPedido(EPIPedidosDTO aprovaPedido);
        Task<EPIPedidosDTO> reprovaPedido(int status, EPIPedidosDTO pedido);
        Task<EPIPedidosDTO> aprovaProdutoPedido(EPIPedidosDTO pedido, int idProduto, int idTamanho);
        Task<EPIPedidosDTO> reprovaProdutoPedido(EPIPedidosDTO pedido, int idProduto, int idTamanho);
        Task<IList<Produtos>> liberarParaVinculo(int idPedido);
    }
}