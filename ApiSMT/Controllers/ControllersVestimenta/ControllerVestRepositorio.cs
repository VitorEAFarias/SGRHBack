using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Vestimenta.DTO;
using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Authorization;
using Vestimenta.BLL.VestRepositorio;
using Vestimenta.BLL.VestCompras;
using Vestimenta.BLL.VestPDF;

namespace ApiSMT.Controllers.ControllersVestimenta
{
    /// <summary>
    /// Controller VestRepositorioController
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class ControllerVestRepositorio : ControllerBase
    {
        private readonly IVestRepositorioBLL _repositorio;
        private readonly IVestComprasBLL _compras;
        private readonly IVestDinkPDFBLL _relatorio;

        /// <summary>
        /// Construtor VestRepositorioController
        /// </summary>
        /// <param name="repositorio"></param>
        /// <param name="compras"></param>
        /// <param name="relatorio"></param>
        public ControllerVestRepositorio(IVestRepositorioBLL repositorio,IVestComprasBLL compras, IVestDinkPDFBLL relatorio)
        {
            _repositorio = repositorio;
            _compras = compras;
            _relatorio = relatorio;
        }

        /// <summary>
        /// Relatório de itens a serem entregues
        /// </summary>
        /// <returns></returns>
        [Authorize]
        [HttpGet("relatorio/{idCompra}")]
        public async Task<IActionResult> relatorioPDF(int idCompra)
        {
            try
            {
                var relatorioCompra = await _relatorio.relatorioCompra(idCompra);

                if (relatorioCompra != null)
                {
                    return Ok(new { message = "Relatório encontrado!!!", result = relatorioCompra });
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
        /// Retorna um item selecionado
        /// </summary>
        /// <param name="idRepositorio"></param>
        /// <returns></returns>
        [Authorize]
        [HttpGet("{id}")]
        public async Task<IActionResult> getRepositorio(int idRepositorio)
        {
            try
            {
                var checkRepositorio = await _repositorio.getRepositorio(idRepositorio);

                if (checkRepositorio != null)
                {
                    return Ok(new { message = "Item encontrado", result = true, repositorio = checkRepositorio });
                }
                else
                {
                    return BadRequest(new { message = "Item não encontrado", result = false });
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Seleciona todos os itens que serão enviados para compras
        /// </summary>
        /// <param name="status"></param>
        /// <returns></returns>
        [Authorize]
        [HttpGet("repositorio/{status}")]
        public async Task<IActionResult> getRepositorioStatus(string status)
        {
            try
            {
                var repositorio = await _repositorio.getRepositorioStatus(status);

                if (repositorio != null)
                {
                    return Ok(new { message = "Repositórios encontrados!!!", result = true, data = repositorio });
                }
                else
                {
                    return BadRequest(new { message = "Nenhum repositório encontrado", result = false });
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Insere item avulso para compras
        /// </summary>
        /// <param name="repositorioAvulso"></param>
        /// <returns></returns>
        [Authorize]
        [HttpPost("avulso")]
        public async Task<IActionResult> avulso([FromBody] List<VestRepositorioDTO> repositorioAvulso)
        {
            try
            {
                if (repositorioAvulso != null || !repositorioAvulso.Equals(0))
                {
                    foreach (var item in repositorioAvulso)
                    {
                        await _repositorio.Insert(item);
                    }                    

                    return Ok(new { message = "Item avulso inserido com sucesso!!!", result = true });
                }
                else
                {
                    return BadRequest(new { message = "Nenhum item avulso enviado", result = false });
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Envia itens para compra
        /// </summary>
        /// <param name="compras"></param>
        /// <returns></returns>
        [Authorize]
        [HttpPost]
        public async Task<IActionResult> enviarParaCompra([FromBody] VestComprasDTO compras)
        {
            try
            {
                var enviaCompra = await _compras.enviarParaCompra(compras);

                if (enviaCompra != null)
                {
                    return Ok(new { message = "Enviado para compras com sucesso!!!", result = true, data = enviaCompra });
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
    }
}
