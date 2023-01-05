using System.Collections.Generic;
using System.Threading.Tasks;
using Vestimenta.DTO;

namespace Vestimenta.BLL
{
    public interface IPedidosVestBLL
    {
        Task<VestPedidosDTO> Insert(VestPedidosDTO pedido);
        Task<VestPedidosDTO> getPedido(int Id);
        Task<IList<VestPedidosDTO>> getPedidosStatus(int idStatus);
        Task<IList<VestPedidosDTO>> getPedidosUsuarios(int idUsuario);
        Task<IList<VestPedidosDTO>> getPedidos();
        Task<IList<VestPedidosDTO>> getPedidosPendentes();
        Task<IList<VestPedidosDTO>> getLiberadoVinculo();
        Task Update(VestPedidosDTO pedido);
        Task Delete(int id);
    }
}
