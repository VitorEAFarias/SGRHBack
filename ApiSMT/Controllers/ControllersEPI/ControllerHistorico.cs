using ControleEPI.BLL.EPIHistorico;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using System;

namespace ApiSMT.Controllers.ControllersEPI
{
    /// <summary>
    /// Classe de PDF
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class ControllerHistorico : ControllerBase
    {
        private readonly IEPIHistoricoBLL _dadosPDF;

        /// <summary>
        /// Construtor VestimentaController
        /// </summary>
        /// <param name="dadosPDF"></param>
        public ControllerHistorico(IEPIHistoricoBLL dadosPDF)
        {
            _dadosPDF = dadosPDF;
        }

        /// <summary>
        /// Lista historico de itens com colaborador e monta PDF
        /// </summary>
        /// <returns></returns>
        [Authorize]
        [HttpGet("{idUsuario}")]
        public async Task<IActionResult> getTodosStatus(int idUsuario)
        {
            try
            {
                var historicoItens = await _dadosPDF.dadosPDF(idUsuario);

                if (historicoItens != null)
                {
                    return Ok(new { message = "Histórico gerado com sucesso!!!", result = true, data = historicoItens });
                }
                else
                {
                    return BadRequest(new { message = "Erro ao gerar histórico de itens", result = false });
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
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
                var relatorioCompra = await _dadosPDF.relatorioCompra(idCompra);

                if (relatorioCompra != null)
                {
                    return Ok(new { message = "Relatório encontrado!!!", data = relatorioCompra, result = true });
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
