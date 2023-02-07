using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Vestimenta.DTO;
using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Authorization;
using Utilitarios.Utilitários.email;
using Vestimenta.BLL.VestRepositorio;
using Vestimenta.BLL.VestVinculo;
using Vestimenta.BLL.VestPedidos;
using RH.BLL.RHUsuarios;
using Vestimenta.BLL.VestStatus;
using Vestimenta.BLL.VestEstoque;
using Vestimenta.BLL.VestLog;
using Vestimenta.BLL.VestVestimenta;
using RH.BLL.RHDepartamentos;
using RH.BLL.RHContratos;

namespace ApiSMT.Controllers.ControllersVestimenta
{
    /// <summary>
    /// Classe de pedidos de vestimenta
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class ControllerVestPedidos : ControllerBase
    {
        private readonly IVestPedidosBLL _pedidosVest;
        private readonly IRHConUserBLL _usuario;
        private readonly IVestStatusBLL _status;
        private readonly IVestEstoqueBLL _estoque;
        private readonly IVestRepositorioBLL _repositorio;
        private readonly IVestVinculoBLL _itemVinculo;
        private readonly IVestLogBLL _log;
        private readonly IMailService _mail;
        private readonly IVestVestimentaBLL _vestimenta;
        private readonly IRHDepartamentosBLL _departamento;
        private readonly IRHEmpContratosBLL _contrato;

        /// <summary>
        /// Construtor de Pedidos
        /// </summary>
        /// <param name="pedidosVest"></param>
        /// <param name="usuario"></param>
        /// <param name="status"></param>
        /// <param name="estoque"></param>
        /// <param name="repositorio"></param>
        /// <param name="itemVinculo"></param>
        /// <param name="log"></param>
        /// <param name="mail"></param>
        /// <param name="vestimenta"></param>
        /// <param name="departamento"></param>
        /// <param name="contrato"></param>
        public ControllerVestPedidos(IVestPedidosBLL pedidosVest, IRHConUserBLL usuario, IVestStatusBLL status, IVestEstoqueBLL estoque, IVestRepositorioBLL repositorio,
            IVestVinculoBLL itemVinculo, IVestLogBLL log, IMailService mail, IVestVestimentaBLL vestimenta, IRHDepartamentosBLL departamento, IRHEmpContratosBLL contrato)
        {
            _pedidosVest = pedidosVest;
            _usuario = usuario;
            _status = status;
            _estoque = estoque;
            _repositorio = repositorio;
            _itemVinculo = itemVinculo;
            _log = log;
            _mail = mail;
            _vestimenta = vestimenta;
            _departamento = departamento;
            _contrato = contrato;
        }

        /// <summary>
        /// Insere um novo pedido
        /// </summary>
        /// <param name="pedido"></param>
        /// <returns></returns>
        [Authorize]
        [HttpPost]
        public async Task<IActionResult> postPedido([FromBody] VestPedidosDTO pedido)
        {
            try
            {
                var novoPedido = await _pedidosVest.Insert(pedido);

                if (novoPedido != null)
                {
                    return Ok(new { message = "Novo pedido inserido com sucesso!!!", result = true, data = novoPedido });
                }
                else
                {
                    return BadRequest(new { message = "Erro ao inserir novo pedido, verifique se tem email funcional vinculado ao colaborador", result = false });
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Atualiza status do pedido e do item
        /// </summary>
        /// <param name="pedidoItem"></param>
        /// <returns></returns>
        [Authorize]
        [HttpPut("{id}")]
        public async Task<IActionResult> atualizaStatusPedidoItem([FromBody] VestPedidosDTO pedidoItem)
        {
            try
            {
                var atualizaStatusPedido = await _pedidosVest.atualizaStatusPedidoItem(pedidoItem);

                if (atualizaStatusPedido != null)
                {
                    return Ok(new { message = "Status do pedido atualizado com sucesso!!!", result = true, data = atualizaStatusPedido });
                }
                else
                {
                    return BadRequest(new { message = "Erro ao atualizar status do pedido", result = false });
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Lista pedidos por usuarios
        /// </summary>
        /// <param name="idUsuario"></param>
        /// <returns></returns>
        [Authorize]
        [HttpGet("usuario/{idUsuario}")]
        public async Task<ActionResult> getPedidosUsuario(int idUsuario)
        {
            try
            {
                var pedidosUsuario = await _pedidosVest.getPedidosUsuarios(idUsuario);

                if (pedidosUsuario != null)
                {
                    return Ok(new { message = "Pedidos encontrados!!!", result = true, data = pedidosUsuario });
                }   
                else
                {
                    return BadRequest(new { message = "Nenhum pedido com esse usuário localizado", result = false });
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Lista pedidos por status
        /// </summary>
        /// <returns></returns>
        [Authorize]
        [HttpGet("status/{idStatus}")]
        public async Task<IActionResult> getPedidosStatus(int idStatus)
        {
            try
            {
                var pedidos = await _pedidosVest.getPedidosStatus(idStatus);

                if (pedidos != null)
                {
                    return Ok(new { message = "Pedidos encontrados!!!", result = true, data = pedidos });
                }
                else
                {
                    return BadRequest(new { message = "Pedidos não encontrados", result = false });
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Pedidos liberados para vinculo 
        /// </summary>
        /// <returns></returns>
        [Authorize]
        [HttpGet("vinculo/{idPedido}")]
        public async Task<IActionResult> getLiberadoVinculo(int idPedido)
        {
            try
            {
                var getPedidos = await _pedidosVest.getPedidoItens(idPedido);                

                if (getPedidos != null)
                {
                    return Ok(new { message = "Itens encontrados!!!", result = true, data = getPedidos });
                }
                else
                {
                    return BadRequest(new { message = "Nenhum item liberado para vinculo foi encontrado", result = false });
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Atualiza status dos itens de todos os pedidos
        /// </summary>
        /// <returns></returns>
        [Authorize]
        [HttpPut("itens/{ids}")]
        public async Task<IActionResult> atualizaStatusTodosPedidos([FromBody] List<VestPedidosDTO> pedidosItens)
        {
            try
            {
                var atualizaStautsPedidos = await _pedidosVest.atualizaStatusTodosPedidos(pedidosItens);

                if (atualizaStautsPedidos != null)
                {
                    return Ok(new { message = "Status dos pedidos atualizados com sucesso!!!", result = true, data = true });
                }
                else
                {
                    return BadRequest(new { message = "Erro ao atualizar status dos pedidos", result = false });
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);                
            }
        }

        /// <summary>
        /// Get todos os itens com seus respectivos pedidos
        /// </summary>
        /// <returns></returns>
        [Authorize]
        [HttpGet]
        public async Task<IActionResult> getItensPedidos()
        {
            try
            {
                var pedidos = await _pedidosVest.getPedidosPendentes();

                if (pedidos != null)
                {
                    return Ok(new { message = "Pedidos encontrados!!!", result = true, data = pedidos });
                }
                else
                {
                    return BadRequest(new { message = "Nenhum pedido pendente encontrado", result = false });
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
        [HttpGet("{id}")]
        public async Task<IActionResult> getPedido(int id)
        {
            try
            {                
                var compra = await _pedidosVest.getPedido(id);

                if (compra != null)
                {
                    return Ok(new { message = "Pedido encontrado!!!", data = compra, result = true });
                }
                else
                {
                    return BadRequest(new { message = "Pedido não encontrado", result = false });
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
