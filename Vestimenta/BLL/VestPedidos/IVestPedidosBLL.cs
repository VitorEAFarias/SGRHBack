﻿using System.Collections.Generic;
using System.Threading.Tasks;
using Vestimenta.DTO;

namespace Vestimenta.BLL.VestPedidos
{
    public interface IVestPedidosBLL
    {
        Task<VestPedidosDTO> Insert(VestPedidosDTO pedido);
        Task<IList<ItemDTO>> getPedidoItens(int Id);
        Task<CompraDTO> getPedido(int idPedido);
        Task<VestPedidosDTO> atualizaStatusPedidoItem(VestPedidosDTO pedidoItem);
        Task<IList<VestPedidosDTO>> atualizaStatusTodosPedidos(List<VestPedidosDTO> pedidosItens);
        Task<IList<ItemUsuarioDTO>> getPedidosStatus(int idStatus);
        Task<IList<ItemUsuarioDTO>> getPedidosUsuarios(int idUsuario);
        Task<IList<VestPedidosDTO>> getPedidos();
        Task<IList<PedidosPententesDTO>> getPedidosPendentes();
        Task<IList<VestPedidosDTO>> getLiberadoVinculo();
        Task Update(VestPedidosDTO pedido);
        Task Delete(int id);
    }
}
