using ControleEPI.BLL;
using ControleEPI.DTO;
using Microsoft.AspNetCore.Mvc;
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

        /// <summary>
        /// Construtor ControllerTamanhos
        /// </summary>
        /// <param name="tamanhos"></param>
        public ControllerTamanhos(IEPITamanhosBLL tamanhos)
        {
            _tamanhos = tamanhos;
        }

        /// <summary>
        /// Inserir novo tamanho de algum EPI
        /// </summary>
        /// <param name="tamanho"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> insereTamanho([FromBody] EPITamanhosDTO tamanho)
        {
            try
             {
                if (tamanho != null)
                {
                    var verificaTamanho = await _tamanhos.verificaTamanho(tamanho.tamanho);

                    if (verificaTamanho == null)
                    {
                        var insereTamanho = await _tamanhos.insereTamanho(tamanho);

                        if (insereTamanho != null)
                        {
                            return Ok(new { message = "Tamanho '" + tamanho.tamanho + "' inserido com sucesso!!!", result = true });
                        }
                        else
                        {
                            return BadRequest(new { message = "Erro ao inserir novo tamanho", result = false });
                        }
                    }
                    else
                    {
                        return BadRequest(new { message = "Já existe esse tamanho cadastrado no sistema", result = false });
                    }
                }
                else
                {
                    return BadRequest(new { message = "Nenhum tamanho enviado para inserir", result = false });
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
        [HttpPut("status/{status}/{id}")]
        public async Task<IActionResult> ativaDesativaProduto(string status, int id)
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
        [HttpGet("{id}")]
        public async Task<IActionResult> localizaTamanho(int id)
        {
            try
            {
                var localizaTamanho = await _tamanhos.localizaTamanho(id);

                if (localizaTamanho != null)
                {
                    return Ok(new { message = "Tamanho encontrado", result = true, data = localizaTamanho });
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
        [HttpGet]
        public async Task<IActionResult> localizaTamanhos()
        {
            try
            {
                var localizaTamanhos = await _tamanhos.localizaTamanhos();

                if (localizaTamanhos != null)
                {
                    return Ok(new { message = "Tamanhos encontrados", result = true, data = localizaTamanhos });
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
        [HttpDelete]
        public async Task<ActionResult> deleteStatus(int id)
        {
            var deletaTamanho = await _tamanhos.localizaTamanho(id);

            if (deletaTamanho == null)
                return BadRequest(new { message = "Tamanho não encontrato", data = false });

            await _tamanhos.Delete(deletaTamanho.id);
            return Ok(new { message = "Tamanho deletado com sucesso!!!", data = true });
        }
    }
}
