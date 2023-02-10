using System;
using ControleEPI.DTO;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using ControleEPI.BLL.EPIProdutos;

namespace ApiSMT.Controllers.ControllersEPI
{
    /// <summary>
    /// Classe ControllerProdutos
    /// </summary>
    [Route("api/[controller]")]
    public class ControllerProdutos : ControllerBase
    {
        private readonly IEPIProdutosBLL _produtos;

        /// <summary>
        /// Contrutor ControllerProdutos
        /// </summary>
        /// <param name="produtos"></param>
        public ControllerProdutos(IEPIProdutosBLL produtos)
        {
            _produtos = produtos;
        }

        /// <summary>
        /// Insere novo produto no estoque
        /// </summary>
        /// <param name="produto"></param>
        /// <returns></returns>
        [Authorize]
        [HttpPost]
        public async Task<IActionResult> insereProduto([FromBody] EPIProdutosDTO produto)
        {
            try
            {
                var insereProduto = await _produtos.Insert(produto);

                if (insereProduto != null)
                {
                    return Ok(new { message = "Produto inserido com sucesso!!!", result = true });
                }
                else
                {
                    return BadRequest(new { message = "Erro ao inserir produtro", result = false });
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
        [Authorize]
        [HttpPut]
        public async Task<IActionResult> atualizaProduto([FromBody] EPIProdutosDTO produto)
        {
            try
            {
                var atualizaProduto = await _produtos.Update(produto);

                if (atualizaProduto != null)
                {
                    return Ok(new { message = "Produto atualizado com sucesso!!!", result = true });
                }
                else
                {
                    return BadRequest(new { message = "Erro ao atualizar produto", result = false });
                }
            }
            catch (System.Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Lista produtos por categoria
        /// </summary>
        /// <param name="idCategoria"></param>
        /// <returns></returns>
        [Authorize]
        [HttpGet("categoria/{idCategoria}")]
        public async Task<IActionResult> listaPorCategoria(int idCategoria)
        {
            try
            {
                var produtosCategoria = await _produtos.verificaCategorias(idCategoria);

                if (produtosCategoria != null)
                {
                    return Ok(new { message = "Produtos encontrados", result = true, data = produtosCategoria });
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

        /// <summary>
        /// Localiza produto
        /// </summary>
        /// <returns></returns>
        [Authorize]
        [HttpGet("{id}")]
        public async Task<IActionResult> localizaProduto(int id)
        {
            try
            {
                var localizaProduto = await _produtos.localizaProduto(id);

                if (localizaProduto != null)
                {
                    return Ok(new { message = "Produtos encontrados", result = true, data = localizaProduto });
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

        /// <summary>
        /// Lista produtos por status
        /// </summary>
        /// <param name="status"></param>
        /// <returns></returns>
        [Authorize]
        [HttpGet("todosStatus/{status}")]
        public async Task<IActionResult> produtosTamanhos(string status)
        {
            try
            {
                var todosProdutos = await _produtos.produtosTamanhos(status);

                if (todosProdutos != null)
                {
                    return Ok(new { message = "Produtos encontrados", result = true, data = todosProdutos });
                }
                else
                {
                    return BadRequest(new { message = "Produtos não encontrados", result = false });
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
        [Authorize]
        [HttpPut("status/{status}/{id}")]
        public async Task<IActionResult> ativaDesativaProduto(string status, int id)
        {
            try
            {
                var ativaDesativaProduto = await _produtos.ativaDesativaProduto(status, id);

                if (ativaDesativaProduto != null)
                {
                    return Ok(new { message = "Produto atualizado com sucesso!!!", result = true });
                }
                else
                {
                    return BadRequest(new { message = "Erro ao atualizar status do produto", result = false });
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
        [Authorize]
        [HttpGet]
        public async Task<IActionResult> localizaProdutos()
        {
            try
            {
                var localizaProdutos = await _produtos.getProdutosSolicitacao();

                if (localizaProdutos != null)
                {
                    return Ok(new { message = "Produtos encontrados", result = true, data = localizaProdutos });
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

        /// <summary>
        /// Localizar tamanhos de um produto
        /// </summary>
        /// <param name="idProduto"></param>
        /// <returns></returns>
        [Authorize]
        [HttpGet("produtosTamanhos/{idProduto}")]
        public async Task<IActionResult> localizaProdutoTamanhos(int idProduto)
        {
            try
            {
                var localizarProdutoTamanhos = await _produtos.localizaProdutoTamanhos(idProduto);

                if (localizarProdutoTamanhos != null)
                {
                    return Ok(new { message = "Tamanhos de produto encontrados!!!", result = true, data = localizarProdutoTamanhos });
                }
                else
                {
                    return BadRequest(new { message = "Nenhum tamanho encontrado", result = false });
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
