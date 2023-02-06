using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using System;
using Microsoft.AspNetCore.Authorization;
using ControleEPI.BLL.EPIVinculos;
using System.Collections.Generic;
using ControleEPI.DTO;

namespace ApiSMT.Controllers.ControllersEPI
{
    /// <summary>
    /// Classe ControllerVinculo
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class ControllerVinculo : ControllerBase
    {
        private readonly IEPIVinculoBLL _vinculo;

        /// <summary>
        /// Construtor ControllerVinculo
        /// </summary>
        /// <param name="vinculo"></param>
        public ControllerVinculo(IEPIVinculoBLL vinculo)
        {
            _vinculo = vinculo;
        }

        /// <summary>
        /// Vincular item(s) com colaborador
        /// </summary>
        /// <param name="vinculos"></param>
        /// <param name="idUsuario"></param>
        /// <param name="senha"></param>
        /// <returns></returns>
        [Authorize]
        [HttpPut("vincular/{idUsuario}/{senha}")]
        public async Task<IActionResult> vincularItem([FromBody] List<EPIVinculoDTO> vinculos, int idUsuario, string senha)
        {
            try
            {
                var vinculaItem = await _vinculo.vincularItem(vinculos, idUsuario, senha);

                if (vinculaItem != null)
                {
                    return Ok(new { message = "Retirada de itens realizada com sucesso!!!", result = true, data = vinculaItem });
                }
                else
                {
                    return BadRequest(new { message = "Erro ao vincular itens com colaborador", result = false });
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Devolver item
        /// </summary>
        /// <param name="idVinculo"></param>
        /// <returns></returns>
        [Authorize]
        [HttpPut("devolverItem/{idVinculo}")]
        public async Task<IActionResult> devolverItem(int idVinculo)
        {
            try
            {
                var devolveItem = await _vinculo.devolverItem(idVinculo);

                if (devolveItem != null)
                {
                    return Ok(new { message = "Item devolvido com sucesso!!!", result = true, data = devolveItem });
                }
                else
                {
                    return BadRequest(new { message = "Errro ao devolver item", result = false });
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Lista vinculos usuario por status
        /// </summary>
        /// <returns></returns>
        [Authorize]
        [HttpGet("usuarioStatus/{idUsuario}/{idStatus}")]
        public async Task<IActionResult> vinculoUsuarioStatus(int idUsuario, int idStatus)
        {
            try
            {
                var listaVinculos = await _vinculo.vinculoUsuarioStatus(idUsuario, idStatus);

                if (listaVinculos != null)
                {
                    return Ok(new { message = "Vinculos encontrados", result = true, data = listaVinculos });
                }
                else
                {
                    return BadRequest(new { message = "Nenhum vinculo encontrado!!!", result = false });
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Lista itens vinculados
        /// </summary>
        /// <param name="status"></param>
        /// <returns></returns>
        [Authorize]
        [HttpGet("{status}")]
        public async Task<IActionResult> listaVinculosStatus(int status)
        {
            try
            {
                var localizaVinculos = await _vinculo.localizaVinculoStatus(status);

                if (localizaVinculos != null)
                {
                    return Ok(new { message = "Lista encontrada", result = true, lista = localizaVinculos });
                }
                else
                {
                    return BadRequest(new { message = "Nenhum vinculo encontrado com esse status", result = false });
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Lista itens vinculados
        /// </summary>
        /// <param name="idUsuario"></param>
        /// <returns></returns>
        [Authorize]
        [HttpGet("usuario/{idUsuario}")]
        public async Task<IActionResult> listaVinculosUsuarios(int idUsuario)
        {
            try
            {
                var localizaVinculos = await _vinculo.localizaVinculoUsuario(idUsuario);

                if (localizaVinculos != null)
                {
                    return Ok(new { message = "Itens encontrados!!!", result = true, data = localizaVinculos });
                }
                else
                {
                    return BadRequest(new { message = "Nenhum vinculo encontrado com esse usuário", result = false });
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
