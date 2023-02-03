﻿using ControleEPI.BLL.EPICategorias;
using ControleEPI.BLL.EPITamanhos;
using ControleEPI.DTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ApiSMT.Controllers.ControllersEPI
{
    /// <summary>
    /// Classe ControllerTamanhos
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class ControllerTamanhos : ControllerBase
    {
        private readonly IEPITamanhosBLL _tamanhos;
        private readonly IEPICategoriasBLL _categoria;

        /// <summary>
        /// Construtor ControllerTamanhos
        /// </summary>
        /// <param name="tamanhos"></param>
        /// <param name="categoria"></param>
        public ControllerTamanhos(IEPITamanhosBLL tamanhos, IEPICategoriasBLL categoria)
        {
            _tamanhos = tamanhos;
            _categoria = categoria;
        }

        /// <summary>
        /// Inserir novo tamanho de algum EPI
        /// </summary>
        /// <param name="tamanho"></param>
        /// <returns></returns>
        [Authorize]
        [HttpPost]
        public async Task<IActionResult> insereTamanho([FromBody] EPITamanhosDTO tamanho)
        {
            try
            {
                var insereTamanho = await _tamanhos.insereTamanho(tamanho);

                if (insereTamanho != null)
                {
                    return Ok(new { message = "Tamanho inserido com sucesso!!!", result = true });
                }
                else
                {
                    return BadRequest(new { message = "Erro ao inserir tamanho", result = false});
                }
            }
            catch (System.Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Ativa ou desativa produto
        /// </summary>
        /// <returns></returns>
        [Authorize]
        [HttpPut("status/{status}/{id}")]
        public async Task<IActionResult> ativaDesativaTamanho(string status, int id)
        {
            try
            {
                var localizaTamanho = await _tamanhos.localizaTamanho(id);

                if (localizaTamanho != null)
                {
                    localizaTamanho.ativo = status;

                    await _tamanhos.Update(localizaTamanho);

                    if (status == "S")
                    {
                        return Ok(new { message = "Tamanho ativado com sucesso!!!", result = true });
                    }
                    else
                    {
                        return Ok(new { message = "Tamanho desativado com sucesso!!!", result = true });
                    }
                }
                else
                {
                    return BadRequest(new { message = "Fornecedor não encontrado", result = false });
                }
            }
            catch (System.Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Atualizat tamanho
        /// </summary>
        /// <param name="tamanho"></param>
        /// <returns></returns>
        [Authorize]
        [HttpPut]
        public async Task<IActionResult> atualizaTamanho([FromBody] EPITamanhosDTO tamanho)
        {
            try
            {
                var localizaTamanho = await _tamanhos.localizaTamanho(tamanho.id);

                if (localizaTamanho != null)
                {
                    var verificaTamanho = await _tamanhos.verificaTamanho(tamanho.tamanho);

                    if (verificaTamanho == null)
                    {
                        localizaTamanho.tamanho = tamanho.tamanho;

                        await _tamanhos.Update(localizaTamanho);

                        return Ok(new { message = "Tamanho '" + tamanho.tamanho + "' atualizado com sucesso!!!", result = true });
                    }
                    else
                    {
                        return BadRequest(new { message = "Já existe esse tamanho cadastrado", result = false });
                    }
                }
                else
                {
                    return BadRequest(new { message = "Tamanho não encontrado", result = false });
                }
            }
            catch (System.Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Localiza um tamanho
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Authorize]
        [HttpGet("{id}")]
        public async Task<IActionResult> localizaTamanho(int id)
        {
            try
            {
                var localizaTamanho = await _tamanhos.localizaTamanho(id);

                if (localizaTamanho != null)
                {
                    List<object> tamanhos = new List<object>();

                    var localizaCategoria = await _categoria.getCategoria(localizaTamanho.idCategoriaProduto);

                    tamanhos.Add(new
                    {
                        localizaTamanho.id,
                        localizaTamanho.tamanho,
                        localizaTamanho.idCategoriaProduto,
                        localizaCategoria.nome,
                        localizaTamanho.ativo
                    });

                    return Ok(new { message = "Tamanho encontrado", result = true, data = tamanhos });
                }
                else
                {
                    return BadRequest(new { message = "Tamanho não encontrado", result = false });
                }
            }
            catch (System.Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Localiza todos os tamanhos
        /// </summary>
        /// <returns></returns>
        [Authorize]
        [HttpGet]
        public async Task<IActionResult> localizaTamanhos()
        {
            try
            {
                var localizaTamanhos = await _tamanhos.localizaTamanhos();

                if (localizaTamanhos != null)
                {
                    List<object> tamanhos = new List<object>();

                    foreach (var item in localizaTamanhos)
                    {
                        var localizaCategoria = await _categoria.getCategoria(item.idCategoriaProduto);

                        tamanhos.Add(new
                        {
                            item.id,
                            item.tamanho,
                            item.idCategoriaProduto,
                            localizaCategoria.nome,
                            item.ativo
                        });
                    }

                    return Ok(new { message = "Tamanhos encontrados", result = true, data = tamanhos });
                }
                else
                {
                    return BadRequest(new { message = "Nenhum tamanho encontrado", result = false });
                }
            }
            catch (System.Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Tamanhos por categoria
        /// </summary>
        /// <param name="idCategoria"></param>
        /// <returns></returns>
        [Authorize]
        [HttpGet("categoria/{idCategoria}")]
        public async Task<IActionResult> tamanhosCategoria(int idCategoria)
        {
            try
            {
                var localizaTamanhosCategoria = await _tamanhos.tamanhosCategoria(idCategoria);

                if (localizaTamanhosCategoria != null)
                {
                    return Ok(new { message = "Tamanhos encontrados", result = true, data = localizaTamanhosCategoria});
                }
                else
                {
                    return BadRequest(new { message = "Nenhum tamanho encontrado", result = false });
                }
            }
            catch (System.Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Deleta tamanho
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Authorize]
        [HttpDelete("{id}")]
        public async Task<IActionResult> deleteStatus(int id)
        {
            var deletaTamanho = await _tamanhos.localizaTamanho(id);

            if (deletaTamanho == null)
                return BadRequest(new { message = "Tamanho não encontrato", result = false });

            await _tamanhos.Delete(deletaTamanho.id);
            return Ok(new { message = "Tamanho deletado com sucesso!!!", result = true });
        }
    }
}
