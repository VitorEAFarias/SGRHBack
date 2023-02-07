using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Vestimenta.DTO;
using System;
using Microsoft.AspNetCore.Authorization;
using Vestimenta.BLL.VestVestimenta;

namespace ApiSMT.Controllers.ControllersVestimenta
{
    /// <summary>
    /// Classe de vestimentas
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class ControllerVestimenta : ControllerBase
    {
        private readonly IVestVestimentaBLL _vestimenta;

        /// <summary>
        /// Construtor VestimentaController
        /// </summary>
        /// <param name="vestimenta"></param>
        public ControllerVestimenta(IVestVestimentaBLL vestimenta)
        {
            _vestimenta = vestimenta;
        }

        /// <summary>
        /// Insere uma nova vestimenta
        /// </summary>
        /// <param name="vestimenta"></param>
        /// <returns></returns>
        [Authorize]
        [HttpPost]
        public async Task<ActionResult<VestVestimentaDTO>> postVestimenta([FromForm] VestVestimentaDTO vestimenta)
        {
            try
            {                
                var checkVestimenta = await _vestimenta.getNomeVestimenta(vestimenta);

                if (checkVestimenta != null)
                {
                    return Ok(new { message = "Vestimenta inserida com sucesso!!!", result = true, data = checkVestimenta });
                }
                else
                {
                    return BadRequest(new { message = "Erro ao inserir vestimenta", result = false });
                }
            }
            catch (System.Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Ativa vestimenta
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Authorize]
        [HttpPut("ativaVestimenta/{id}")]
        public async Task<IActionResult> ativaVestimenta(int id)
        {
            try
            {
                var checkVestimenta = await _vestimenta.getVestimenta(id);

                if (checkVestimenta != null)
                {
                    return Ok(new { message = "Vestimenta ativada com sucesso!!!", result = true });                    
                }
                else
                {
                    return BadRequest(new { message = "Erro ao ativar vestimenta", result = false });
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Atualiza Vestimenta
        /// </summary>
        /// <param name="vestimenta"></param>
        /// <returns></returns>
        [Authorize]
        [HttpPut("{id}")]
        public async Task<IActionResult> putVestimenta([FromForm] VestVestimentaDTO vestimenta)
        {
            try
            {
                var atualizaVestimenta = await _vestimenta.Update(vestimenta);

                if (atualizaVestimenta != null)
                {
                    return Ok(new { message = "Vestimenta atualizada com sucesso!!!", result = true, data = atualizaVestimenta });
                }
                else
                {
                    return BadRequest(new { message = "Verifique se não há tamanhos vinculados no sistema", result = false });
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Desativa vestimenta
        /// </summary>
        /// <param name="id"></param>
        /// <param name="status"></param>
        /// <returns></returns>
        [Authorize]
        [HttpDelete("{id}/{status}")]
        public async Task<IActionResult> desativaVestimenta(int id, int status)
        {
            try
            {
                var desativaVes = await _vestimenta.desativaVestimenta(id, status);

                if (desativaVes != null)
                {
                    return Ok(new { message = "Vestimenta desativada com sucesso!!!", result = true, data = desativaVes });
                }
                else
                {
                    return BadRequest(new { message = "Erro ao desativar vestimenta", result = false });
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Lista todos as vestimentas
        /// </summary>
        /// <returns></returns>
        [Authorize]
        [HttpGet]
        public async Task<IActionResult> getVestimentas()
        {
            try
            {
                var vestimenta = await _vestimenta.getVestimentas();

                if (vestimenta != null)
                {
                    return Ok(new { message = "lista encontrada", result = true, lista = vestimenta });
                }
                else
                {
                    return BadRequest(new { message = "Nenhuma vestimenta encontrada!!!", result = false });
                }                
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Seleciona uma vestimenta
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Authorize]
        [HttpGet("{id}")]
        public async Task<IActionResult> getVestimenta(int id)
        {
            try
            {                
                var vestimenta = await _vestimenta.getVestimenta(id);

                if (vestimenta != null)
                {
                    return Ok(new { message = "Vestimenta encontrada!!!", result = true, data = vestimenta });
                }
                else
                {
                    return BadRequest(new { message = "Vestimenta não encontrada", result = false });
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
