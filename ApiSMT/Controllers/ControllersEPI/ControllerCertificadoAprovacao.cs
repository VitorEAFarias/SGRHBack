using ControleEPI.BLL;
using ControleEPI.DTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace ApiSMT.Controllers.ControllersEPI
{
    /// <summary>
    /// Classe ControllerCertificadoAprovacao
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class ControllerCertificadoAprovacao : ControllerBase
    {
        private readonly IEPICertificadoAprovacaoBLL _certificado;
        private readonly IEPIProdutosBLL _produtos;

        /// <summary>
        /// Construtor ControllerCertificadoAprovacao
        /// </summary>
        /// <param name="certificado"></param>
        /// <param name="produtos"></param>
        public ControllerCertificadoAprovacao(IEPICertificadoAprovacaoBLL certificado, IEPIProdutosBLL produtos)
        {
            _certificado = certificado;
            _produtos = produtos;
        }

        /// <summary>
        /// Cadastra novo certificado
        /// </summary>
        /// <param name="certificado"></param>
        /// <returns></returns>
        [Authorize]
        [HttpPost]
        public async Task<IActionResult> insereCertificadoAprovacao([FromBody] EPICertificadoAprovacaoDTO certificado)
        {
            try
            {
                EPICertificadoAprovacaoDTO verificaValorCertificado = await _certificado.getValorCertificado(certificado.numero);

                if (verificaValorCertificado == null)
                {
                    certificado.ativo = "S";
                    certificado.observacao = "";

                    await _certificado.Insert(certificado);

                    return Ok(new { message = "Certificado cadastrado com sucesso", result = true });
                }
                else
                {
                    return BadRequest(new { message = "Ja existe um certificado com esse valor", result = false });
                }
            }
            catch (System.Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Lista certificados ativados
        /// </summary>
        /// <returns></returns>
        [Authorize]
        [HttpGet("ativados/{status}")]
        public async Task<IActionResult> listaAtivados(string status)
        {
            try
            {
                var localizaAtivados = await _certificado.listaStatus(status);

                if (localizaAtivados != null || !localizaAtivados.Equals(0))
                {
                    return Ok(new { message = "Lista encontrada", result = true, data = localizaAtivados });
                }
                else
                {
                    return BadRequest(new { message = "Nenhum certificado enconrtado com os status enviado", result = false });
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
        [HttpPut("status/{status}/{id}/{observacao}")]
        public async Task<IActionResult> ativaDesativaCertificado(string status, int id, string observacao)
        {
            try
            {
                var localizaCertificado = await _certificado.getCertificado(id);

                if (localizaCertificado != null)
                {
                    localizaCertificado.ativo = status;
                    localizaCertificado.observacao = observacao;

                    if (status == "S")
                    {
                        await _certificado.Update(localizaCertificado);

                        return Ok(new { message = "Certificado ativado com sucesso!!!", result = true });
                    }
                    else
                    {
                        var verificaCertificado = await _produtos.getCertificadoProduto(localizaCertificado.id);

                        if (verificaCertificado == null || verificaCertificado.Equals(0))
                        {
                            await _certificado.Update(localizaCertificado);

                            return Ok(new { message = "Certificado desativado com sucesso!!!", result = true });
                        }
                        else
                        {
                            return BadRequest(new { message = "Não é possivel desativar um certificado vinculado a um produto", result = false });
                        }                        
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
        /// Lista certificados e seus respectivos produtos
        /// </summary>
        /// <returns></returns>
        [Authorize]
        [HttpGet]
        public async Task<IActionResult> localizaProdutosCertificado()
        {
            try
            {
                var localizaCertificadosProdutos = await _certificado.getCertificadosNumero();

                if (localizaCertificadosProdutos != null)
                {
                    return Ok(new { message = "Lista encontrada", result = true, data = localizaCertificadosProdutos });
                }
                else
                {
                    return BadRequest(new { message = "Nenhum produto com certificado encontrado", result = false });
                }
            }
            catch (System.Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Localiza o certificado de um produto
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Authorize]
        [HttpGet("{id}")]
        public async Task<IActionResult> localizaProdutoCertificado(int id)
        {
            try
            {
                var localizaCertificadoProduto = await _certificado.getCertificado(id);

                if (localizaCertificadoProduto != null)
                {
                    return Ok(new { message = "Certificado encontrado", result = true, data = localizaCertificadoProduto });
                }
                else
                {
                    return BadRequest(new { message = "Certificado não encontrado", result = false });
                }
            }
            catch (System.Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
