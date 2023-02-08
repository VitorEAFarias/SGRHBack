using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Vestimenta.DTO;
using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Authorization;
using Vestimenta.DTO.FromBody;
using Vestimenta.BLL.VestVinculo;
using Vestimenta.BLL.VestPedidos;

namespace ApiSMT.Controllers.ControllersVestimenta
{
    /// <summary>
    /// Classe VestVinculoController
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class ControllerVestVinculo : ControllerBase
    {
        private readonly IVestVinculoBLL _vinculo;
        private readonly IVestPedidosBLL _pedidos;

        /// <summary>
        /// Construtor VestVinculoController
        /// </summary>
        /// <param name="vinculo"></param>
        /// <param name="pedidos"></param>
        public ControllerVestVinculo(IVestVinculoBLL vinculo, IVestPedidosBLL pedidos)
        {
            _vinculo = vinculo;
            _pedidos = pedidos;
        }

        /// <summary>
        /// Cadastra vestimenta vinculada
        /// </summary>
        /// <param name="historico"></param>
        /// <returns></returns>
        [Authorize]
        [HttpPost]
        public async Task<IActionResult> vinculoHistorico([FromBody] VestHistoricoVinculadoDTO historico)
        {
            try
            {
                var historicos = await _vinculo.vinculoHistorico(historico);

                if (historicos != null)
                {
                    return Ok(new { message = "Histórico encontrado!!!", result = true, data = historicos });
                }
                else
                {
                    return BadRequest(new { message = "Nenhum histórico encontrado", result = false });
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Insere um novo status
        /// </summary>
        /// <param name="itemVinculo"></param>
        /// <param name="idUsuario"></param>
        /// <returns></returns>
        [Authorize]
        [HttpPut("{idUsuario}")]
        public async Task<IActionResult> atualizaVinculo(int idUsuario, [FromBody] VestVinculoDTO itemVinculo)
        {
            try
            {
                var atualizarVinculo = await _vinculo.atualizaVinculo(idUsuario, itemVinculo);

                if (atualizarVinculo != null)
                {
                    return Ok(new { message = "Vinculo atualizado com sucesso!!!", result = true, data = atualizarVinculo });
                }
                else
                {
                    return BadRequest(new { message = "Erro ao atualizar vinculo", result = false });
                }
                
            }
            catch (System.Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Desvincula vestimenta
        /// </summary>
        /// <returns></returns>
        [Authorize]
        [HttpPut("desvincular/{enviarEstoque}/{idVinculo}")]
        public async Task<IActionResult> retiraItemVinculo(bool enviarEstoque, int idVinculo)
        {
            try
            {
                var retirarItemVinculo = await _vinculo.retiraItemVinculo(enviarEstoque, idVinculo);

                if (retirarItemVinculo != null)
                {
                    return Ok(new { message = "Vinculo retirado com sucesso!!!", result = true, data = retirarItemVinculo });
                }
                else
                {
                    return BadRequest(new { message = "Erro ao retirar item do vinculo", result = false });
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Função que vincula itens com colaborador
        /// </summary>
        /// <param name="pedidosItens"></param>
        /// <param name="idUsuario"></param>
        /// <param name="senha"></param>
        /// <returns></returns>
        [Authorize]
        [HttpPut("verificacao/{idUsuario}/{senha}")]
        public async Task<IActionResult> aceitaVinculo(int idUsuario, string senha, [FromBody] List<VestPedidoItensVinculoDTO> pedidosItens)
        {
            try
            {
                var aceitarVinculo = await _vinculo.aceitaVinculo(idUsuario, senha, pedidosItens);

                if (aceitarVinculo != null)
                {
                    return Ok(new { message = "Itens adquiridos com sucesso!!!", result = true });
                }
                else
                {
                    return BadRequest(new { message = "Erro ao vincular itens com colaborador", result = false });
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Get de vinculos pendentes
        /// </summary>
        /// <param name="idStatus"></param>
        /// <param name="idUsuario"></param>
        /// <returns></returns>
        [Authorize]
        [HttpGet("status/{idStatus}/{idUsuario}")]
        public async Task<IActionResult> getStatus(int idStatus, int idUsuario)
        {
            try
            {
                var itensPendentes = await _vinculo.getVinculoPendente(idStatus, idUsuario);

                if (itensPendentes != null)
                {
                    return Ok(new { message = "Itens pendentes encontrados!!!", result = true, data = itensPendentes });
                }
                else
                {
                    return BadRequest(new { message = "Nenhum item pendente encontrado", result = false });
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }        
        
        /// <summary>
        /// get todas as situações do colaborador
        /// </summary>
        /// <param name="idUsuario"></param>
        /// <returns></returns>
        [Authorize]
        [HttpGet("situacoes/{idUsuario}")]
        public async Task<IActionResult> getSituações(int idUsuario)
        {
            try
            {
                var getPendente = await _vinculo.getItensUsuarios(idUsuario);
                var getPedidos = await _pedidos.getPedidosUsuarios(idUsuario);

                List<object> pendentes = new List<object>();
                List<object> vinculados = new List<object>();
                List<object> pedidosFinalizados = new List<object>();
                List<object> pedidosPendentes = new List<object>();                
                List<object> pedidosReprovados = new List<object>();

                foreach (var item in getPendente)
                {
                    if (item.status == 6)
                    {
                        vinculados.Add(new { 
                            item = item.id
                        });
                    }
                    else
                    {
                        pendentes.Add(new {
                            item = item.id
                        });
                    }
                }

                foreach (var item in getPedidos)
                {
                    if (item.status.Equals(2))
                    {
                        pedidosFinalizados.Add(new { 
                            item = item.id
                        });

                    }
                    else if (item.status.Equals(1))
                    {
                        pedidosPendentes.Add(new {
                            item = item.id
                        });
                    }
                    else
                    {
                        pedidosReprovados.Add(new {
                            item = item.id
                        });
                    }
                }

                return Ok(new { message = "Numeros encontrados!!!", result = true, vinculado = vinculados.Count, pendente = pendentes.Count, pedidosFinalizados = pedidosFinalizados.Count,
                    pedidosPendentes = pedidosPendentes.Count, pedidosReprovados = pedidosReprovados.Count });
                
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Pega todos os itens vinculados com um usuario
        /// </summary>
        /// <param name="idUsuario"></param>
        /// <returns></returns>
        [Authorize]
        [HttpGet("vinculados/{idUsuario}")]
        public async Task<IActionResult> getItensVinculados(int idUsuario)
        {
            try
            {
                var checkVinculados = await _vinculo.getItensVinculados(idUsuario);

                if (checkVinculados != null)
                {                    
                    return Ok(new { message = "Itens encontrados", data = checkVinculados, result = true });                    
                }
                else
                {
                    return BadRequest(new { message = "Nenhum item vinculado com esse colaborador", result = false });
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
