using System.Collections.Generic;
using System.Threading.Tasks;
using ControleEPI.DTO;

namespace ControleEPI.BLL
{
    public interface IEPIPedidosBLL
    {        
        Task<EPIPedidosDTO> Insert(EPIPedidosDTO pedido);
        Task<IList<EPIPedidosDTO>> getPedidos();
        Task<EPIPedidosDTO> getPedido(int Id);
        Task<IList<EPIPedidosDTO>> getTodosPedidos(int status);
        Task<IList<EPIPedidosDTO>> getPedidosUsuario(int Id);
        Task Update(EPIPedidosDTO pedido);
    }
}