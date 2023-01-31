using ControleEPI.DAL.EPIPedidosAprovados;
using ControleEPI.DTO;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ControleEPI.BLL.EPIPedidosAprovados
{
    public class EPIPedidosAprovadosBLL : IEPIPedidosAprovadosBLL
    {
        private readonly IEPIPedidosAprovadosDAL _pedidosAprovados;

        public EPIPedidosAprovadosBLL(IEPIPedidosAprovadosDAL pedidosAprovados)
        {
            _pedidosAprovados = pedidosAprovados;
        }

        public async Task<EPIPedidosAprovadosDTO> getProdutoAprovado(int Id, string status)
        {
            try
            {
                var localizaProdutoAprovado = await _pedidosAprovados.getProdutoAprovado(Id, status);

                if (localizaProdutoAprovado != null)
                {
                    return localizaProdutoAprovado;
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

        public async Task<IList<EPIPedidosAprovadosDTO>> getProdutosAprovados(string status)
        {
            try
            {
                var localizaProdutosAprovados = await _pedidosAprovados.getProdutosAprovados(status);

                if (localizaProdutosAprovados != null)
                {
                    return localizaProdutosAprovados;
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

        public async Task<EPIPedidosAprovadosDTO> Insert(EPIPedidosAprovadosDTO produtoAprovado)
        {
            try
            {
                var insereProdutoAprovado = await _pedidosAprovados.Insert(produtoAprovado);

                if (insereProdutoAprovado != null)
                {
                    return insereProdutoAprovado;
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

        public async Task<EPIPedidosAprovadosDTO> Update(EPIPedidosAprovadosDTO produtoAprovado)
        {
            try
            {
                var atualizaProduto = await _pedidosAprovados.Update(produtoAprovado);

                if (atualizaProduto != null)
                {
                    return atualizaProduto;
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

        public async Task<EPIPedidosAprovadosDTO> verificaProdutoAprovado(int idProduto, int idPedido, int idTamanho)
        {
            try
            {
                var verificaProdutoAprovado = await _pedidosAprovados.verificaProdutoAprovado(idProduto, idPedido, idTamanho);

                if (verificaProdutoAprovado != null)
                {
                    return verificaProdutoAprovado;
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
