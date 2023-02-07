using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Vestimenta.DTO;
using System;
using Microsoft.AspNetCore.Authorization;
using Vestimenta.BLL.VestCompras;

namespace ApiSMT.Controllers.ControllersVestimenta
{
    /// <summary>
    /// Classe de Compras
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class ControllerVestCompras : ControllerBase
    {
        private readonly IVestComprasBLL _compras;

        /// <summary>
        /// Construtor de Compras
        /// </summary>
        /// <param name="compras"></param>
        public ControllerVestCompras(IVestComprasBLL compras)
        {
            _compras = compras;
        }

        /// <summary>
        /// Cadastra uma nova compra
        /// </summary>
        /// <param name="compra"></param>
        /// <returns></returns>
        [Authorize]
        [HttpPost]
        public async Task<IActionResult> postCadastrarCompra([FromBody] VestComprasDTO compra)
        {
            try
            {
                var novaCompra = await _compras.Insert(compra);

                if (novaCompra != null)
                {
                    return Ok(new { message = "Compra cadastrada com sucesso!!!", result = true, data = novaCompra });
                }
                else
                {
                    return BadRequest(new { message = "Erro ao inserir compra", result = false });
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }        

        /// <summary>
        /// Compra aprovada
        /// </summary>
        /// <param name="processoCompra"></param>
        /// <returns></returns>
        [Authorize]
        [HttpPut("processoCompra/{idCompra}")]
        public async Task<IActionResult> processoDeCompra([FromBody] VestComprasDTO processoCompra)
        {
            try
            {
                var processoDeCompras = await _compras.processoDeCompra(processoCompra);

                if (processoDeCompras != null)
                {
                    return Ok(new { message = "Itens aprovados com sucesso!!!", result = true });
                }
                else
                {
                    return BadRequest(new { message = "Erro ao atualizar processo de compra, verifique as configurações do colaborador no Portal do RH", result = false });
                }                
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Reprovar compra
        /// </summary>
        /// <param name="reprovarCompra"></param>
        /// <returns></returns>
        [Authorize]
        [HttpPut("reprovarCompra/{idCompra}")]
        public async Task<IActionResult> reprovarCompra([FromBody] VestComprasDTO reprovarCompra)
        {
            try
            {
                var reprovaCompra = await _compras.reprovarCompra(reprovarCompra);

                if (reprovaCompra != null)
                {
                    return Ok(new { message = "Compra reprovada com sucesso!!!", result = true, data = reprovaCompra });
                }   
                else
                {
                    return BadRequest(new { message = "Erro ao reprovar compra", result = false });
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Item comprado e enviado para estoque
        /// </summary>
        /// <param name="comprarItens"></param>
        /// <returns></returns>
        [Authorize]
        [HttpPut("comprarItens/{idCompra}")]
        public async Task<IActionResult> comprarItem([FromBody] VestComprasDTO comprarItens)
        {
            try
            {
                var compraItens = await _compras.comprarItem(comprarItens);

                if (comprarItens != null)
                {
                    return Ok(new { message = "Itens comprados com sucesso!!!", result = true });
                }
                else
                {
                    return BadRequest(new { message = "Erro ao efeutar compra", result = false });
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }        

        /// <summary>
        /// Lista todas as compras
        /// </summary>
        /// <returns></returns>
        [Authorize]
        [HttpGet]
        public async Task<IActionResult> getCompras()
        {
            try
            {                                
                var compras = await _compras.getCompras();

                if (compras != null)
                {
                    return Ok(new { message = "lista encontrada", result = true, data = compras });
                }
                else
                {
                    return BadRequest(new { message = "Nenhuma compra encontrada", result = false});
                }

            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Seleciona uma compra
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Authorize]
        [HttpGet("{id}")]
        public async Task<IActionResult> getCompra(int id)
        {
            try
            {
                var checkCompra = await _compras.getCompra(id);

                if (checkCompra != null)
                {
                    return Ok(new { message = "Compra encontrada!!!", result = true, data = checkCompra });
                }
                else
                {
                    return BadRequest(new { message = "Compra não encontrada", result = false });
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }        
    }
}
