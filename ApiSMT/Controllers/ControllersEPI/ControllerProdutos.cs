using System;
using ControleEPI.DTO;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using System.Collections.Generic;
using ControleEPI.BLL.EPICategorias;
using ControleEPI.BLL.EPIProdutos;
using ControleEPI.BLL.EPICertificados;

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
        private readonly IEPICategoriasBLL _categoria;

        /// <summary>
        /// Contrutor ControllerProdutos
        /// </summary>
        /// <param name="produtos"></param>
        /// <param name="certificado"></param>
        /// <param name="categoria"></param>
        public ControllerProdutos(IEPIProdutosBLL produtos, IEPICertificadoAprovacaoBLL certificado, IEPICategoriasBLL categoria)
        {
            _produtos = produtos;
            _certificado = certificado;
            _categoria = categoria;
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
                    return BadRequest(new { message = "Ja existe um produto chamado '" + produto.nome + "'", result = false });
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
                var localizaProduto = await _produtos.localizaProduto(produto.id);

                if (localizaProduto != null)
                {
                    if (localizaProduto.nome == produto.nome)
                    {
                        await _produtos.Update(produto);

                        return Ok(new { message = "Produtor atualizado com sucesso!!!", result = true });
                    }
                    else
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
        [Authorize]
        [HttpGet("{id}")]
        public async Task<IActionResult> localizaProduto(int id)
        {
            try
            {
                var localizaProduto = await _produtos.ativaDesativaProduto(id);

                if (localizaProduto != null)
                {
                    List<object> produtoRetorno = new List<object>();

                    var verificaCategoria = await _categoria.getCategoria(localizaProduto.idCategoria);
                    var verificaCertificado = await _certificado.getCertificado(localizaProduto.idCertificadoAprovacao);

                    produtoRetorno.Add(new
                    {
                        idProduto = localizaProduto.id,
                        idCategoria = verificaCategoria.id,
                        idCertificado = verificaCertificado.id,
                        produto = localizaProduto.nome,
                        categoria = verificaCategoria.nome,
                        certificado = verificaCertificado.numero,
                        ativo = localizaProduto.ativo,
                        foto = localizaProduto.foto,
                        validadeEmUso = localizaProduto.validadeEmUso,
                        preco = localizaProduto.preco
                    });

                    return Ok(new { message = "Produto encontrado", result = true, data = produtoRetorno });
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
        /// Lista produtos por status
        /// </summary>
        /// <param name="status"></param>
        /// <returns></returns>
        [Authorize]
        [HttpGet("todosStatus/{status}")]
        public async Task<IActionResult> produtosStatus(string status)
        {
            try
            {
                var todosProdutos = await _produtos.produtosStatus(status);

                if (todosProdutos != null || !todosProdutos.Equals(0))
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
        [Authorize]
        [HttpGet]
        public async Task<IActionResult> localizaProdutos()
        {
            try
            {
                var localizaProdutos = await _produtos.getProdutosSolicitacao();

                if (localizaProdutos != null || !localizaProdutos.Equals(0))
                {
                    List<object> produtoRetorno = new List<object>();

                    foreach (var item in localizaProdutos)
                    {
                        var verificaCategoria = await _categoria.getCategoria(item.idCategoria);
                        var verificaCertificado = await _certificado.getCertificado(item.idCertificadoAprovacao);

                        produtoRetorno.Add(new
                        {
                            idProduto = item.id,
                            idCategoria = verificaCategoria.id,
                            idCertificado = verificaCertificado.id,
                            produto = item.nome,
                            categoria = verificaCategoria.nome,
                            certificado = verificaCertificado.numero,
                            ativo = item.ativo,
                            foto = item.foto,
                            validadeEmUso = item.validadeEmUso,
                            preco = item.preco
                        });
                    }

                    return Ok(new { message = "Produtos encontrados", result = true, data = produtoRetorno });
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
