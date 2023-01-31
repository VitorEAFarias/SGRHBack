using ControleEPI.DAL.EPIPedidos;
using ControleEPI.DTO;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ControleEPI.BLL.EPIPedidos
{
    public class EPIPedidosBLL : IEPIPedidosBLL
    {
        private readonly IEPIPedidosDAL _pedidos;

        public EPIPedidosBLL(IEPIPedidosDAL pedidos)
        {
            _pedidos = pedidos;
        }

        public async Task<EPIPedidosDTO> getPedido(int Id)
        {
            try
            {
                var localizaPedido = await _pedidos.getPedido(Id);

                if (localizaPedido != null)
                {
                    return localizaPedido;
                }
                else
                {
                    return null;
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<IList<EPIPedidosDTO>> getPedidos()
        {
            try
            {
                var localizaPedidos = await _pedidos.getPedidos();

                if (localizaPedidos != null)
                {
                    return localizaPedidos;
                }
                else
                {
                    return null;
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<IList<EPIPedidosDTO>> getPedidosUsuario(int Id)
        {
            try
            {
                var localizaPedidoUsuario = await _pedidos.getPedidosUsuario(Id);

                if (localizaPedidoUsuario != null)
                {
                    return localizaPedidoUsuario;
                }
                else
                {
                    return null;
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<IList<EPIPedidosDTO>> getTodosPedidos(int status)
        {
            try
            {
                var localizaPedidosStatus = await _pedidos.getTodosPedidos(status);

                if (localizaPedidosStatus != null)
                {
                    return localizaPedidosStatus;
                }
                else
                {
                    return null;
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<EPIPedidosDTO> Insert(EPIPedidosDTO pedido)
        {
            try
            {
                var inserePedido = await _pedidos.Insert(pedido);

                if (inserePedido != null)
                {
                    return inserePedido;
                }
                else
                {
                    return null;
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<EPIPedidosDTO> Update(EPIPedidosDTO pedido)
        {
            try
            {
                var atualizaPedidos = await _pedidos.Update(pedido);

                if (atualizaPedidos != null)
                {
                    return atualizaPedidos;
                }
                else
                {
                    return null;    
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}
