using System;
using ControleEPI.DTO;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using ControleEPI.BLL.EPIProdutosEstoque;

namespace ApiSMT.Controllers.ControllersEPI
{
    /// <summary>
    /// Classe que manipula as informações de produtos e seu estoque
    /// </summary>
    [Route("api/[controller]")]
    public class ControllerProdutosEstoque : ControllerBase
    {
        private readonly IEPIProdutosEstoqueBLL _produtosEstoque;

        /// <summary>
        /// Construtor ProdutosEstoqueController
        /// </summary>
        /// <param name="produtosEstoque"></param>
        public ControllerProdutosEstoque(IEPIProdutosEstoqueBLL produtosEstoque)
        {
            _produtosEstoque = produtosEstoque;
        }

        /// <summary>
        /// Insere produtos no estoque
        /// </summary>
        /// <param name="produto"></param>
        /// <returns></returns>
        [Authorize]
        [HttpPost]
        public async Task<IActionResult> insereEstoque([FromBody] EPIProdutosEstoqueDTO produto)
        {
            try
            {                   
                var insereEstoque = await _produtosEstoque.Insert(produto);

                if (insereEstoque != null)
                {
                    return Ok(new { message = "Cadastrado em estoque inserido com sucesso!!!", result = true });
                }
                else
                {
                    return BadRequest(new { message = "Erro ao cadastrar produto no estoque", result = false });
                }               
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Aatualiza quantidade disponivel em estoque
        /// </summary>
        /// <returns></returns>
        [Authorize]
        [HttpPut("estoque")]
        public async Task<IActionResult> atualizaEstoque([FromBody] EPIProdutosEstoqueDTO estoque)
        {
            try
            {
                var atualizaEstoque = await _produtosEstoque.Update(estoque);

                if (atualizaEstoque != null)
                {
                    return Ok(new { message = "Estoque atualizado com sucesso", result = true });
                }
                else
                {
                    return BadRequest(new { message = "Erro ao atualizar estoque", result = false });
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }        

        /// <summary>
        /// Lista todos os produtos cadastrados no estoque
        /// </summary>
        /// <returns></returns>
        [Authorize]
        [HttpGet]
        public async Task<IActionResult> listaTodosProdutosEmEstoque()
        {
            try
            {
                var listaDeProdutosEstoque = await _produtosEstoque.getProdutosEstoque();

                if (listaDeProdutosEstoque != null)
                {
                    return Ok(new { message = "Produtos em estoque encontrados", result = true, data = listaDeProdutosEstoque });
                }
                else
                {
                    return BadRequest(new { message = "Nenhum produto encontrado", result = false });
                }
            }
            catch (System.Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Seleciona um produto
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Authorize]
        [HttpGet("{id}")]
        public async Task<IActionResult> selecionaProduto(int id)
        {
            try
            {
                var localizaProdutoEstoque = await _produtosEstoque.getProdutoEstoque(id);

                if (localizaProdutoEstoque != null)
                {
                    return Ok(new { message = "Produto encontrado", result = true, data = localizaProdutoEstoque });
                }
                else
                {
                    return BadRequest(new { message = "Nenhum produto em estoque encontrado", result = false });
                }
            }
            catch (System.Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
