using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using System;
using Microsoft.AspNetCore.Authorization;
using Vestimenta.BLL.VestPDF;

namespace ApiSMT.Controllers.ControllersVestimenta
{
    /// <summary>
    /// Classe de PDF
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class ControllerPDFVinculo : ControllerBase
    {
        private readonly IVestDinkPDFBLL _dadosPDF;

        /// <summary>
        /// Construtor VestimentaController
        /// </summary>
        /// <param name="dadosPDF"></param>
        public ControllerPDFVinculo(IVestDinkPDFBLL dadosPDF)
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
                    return BadRequest(new { message = "Erro ao gerar historico de itens", result = false });
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
