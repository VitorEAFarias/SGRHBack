using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using ControleEPI.DTO;
using System;
using Microsoft.AspNetCore.Authorization;
using ControleEPI.BLL.EPICompras;

namespace ApiSMT.Controllers.ControllersEPI
{
    /// <summary>
    /// Classe que manipula as informações relacionadas a compras de produtos
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class ControllerCompras : ControllerBase
    {
        private readonly IEPIComprasBLL _compras;

        /// <summary>
        /// Construtor ComprasController
        /// </summary>
        /// <param name="compras"></param>
        public ControllerCompras(IEPIComprasBLL compras)
        {
            _compras = compras;
        }

        /// <summary>
        /// Lista todas as compras
        /// </summary>
        /// <returns></returns>
        [Authorize]
        [HttpGet("localizaTodasCompras")]
        public async Task<IActionResult> getTodasCompras()
        {
            try
            {
                var compras = await _compras.getTodasCompras();

                if (compras != null)
                {
                    return Ok(new { message = "Compras encontradas", result = true, data = compras });
                }
                else
                {
                    return BadRequest(new { message = "Nenhuma compra encontrada", result = false });
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
        /// <param name="status"></param>
        /// <returns></returns>
        [Authorize]
        [HttpGet("localizaCompras/{status}")]
        public async Task<IActionResult> getCompras(string status)
        {
            try
            {
                var compras = await _compras.getCompras(status);

                if (compras != null)
                {
                    return Ok(new { message = "Compras encontradas", result = true, data = compras });
                }
                else
                {
                    return BadRequest(new { message = "Nenhuma compra encontrada", result = false });
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
                var localizaCompra = await _compras.getCompra(id);

                if (localizaCompra != null)
                {
                    return Ok(new { message = "Compra encontrada!!!", result = true, data = localizaCompra });
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

        /// <summary>
        /// Efetuar compra e atualizar estoque
        /// </summary>
        /// <param name="compras"></param>
        /// <returns></returns>
        [Authorize]
        [HttpPut("compras")]
        public async Task<IActionResult> efetuarCompra([FromBody] EPIComprasDTO compras)
        {
            try
            {
                var efetuarCompra = await _compras.efetuarCompra(compras);

                if (efetuarCompra != null)
                {
                    return Ok(new { message = "Compra realizada com sucesso!!!", result = true, data = efetuarCompra });
                }
                else
                {
                    return BadRequest(new { message = "Erro ao efetuar compra", result = false });
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
        [HttpPut("reprovar")]
        public async Task<IActionResult> reprovaCompra([FromBody] EPIComprasDTO reprovarCompra)
        {
            try
            {
                var reprovaCompra = await _compras.reprovaCompra(reprovarCompra);

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
    }
}
