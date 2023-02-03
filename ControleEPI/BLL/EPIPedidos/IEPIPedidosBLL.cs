using System.Collections.Generic;
using System.Threading.Tasks;
using ControleEPI.DTO;

namespace ControleEPI.BLL.EPIPedidos
{
    public interface IEPIPedidosBLL
    {
        Task<EPIPedidosDTO> Insert(EPIPedidosDTO pedido);
        Task<IList<PedidosUsuarioDTO>> getPedidos();
        Task<EPIPedidosDTO> getPedido(int Id);
        Task<PedidosDTO> getPedidoProduto(int Id);
        Task<IList<EPIPedidosDTO>> getTodosPedidos(int status);
        Task<IList<PedidosUsuarioDTO>> getPedidosUsuario(int Id);
        Task<EPIPedidosDTO> Update(EPIPedidosDTO pedido);
        Task<EPIPedidosDTO> aprovaPedido(EPIPedidosDTO aprovaPedido);
        Task<EPIPedidosDTO> reprovaPedido(int status, EPIPedidosDTO pedido);
        Task<EPIPedidosDTO> aprovaProdutoPedido(int idProduto, EPIPedidosDTO pedido);
        Task<EPIPedidosDTO> reprovaProdutoPedido(int idProduto, EPIPedidosDTO pedido);        
    }
}