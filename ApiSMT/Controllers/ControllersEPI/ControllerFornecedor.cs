using ControleEPI.BLL;
using ControleEPI.DTO;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace ApiSMT.Controllers.ControllersEPI
{
    /// <summary>
    /// Classe que manipula os dados de fornecedores
    /// </summary>
    [Route("api/[controller]")]
    public class ControllerFornecedor : ControllerBase
    {
        private readonly IEPIFornecedoresBLL _fornecedor;

        /// <summary>
        /// Construtos de FornecedorController
        /// </summary>
        /// <param name="fornecedor"></param>
        public ControllerFornecedor(IEPIFornecedoresBLL fornecedor)
        {
            _fornecedor = fornecedor;
        }

        /// <summary>
        /// Insere um novo fornecedor
        /// </summary>
        /// <param name="fornecedor"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> insereFornecedor([FromBody] EPIFornecedoresDTO fornecedor)
        {
            try
            {
                var verificaFornecedor = await _fornecedor.verificaFornecedor(fornecedor.nome, fornecedor.cnpj);

                if (verificaFornecedor == null)
                {
                    fornecedor.ativo = "S";

                    var insereFornecedor = await _fornecedor.Insert(fornecedor);

                    if (insereFornecedor != null)
                    {
                        return Ok(new { message = "Fornecedor '" + fornecedor.nome + "' cadastrado com sucesso!!!", result = true });
                    }
                    else
                    {
                        return BadRequest(new { message = "Erro ao cadastrar fornecedor '" + fornecedor.nome + "'", result = false });
                    }
                }
                else
                {
                    return BadRequest(new { message = "Ja existe um fornecedor cadastrado com esses dados", result = false });
                }
            }
            catch (System.Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Atualiza dados do fornecedor
        /// </summary>
        /// <param name="fornecedor"></param>
        /// <returns></returns>
        [HttpPut]
        public async Task<IActionResult> atualizaFornecedor([FromBody] EPIFornecedoresDTO fornecedor)
        {
            try
            {
                var localizaFornecedor = await _fornecedor.getFornecedor(fornecedor.id);

                if (localizaFornecedor != null)
                {
                    await _fornecedor.Update(fornecedor);

                    return Ok(new { message = "Dados do fornecedor '"+localizaFornecedor.nome+"' atualizado com sucesso!!!", result = true });
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
        /// Ativa ou desativa fornecedor
        /// </summary>
        /// <returns></returns>
        [HttpPut("status/{status}/{id}")]
        public async Task<IActionResult> ativaDesativaFornecedor(string status, int id)
        {
            try
            {
                var localizaFornecedor = await _fornecedor.getFornecedor(id);

                if (localizaFornecedor != null)
                {
                    localizaFornecedor.ativo = status;

                    await _fornecedor.Update(localizaFornecedor);

                    if (status == "S")
                    {
                        return Ok(new { message = "Fornecedor ativado com sucesso!!!", result = true });
                    }
                    else
                    {
                        return Ok(new { message = "Fornecedor desativado com sucesso!!!", result = true });
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
        /// Localiza uma fornecedor
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("{id}")]
        public async Task<IActionResult> localizaFornecedor(int id)
        {
            try
            {
                var localizaFornecedor = await _fornecedor.getFornecedor(id);

                if (localizaFornecedor != null)
                {
                    return Ok(new { message = "Fornecedor encontrada!!!", result = true, fornecedor = localizaFornecedor });
                }
                else
                {
                    return BadRequest(new { message = "Fornecedor não encontrada", result = false });
                }
            }
            catch (System.Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Lista fornecedores ativos e desativos
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> getFornecedores()
        {
            try
            {
                var getFornecedores = await _fornecedor.getFornecedores();

                if (getFornecedores != null)
                {
                    return Ok(new { message = "Fornecedores encontrados", result = true, data = getFornecedores });
                }
                else
                {
                    return BadRequest(new { message = "Nenhum fornecedor encontrado", result = false });
                }                
            }
            catch (System.Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
