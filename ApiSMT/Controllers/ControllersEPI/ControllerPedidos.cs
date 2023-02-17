using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using ControleEPI.DTO;
using System;
using Microsoft.AspNetCore.Authorization;
using ControleEPI.BLL.EPIPedidos;
using Vestimenta.DTO;

namespace ApiSMT.Controllers.ControllersEPI
{
    /// <summary>
    /// Classe de manipulação de pedidos de epi's
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class ControllerPedidos : ControllerBase
    {
        private readonly IEPIPedidosBLL _pedidos;

        /// <summary>
        /// Construtor PedidosController
        /// </summary>
        /// <param name="pedidos"></param>
        public ControllerPedidos(IEPIPedidosBLL pedidos)
        {
            _pedidos = pedidos;
        }

        /// <summary>
        /// insere um pedido a ser feito
        /// </summary>
        /// <param name="pedido"></param>
        /// <returns></returns>
        [Authorize]
        [HttpPost]
        public async Task<IActionResult> inserePedido([FromBody] EPIPedidosDTO pedido)
        {
            try
            {   
                if (pedido.produtos != null || !pedido.produtos.Equals(0))
                {
                    var novoPedido = await _pedidos.Insert(pedido);

                    if (novoPedido != null)
                    {
                        return Ok(new { message = "Pedido inserido com sucesso!!!", result = true, data = novoPedido });
                    }
                    else
                    {
                        return BadRequest(new { message = "Erro ao inserir novo pedido, verifique no RH se as informações do seu usuário está correto", result = false });
                    }
                }
                else
                {
                    return BadRequest(new { message = "Nenhum produto selecionado para o pedido", result = false });
                }
                                                 
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }        
        }

        /// <summary>
        /// Aprovar pedido
        /// </summary>
        /// <param name="pedido"></param>
        /// <returns></returns>
        [Authorize]
        [HttpPut("aprovaPedido")]
        public async Task<IActionResult> aprovaPedido([FromBody] EPIPedidosDTO pedido)
        {
            try
            {
                var aprovarPedido = await _pedidos.aprovaPedido(pedido);

                if (aprovarPedido != null)
                {
                    return Ok(new { message = "Pedido aprovado com sucesso!!!", result = true });
                }
                else
                {
                    return BadRequest(new { message = "Erro ao aprovar pedido, verifique as informações do colaborador no Portal do RH", result = false });
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Aprova um produto do pedido
        /// </summary>
        /// <param name="pedido"></param>
        /// <param name="idProduto"></param>
        /// <param name="idTamanho"></param>
        /// <returns></returns>
        [Authorize]
        [HttpPut("aprovaProdutoPedido/{idProduto}/{idTamanho}")]
        public async Task<IActionResult> aprovaProdutoPedido([FromBody] EPIPedidosDTO pedido, int idProduto, int idTamanho)
        {
            try
            {
                var aprovaProdutoPedido = await _pedidos.aprovaProdutoPedido(pedido, idProduto, idTamanho);

                if (aprovaProdutoPedido != null)
                {
                    return Ok(new { message = "Produto aprovado com sucesso!!!", result = true });
                }
                else
                {
                    return BadRequest(new { message = "Erro ao aprovar produto do pedido '" + pedido.id + "'", result = false });
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Reprovar produto do pedido
        /// </summary>
        /// <param name="pedido"></param>
        /// <param name="idProduto"></param>
        /// <param name="idTamanho"></param>
        /// <returns></returns>
        [Authorize]
        [HttpPut("reprovarProdutoPedido/{idProduto}/{idTamanho}")]
        public async Task<IActionResult> reprovaProdutoPedido([FromBody] EPIPedidosDTO pedido, int idProduto, int idTamanho)
        {
            try
            {
                var reprovaProdutoPedido = await _pedidos.reprovaProdutoPedido(pedido, idProduto, idTamanho);

                if (reprovaProdutoPedido != null)
                {
                    return Ok(new { message = "Produto reprovado com sucesso!!!", result = true });
                }
                else
                {
                    return BadRequest(new { message = "Erro ao reprovar produto do pedido '" + pedido.id + "'", result = false });
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }    
        }

        /// <summary>
        /// Reprova pedido de EPI
        /// </summary>
        /// <param name="status"></param>
        /// <param name="pedido"></param>
        /// <returns></returns>
        [Authorize]
        [HttpPut("reprovarPedido/{status}")]
        public async Task<IActionResult> reprovaPedido(int status, [FromBody] EPIPedidosDTO pedido)
        {
            try
            {
                var reprovaPedido = await _pedidos.reprovaPedido(status, pedido);

                if (reprovaPedido != null)
                {
                    return Ok(new { message = "Pedido reprovado com sucesso!!!", result = true });
                }
                else
                {
                    return BadRequest(new { message = "Erro ao reprovar pedido", result = false });
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Seleciona um pedido
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Authorize]
        [HttpGet("localizaPedido/{id}")]
        public async Task<IActionResult> GetPedido(int id)
        {
            try
            {
                var pedido = await _pedidos.getPedidoProduto(id);

                if (pedido != null)
                {
                    return Ok(new { message = "Pedido encontrado", data = pedido, result = true });
                }
                else
                {
                    return BadRequest(new { message = "Pedido não encontrado", result = false });   
                }
            }
            catch (System.Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Localiza pedidos por usuario e status
        /// </summary>
        /// <param name="idUsuario"></param>
        /// <param name="idStatus"></param>
        /// <returns></returns>
        [Authorize]
        [HttpGet("usuarioStatus/{idUsuario}/{idStatus}")]
        public async Task<IActionResult> pedidosUsuarioStatus(int idUsuario, int idStatus)
        {
            try
            {
                var localizaPedidosUsuarioStatus = await _pedidos.localizaPedidosUsuarioStatus(idUsuario, idStatus);

                if (localizaPedidosUsuarioStatus != null)
                {
                    return Ok(new { message = "Pedidos encontrados!!!", result = true, data = localizaPedidosUsuarioStatus });
                }
                else
                {
                    return BadRequest(new { message = "Nenhum pedido encontrado", result = false });
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Seleciona os produto ao qual fazem parte de uma categoria especifica
        /// </summary>
        /// <param name="idUsuario"></param>
        /// <returns></returns>
        [Authorize]
        [HttpGet("usuario/{idUsuario}")]
        public async Task<IActionResult> getPedidosUsuario(int idUsuario)
        {
            try
            {
                var pedidos = await _pedidos.getPedidosUsuario(idUsuario);

                if (pedidos != null)
                {
                    return Ok(new { message = "Pedidos encontrados", result = true, data = pedidos });
                }
                else
                {
                    return BadRequest(new { message = "Nenhum pedido encontrado", result = false });
                }
            }
            catch (System.Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Lista todos os pedidos
        /// </summary>
        /// <returns></returns>
        [Authorize]
        [HttpGet]
        public async Task<IActionResult> getTodosPedidos()
        {
            try
            {
                var localizaPedidos = await _pedidos.getPedidos();

                if (localizaPedidos != null)
                {
                    return Ok(new { message = "Pedidos encontrados", result = true, data = localizaPedidos });
                }
                else
                {
                    return BadRequest(new { message = "Nenhum pedido encontrado", result = false });
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Lista todos os pedidos
        /// </summary>
        /// <param name="status"></param>
        /// <returns></returns>
        [Authorize]
        [HttpGet("{status}")]
        public async Task<IActionResult> getPedidos(int status)
        {
            try
            {
                var pedidos = await _pedidos.getTodosPedidos(status);

                if (pedidos != null)
                {
                    return Ok(new { message = "lista encontrada", result = true, lista = pedidos });
                }
                else
                {
                    return BadRequest(new { message = "Nenhum pedido encontrado", result = false});
                }                
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Liberar produtos para vinculo ao colaborador
        /// </summary>
        /// <param name="idPedido"></param>
        /// <returns></returns>
        [Authorize]
        [HttpPut("liberarVinculo/{idPedido}")]
        public async Task<IActionResult> liberarParaVinculo(int idPedido)
        {
            try
            {
                var liberarVinculo = await _pedidos.liberarParaVinculo(idPedido);

                if (liberarVinculo != null)
                {
                    return Ok(new { message = "Produtos liberados para vínculo", result = true, data = liberarVinculo });
                }
                else
                {
                    return BadRequest(new { message = "Erro ao liberar produtos para o colaborador", result = false });
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
