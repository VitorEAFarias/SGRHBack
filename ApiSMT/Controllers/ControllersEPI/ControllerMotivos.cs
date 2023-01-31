using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using System;
using Microsoft.AspNetCore.Authorization;
using ControleEPI.BLL.EPIMotivos;

namespace ApiSMT.Controllers.ControllersEPI
{
    /// <summary>
    /// Classe ControllerMotivos
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class ControllerMotivos : ControllerBase
    {
        private readonly IEPIMotivosBLL _motivos;

        /// <summary>
        /// Construtor ControllerMotivos
        /// </summary>
        /// <param name="motivos"></param>
        public ControllerMotivos(IEPIMotivosBLL motivos)
        {
            _motivos = motivos;
        }

        /// <summary>
        /// Localizar motivo
        /// </summary>
        /// <param name="idMotivo"></param>
        /// <returns></returns>
        [Authorize]
        [HttpGet("idMotivo")]
        public async Task<IActionResult> localizaMotivo(int idMotivo)
        {
            try
            {
                var localizaMotivo = await _motivos.getMotivo(idMotivo);

                if (localizaMotivo != null)
                {
                    return Ok(new { message = "Motivo encontrado", result = true, data = localizaMotivo });
                }
                else
                {
                    return BadRequest(new { message = "Motivo não encontrado", result = false });
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Localiza todos os motivos cadastrados
        /// </summary>
        /// <returns></returns>
        [Authorize]
        [HttpGet]
        public async Task<IActionResult> localizaMotivos()
        {
            try
            {
                var localizaMotivos = await _motivos.getMotivos();

                if (localizaMotivos != null)
                {
                    return Ok(new { message = "Motivos encontrados", result = true, data = localizaMotivos });
                }
                else
                {
                    return BadRequest(new { message = "Nenhum motivo encontrado", result = false });
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
