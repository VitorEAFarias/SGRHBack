using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Vestimenta.DTO;
using System;
using Microsoft.AspNetCore.Authorization;
using System.Collections.Generic;
using Vestimenta.BLL.VestEstoque;

namespace ApiSMT.Controllers.ControllersVestimenta
{
    /// <summary>
    /// Classe EstoqueController
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class ControllerEstoque : ControllerBase
    {
        private readonly IVestEstoqueBLL _estoque;

        /// <summary>
        /// Construtor EstoqueController
        /// </summary>
        /// <param name="estoque"></param>
        public ControllerEstoque(IVestEstoqueBLL estoque)
        {
            _estoque = estoque;
        }

        /// <summary>
        /// Atualiza estoque
        /// </summary>
        /// <param name="estoque"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        [Authorize]
        [HttpPut("{id}")]        
        public async Task<IActionResult> putEstoque(int id, [FromBody] List<VestEstoqueDTO> estoque)
        {
            try
            {
                var atualizaLogEstoque = await _estoque.atualizaLogEstoque(id, estoque);

                if (atualizaLogEstoque != null)
                {
                    return Ok(new { message = "Estoque atualizado com sucesso!!!", result = true });
                }
                else
                {
                    return BadRequest(new { message = "Erro ao atualizar estoque", result = false });
                }
            }
            catch (System.Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Lista todos os tamanhos do idItem enviado
        /// </summary>
        /// <returns></returns>
        [Authorize]
        [HttpGet("tamanhos/{idItem}")]
        public async Task<IActionResult> getEstoque(int idItem)
        {
            try
            {
                var itens = await _estoque.getItensExistentes(idItem);

                if (itens != null)
                {
                    return Ok(new { message = "Itens encontrados!!!", result = true, data = itens });
                }
                else
                {
                    return BadRequest(new { message = "Nenhum item encontrado", result = false });
                }
                
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Seleciona um item do estoque
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Authorize]
        [HttpGet("{id}")]
        public async Task<IActionResult> getItemStoque(int id)
        {
            try
            {                
                var estoque = await _estoque.getItemEstoque(id);

                if (estoque != null)
                { 
                    return Ok(new { message = "Item do estoque encontrado", data = estoque, result = true });
                }
                else
                {
                    return BadRequest(new { message = "Item do estoque não encontrado", result = false });
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
