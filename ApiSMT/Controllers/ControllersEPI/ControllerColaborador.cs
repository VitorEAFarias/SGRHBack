using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using ControleEPI.BLL.RHUsuarios;

namespace ApiSMT.Controllers.ControllersEPI
{
    /// <summary>
    /// Classe que manipula os dados de colaborador
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class ControllerColaborador : ControllerBase
    {
        private readonly IRHConUserBLL _usuario;

        /// <summary>
        /// Construtor ColaboradorController
        /// </summary>
        /// <param name="usuario"></param>
        public ControllerColaborador(IRHConUserBLL usuario)
        {
            _usuario = usuario;
        }

        /// <summary>
        /// Lista todos os colaboradores
        /// </summary>
        /// <param name="idSuperior"></param>
        /// <returns></returns>
        [Authorize]
        [HttpGet("superior/{idSuperior}")]
        public async Task<IActionResult> getListColaboradores(int idSuperior)
        {
            try
            {
                if (idSuperior == 0)
                {
                    var colaboradores = await _usuario.GetColaboradores();                    

                    if (colaboradores != null)
                    {
                        return Ok(new { message = "Lista encontrada", data = colaboradores, result = true });
                    }
                    else
                    {
                        return BadRequest(new { message = "Nenhum colaborador encontrado", result = false });
                    }
                }
                else
                {
                    var checkEquipe = await _usuario.getColaboradores(idSuperior);

                    if (checkEquipe.Count != 0)
                    {
                        return Ok(new { message = "Lista encontrada", data = checkEquipe, result = true });
                    }
                    else
                    {
                        var infoEmp = await _usuario.GetEmp(idSuperior);

                        if (infoEmp != null)
                        {
                            return Ok(new { message = "Colaborador encontrado", result = true, data = infoEmp });
                        }
                        else
                        {
                            return BadRequest(new { message = "Colaborador não encontrado", result = false });
                        }                        
                    }
                }
            }
            catch (System.Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Seleciona um colaborador
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Authorize]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetColaborador(int id)
        {
            try
            {
                var colaborador = await _usuario.GetEmp(id);

                if (colaborador != null)
                {
                    return Ok(new { message = "Colaborador encontrado", data = colaborador, result = true });                    
                }
                else
                {
                    return BadRequest(new { message = "Colaborador não encontrado", result = false });
                }
            }
            catch (System.Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
