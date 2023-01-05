using System;
using ControleEPI.BLL;
using ControleEPI.DTO;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace ApiSMT.Controllers.ControllersEPI
{
    /// <summary>
    /// Classe ControllerProdutos
    /// </summary>
    [Route("api/[controller]")]
    public class ControllerProdutos : ControllerBase
    {
        private readonly IEPIProdutosBLL _produtos;
        private readonly IEPICertificadoAprovacaoBLL _certificado;

        /// <summary>
        /// Contrutor ControllerProdutos
        /// </summary>
        /// <param name="produtos"></param>
        /// <param name="certificado"></param>
        public ControllerProdutos(IEPIProdutosBLL produtos, IEPICertificadoAprovacaoBLL certificado)
        {
            _produtos = produtos;
            _certificado = certificado;
        }

        /// <summary>
        /// Insere novo produto no estoque
        /// </summary>
        /// <param name="produto"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> insereProduto([FromBody] EPIProdutosDTO produto)
        {
            try
            {
                var localizaProduto = await _produtos.getNomeProduto(produto.nome);

                if (localizaProduto == null)
                {
                    produto.ativo = "S";

                    var insereProduto = await _produtos.Insert(produto);

                    if (insereProduto != null)
                    {
                        return Ok(new { message = "Produto inserido com sucesso!!!", result = true });
                    }
                    else
                    {
                        return BadRequest(new { message = "Erro ao inserir produto", result = false });
                    }
                }
                else
                {
                    return BadRequest(new { message = "Ja existe um produto chamado '"+produto.nome+"'", result = false });
                }
                               
            }
            catch (System.Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Atualiza informações do produto com exceção da quantidade em estoque
        /// </summary>
        /// <param name="produto"></param>
        /// <returns></returns>
        [HttpPut]
        public async Task<IActionResult> atualizaProduto([FromBody] EPIProdutosDTO produto)
        {
            try
            {
                var localizaProduto = await _produtos.getProduto(produto.id);

                if (localizaProduto != null)
                {
                    var verificaNomeProduto = await _produtos.getNomeProduto(produto.nome);

                    if (verificaNomeProduto == null)
                    {
                        await _produtos.Update(produto);

                        return Ok(new { message = "Produtor atualizado com sucesso!!!", result = true });                        
                    }
                    else
                    {
                        return BadRequest(new { message = "Ja existe um produto chamado '" + produto.nome + "'", result = false });
                    }
                }
                else
                {
                    return BadRequest(new { message = "Produto não encontrado", result = false });
                }
            }
            catch (System.Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Localiza produto
        /// </summary>
        /// <returns></returns>
        [HttpGet("{id}")]
        public async Task<IActionResult> localizaProduto(int id)
        {
            try
            {
                var localizaProduto = await _produtos.ativaDesativaProduto(id);

                if (localizaProduto != null)
                {
                    return Ok(new { message = "Produto encontrado", result = true, data = localizaProduto });
                }
                else
                {
                    return BadRequest(new { message = "Produto não encontrado", result = false });
                }
            }
            catch (Exception ex)
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
                var localizaProduto = await _produtos.ativaDesativaProduto(id);

                if (localizaProduto != null)
                {                    
                    localizaProduto.ativo = status;
                 
                    await _produtos.Update(localizaProduto);

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
        /// Localiza todos os produtos
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> localizaProdutos()
        {
            try
            {
                var localizaProdutos = await _produtos.getProdutosSolicitacao();

                if (localizaProdutos != null || !localizaProdutos.Equals(0))
                {
                    return Ok(new { message = "Produtos encontrados", result = true, lista = localizaProdutos });
                }
                else
                {
                    return BadRequest(new { message = "Nenhum produto encontrado", result = false });
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
