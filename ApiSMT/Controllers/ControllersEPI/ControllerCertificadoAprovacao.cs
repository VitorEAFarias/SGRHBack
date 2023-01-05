using ControleEPI.BLL;
using ControleEPI.DTO;
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
        [HttpPost]
        public async Task<IActionResult> insereCertificadoAprovacao([FromBody] EPICertificadoAprovacaoDTO certificado)
        {
            try
            {
                EPICertificadoAprovacaoDTO verificaValorCertificado = await _certificado.getValorCertificado(certificado.numero);

                if (verificaValorCertificado == null)
                {
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
        /// Atualiza certificado EPI
        /// </summary>
        /// <returns></returns>
        [HttpPut]
        public async Task<IActionResult> atualizaCertificado([FromBody] EPICertificadoAprovacaoDTO certificado)
        {
            try
            {
                EPICertificadoAprovacaoDTO localizaCertificado = await _certificado.getCertificado(certificado.id);

                if (localizaCertificado != null)
                {
                    EPIProdutosDTO localizaProduto = await _produtos.getCertificadoProduto(localizaCertificado.id);

                    if (localizaProduto != null)
                    {
                        EPICertificadoAprovacaoDTO verificaCertificado = await _certificado.getValorCertificado(certificado.numero);

                        if (verificaCertificado == null)
                        {
                            localizaCertificado.numero = certificado.numero;
                            localizaCertificado.validade = certificado.validade;

                            await _certificado.Update(localizaCertificado);

                            return Ok(new { message = "Certificado atualizado para o produto '" + localizaProduto.nome + "'", result = true, data = certificado.numero });
                        }
                        else
                        {
                            return BadRequest(new { message = "Ja existe um certificado cadastrado com esse valor", result = false });
                        }
                    }
                    else
                    {
                        return BadRequest(new { message = "Nenhum produto atribuido com esse certificado", result = false });
                    }
                }
                else
                {
                    return BadRequest(new { message = "Certificado de autorização não encontrado", result = false });
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
        public async Task<IActionResult> ativaDesativaCertificado(string status, int id)
        {
            try
            {
                var localizaCertificado = await _certificado.getCertificado(id);

                if (localizaCertificado != null)
                {
                    localizaCertificado.ativo = status;

                    await _certificado.Update(localizaCertificado);

                    if (status == "S")
                    {
                        return Ok(new { message = "Certificado ativado com sucesso!!!", result = true });
                    }
                    else
                    {
                        return Ok(new { message = "Certificado desativado com sucesso!!!", result = true });
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
