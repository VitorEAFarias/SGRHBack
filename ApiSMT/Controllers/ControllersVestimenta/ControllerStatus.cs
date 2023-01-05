using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Vestimenta.BLL;
using Vestimenta.DTO;
using System;
using Microsoft.AspNetCore.Authorization;

namespace ApiSMT.Controllers.ControllersVestimenta
{
    /// <summary>
    /// Classe de Status
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class ControllerStatus : ControllerBase
    {
        private readonly IStatusVestBLL _statusVest;

        /// <summary>
        /// Construtor VestimentaController
        /// </summary>
        /// <param name="statusVest"></param>
        public ControllerStatus(IStatusVestBLL statusVest)
        {
            _statusVest = statusVest;
        }

        /// <summary>
        /// Insere um novo status
        /// </summary>
        /// <param name="status"></param>
        /// <returns></returns>
        [Authorize]
        [HttpPost]
        public async Task<ActionResult<VestStatusDTO>> postStatus([FromBody] VestStatusDTO status)
        {
            try
            {
                if (!string.IsNullOrEmpty(Convert.ToString(status)))
                {
                    var checkStatus = await _statusVest.getNomeStatus(status.nome);
                    if (checkStatus != null)
                    {
                        return BadRequest(new { message = "Ja existe um status chamado: " + status.nome, result = false });
                    }
                    else
                    {
                        var novoStatus = await _statusVest.Insert(status);
                        return Ok(new { message = "Status inserido com sucesso!!!", result = true, data = novoStatus });
                    }
                }
                else
                {
                    return BadRequest(new { message = "Erro ao inserir status " + status.nome, result = false });
                }
            }
            catch (System.Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Atualiza Vestimenta
        /// </summary>
        /// <param name="status"></param>
        /// <returns></returns>
        [Authorize]
        [HttpPut("{id}")]
        public async Task<ActionResult> putStatus([FromBody] VestStatusDTO status)
        {
            try
            {
                if (!string.IsNullOrEmpty(Convert.ToString(status)))
                {
                    await _statusVest.Update(status);

                    return Ok(new { message = status.nome + " Atualizado com sucesso!!!", result = true });
                }
                else
                {
                    return BadRequest(new { message = "Nenhum status encontrado!!!", result = false });
                }
            }
            catch (System.Exception ex)
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
        public async Task<ActionResult> getTodosStatus()
        {
            try
            {
                var status = await _statusVest.getTodosStatus();

                return Ok(new { message = "lista encontrada", result = true, lista = status });
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
        public async Task<ActionResult<VestVestimentaDTO>> getStatus(int id)
        {
            try
            {
                if (id != 0)
                {
                    var status = await _statusVest.getStatus(id);

                    return Ok(new { message = "Status encontrado", motivo = status.nome, result = true });
                }
                else
                {
                    return BadRequest(new { message = "Status não encontrado", result = false });
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Deleta um status selecionado
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Authorize]
        [HttpDelete]
        public async Task<ActionResult> deleteStatus(int id)
        {
            var deletaStatus = await _statusVest.getStatus(id);

            if (deletaStatus == null)
                return BadRequest(new { message = "Status não encontrato", data = false });

            await _statusVest.Delete(deletaStatus.id);
            return Ok(new { message = "Status deletado com sucesso!!!", data = true });
        }

        /// <summary>
        /// teste
        /// </summary>
        /// <param name="teste"></param>
        /// <returns></returns>
        [HttpGet("teste/{teste}")]
        public IActionResult teste(string teste)
        {
            string mensagem = "Ta funcionando";

            return Ok(mensagem);
        }
    }
}
