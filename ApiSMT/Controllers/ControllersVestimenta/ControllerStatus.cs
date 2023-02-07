using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Vestimenta.DTO;
using System;
using Microsoft.AspNetCore.Authorization;
using Vestimenta.BLL.VestStatus;

namespace ApiSMT.Controllers.ControllersVestimenta
{
    /// <summary>
    /// Classe de Status
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class ControllerStatus : ControllerBase
    {
        private readonly IVestStatusBLL _statusVest;

        /// <summary>
        /// Construtor VestimentaController
        /// </summary>
        /// <param name="statusVest"></param>
        public ControllerStatus(IVestStatusBLL statusVest)
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
        public async Task<IActionResult> postStatus([FromBody] VestStatusDTO status)
        {
            try
            {
                var novoStatus = await _statusVest.Insert(status);

                if (novoStatus != null)
                {
                    return Ok(new { message = "Novo status inserido com sucesso!!!", result = true, data = novoStatus });
                }
                else
                {
                    return BadRequest(new { message = "Erro ao inserir novo status, verifique se o mesmo ja nao existe", result = false });
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
        public async Task<IActionResult> putStatus([FromBody] VestStatusDTO status)
        {
            try
            {                
                var atualizaStatus = await _statusVest.Update(status);

                if (atualizaStatus != null)
                { 
                    return Ok(new { message = status.nome + " Atualizado com sucesso!!!", result = true });
                }
                else
                {
                    return BadRequest(new { message = "Nenhum status encontrado!!!", result = false });
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
        public async Task<IActionResult> getTodosStatus()
        {
            try
            {
                var status = await _statusVest.getTodosStatus();

                if (status != null)
                {
                    return Ok(new { message = "lista encontrada", result = true, data = status });
                }
                else
                {
                    return BadRequest(new { message = "Nenhum status encontrado", result = false });
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
        public async Task<IActionResult> getStatus(int id)
        {
            try
            {                
                var status = await _statusVest.getStatus(id);

                if (status != null)
                {               
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
        public async Task<IActionResult> deleteStatus(int id)
        {
            try
            {
                var deletaStatus = await _statusVest.Delete(id);

                if (deletaStatus != null)
                {
                    return Ok(new { message = "Status deletado com sucesso!!!", result = true, data = deletaStatus });
                }
                else
                {
                    return BadRequest(new { message = "Erro ao deletar status", result = false });
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// teste
        /// </summary>
        /// <returns></returns>
        [HttpGet("teste")]
        public IActionResult teste()
        {
            string mensagem = "Ta funcionando";

            return Ok(mensagem);
        }
    }
}
