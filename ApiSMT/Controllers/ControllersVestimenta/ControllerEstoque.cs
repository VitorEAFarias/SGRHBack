using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Vestimenta.BLL;
using Vestimenta.DTO;
using System;
using Microsoft.AspNetCore.Authorization;
using System.Collections.Generic;

namespace ApiSMT.Controllers.ControllersVestimenta
{
    /// <summary>
    /// Classe EstoqueController
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class ControllerEstoque : ControllerBase
    {
        private readonly IEstoqueBLL _estoque;
        private readonly ILogBLL _log;
        private readonly IVestimentaBLL _vestimenta;

        /// <summary>
        /// Construtor EstoqueController
        /// </summary>
        /// <param name="estoque"></param>
        /// <param name="log"></param>
        /// <param name="vestimenta"></param>
        public ControllerEstoque(IEstoqueBLL estoque, ILogBLL log, IVestimentaBLL vestimenta)
        {
            _estoque = estoque;
            _log = log;
            _vestimenta = vestimenta;
        }

        /// <summary>
        /// Atualiza estoque
        /// </summary>
        /// <param name="estoque"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        [Authorize]
        [HttpPut("{id}")]        
        public async Task<ActionResult> putEstoque(int id, [FromBody] List<VestEstoqueDTO> estoque)
        {
            try
            {
                if (estoque != null)
                {
                    foreach (var item in estoque)
                    {
                        var checkEstoque = await _estoque.getItemExistente(item.idItem, item.tamanho);
                                                
                        if (item.quantidadeUsado == checkEstoque.quantidadeUsado)
                        {
                            item.dataAlteracao = DateTime.Now;

                            await _estoque.Update(item);

                            VestLogDTO log = new VestLogDTO();

                            log.data = DateTime.Now;
                            log.idUsuario = id;
                            log.idItem = item.idItem;
                            log.quantidadeAnt = checkEstoque.quantidade;
                            log.quantidadeDep = checkEstoque.quantidade + item.quantidade;
                            log.tamanho = checkEstoque.tamanho;
                            log.usado = "N";

                            await _log.Insert(log);
                        }
                        else
                        {
                            item.dataAlteracao = DateTime.Now;

                            await _estoque.Update(item);

                            VestLogDTO log = new VestLogDTO();

                            log.data = DateTime.Now;
                            log.idUsuario = id;
                            log.idItem = item.idItem;
                            log.quantidadeAnt = checkEstoque.quantidadeUsado;
                            log.quantidadeDep = checkEstoque.quantidadeUsado + item.quantidadeUsado;
                            log.tamanho = checkEstoque.tamanho;
                            log.usado = "N";

                            var insereLog = await _log.Insert(log);                            
                        }                        
                    }

                    return Ok(new { message = "Quantidades atualizadas com êxito!!!", result = true });
                }
                else
                {
                    return BadRequest(new { message = "Nenhuma informação enviada!!!", result = false });
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
        public async Task<ActionResult> getEstoque(int idItem)
        {
            try
            {
                var itens = await _estoque.getItensExistentes(idItem);
                List<object> lista = new List<object>();

                if (itens != null)
                {
                    foreach (var item in itens)
                    {
                        var getNome = await _vestimenta.getVestimenta(item.idItem);

                        lista.Add(new { 
                            id = item.id,
                            idItem = item.idItem,
                            nome = getNome.nome,
                            quantidade = item.quantidade,
                            quantidadeUsado = item.quantidadeUsado
                        });
                    }

                    return Ok(new { message = "Lista encontrada", result = true, lista = lista });
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
        public async Task<ActionResult<VestEstoqueDTO>> getItemStoque(int id)
        {
            try
            {
                if (id != 0)
                {
                    var estoque = await _estoque.getItemEstoque(id);

                    return Ok(new { message = "Item do estoque encontrado", estoque = estoque, result = true });
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
