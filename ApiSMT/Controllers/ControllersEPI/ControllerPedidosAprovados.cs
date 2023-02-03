using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using ControleEPI.DTO;
using System.Collections.Generic;
using System;
using Microsoft.AspNetCore.Authorization;
using ControleEPI.BLL.EPIPedidosAprovados;

namespace ApiSMT.Controllers.ControllersEPI
{
    /// <summary>
    /// Classe ProdutosAprovadosController
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class ControllerPedidosAprovados : ControllerBase
    {
        private readonly IEPIPedidosAprovadosBLL _pedidosAprovados;

        /// <summary>
        /// Construtor ProdutosAprovadosController
        /// </summary>
        /// <param name="pedidosAprovados"></param>
        public ControllerPedidosAprovados(IEPIPedidosAprovadosBLL pedidosAprovados)
        {
            _pedidosAprovados = pedidosAprovados;
        }

        /// <summary>
        /// Envia produtos avulsos para compras
        /// </summary>
        /// <returns></returns>
        [Authorize]
        [HttpPost]
        public async Task<IActionResult> produtosAvulsos([FromBody] List<EPIPedidosAprovadosDTO> itensAvulsos)
        {
            try
            {
                var produtosAvulsos = await _pedidosAprovados.insereProdutosAprovados(itensAvulsos);

                if (produtosAvulsos != null)
                {
                    return Ok(new { message = "Itens avulsos inseridos com sucesso!!!", result = true });
                }
                else
                {
                    return BadRequest(new { message = "Erro ao inserir itens avulsos", result = false });   
                }                
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Enviar produtos para compras
        /// </summary>
        /// <param name="enviaCompra"></param>
        /// <param name="idUsuario"></param>
        /// <returns></returns>
        [Authorize]
        [HttpPut("enviarCompra/{idUsuario}")]
        public async Task<IActionResult> enviaParaCompras([FromBody] List<EPIPedidosAprovadosDTO> enviaCompra, int idUsuario)
        {
            try
            {
                var enviaCompras = await _pedidosAprovados.enviaParaCompras(enviaCompra, idUsuario);

                if (enviaCompras != null)
                {
                    return Ok(new { message = "Itens enviados para compra com sucesso!!!", result = true });
                }
                else
                {
                    return BadRequest(new { message = "Erro ao enviar para compras", result = false });
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
                
        /// <summary>
        /// Aprovar para vinculo
        /// </summary>
        /// <returns></returns>
        [Authorize]
        [HttpPut("aprovarVinculo")]
        public async Task<IActionResult> aprovarVinculo([FromBody] List<EPIPedidosAprovadosDTO> enviaVinculo)
        {
            try
            {
                var atualizaPedidosAprovados = await _pedidosAprovados.atualizaVinculos(enviaVinculo);

                if (atualizaPedidosAprovados != null)
                {
                    return Ok(new { message = "Produtos liberados para vinculo com sucesso!!!", result = true });
                }
                else
                {
                    return BadRequest(new { message = "Erro ao liberar produtos para vinculo", result = false });
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Lista todos os produtos aprovados
        /// </summary>
        /// <param name="statusCompra"></param>
        /// <param name="statusVinculo"></param>
        /// <returns></returns>
        [Authorize]
        [HttpGet("produtosAprovados/{statusCompra}/{statusVinculo}")]
        public async Task<IActionResult> produtosAprovados(string statusCompra, string statusVinculo)
        {
            try
            {
                var produtosAprovados = await _pedidosAprovados.getProdutosAprovados(statusCompra, statusVinculo);

                if (produtosAprovados != null)
                {
                    return Ok(new { message = "Produtos encontrados", result = true, data = produtosAprovados});
                }
                else
                {
                    return BadRequest(new { message = "Nenhum produto encontrado", result = false });
                }
            }
            catch (System.Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
