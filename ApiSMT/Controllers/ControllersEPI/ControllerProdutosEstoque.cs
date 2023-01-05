using System;
using ControleEPI.BLL;
using ControleEPI.DTO;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace ApiSMT.Controllers.ControllersEPI
{
    /// <summary>
    /// Classe que manipula as informações de produtos e seu estoque
    /// </summary>
    [Route("api/[controller]")]
    public class ControllerProdutosEstoque : ControllerBase
    {
        private readonly IEPIProdutosEstoqueBLL _produtosEstoque;
        private readonly IEPIProdutosBLL _produtos;
        private readonly IEPITamanhosBLL _tamanhos;
        private readonly IRHConUserBLL _usuario;
        private readonly IEPILogEstoqueBLL _logEstoque;
        private readonly IEPICertificadoAprovacaoBLL _certificado;

        /// <summary>
        /// Construtor ProdutosEstoqueController
        /// </summary>
        /// <param name="produtosEstoque"></param>
        /// <param name="produtos"></param>
        /// <param name="tamanhos"></param>
        /// <param name="usuario"></param>
        /// <param name="logEstoque"></param>
        /// <param name="certificado"></param>
        public ControllerProdutosEstoque(IEPIProdutosEstoqueBLL produtosEstoque, IEPIProdutosBLL produtos, IEPITamanhosBLL tamanhos,
            IRHConUserBLL usuario, IEPILogEstoqueBLL logEstoque, IEPICertificadoAprovacaoBLL certificado)
        {
            _produtosEstoque = produtosEstoque;
            _produtos = produtos;
            _tamanhos = tamanhos;
            _usuario = usuario;
            _logEstoque = logEstoque;
            _certificado = certificado;
        }

        /// <summary>
        /// Insere produtos no estoque
        /// </summary>
        /// <param name="produto"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> insereEstoque([FromBody] EPIProdutosEstoqueDTO produto)
        {
            try
            {
                if (produto != null)
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
                else
                {
                    return BadRequest(new { message = "Nenhum produto enviado", result = false });
                }                
            }
            catch (System.Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Aatualiza quantidade disponivel em estoque
        /// </summary>
        /// <returns></returns>
        [HttpPut("estoque")]
        public async Task<IActionResult> atualizaEstoque([FromBody] List<EPIProdutosEstoqueDTO> estoque)
        {
            try
            {
                string message = string.Empty;

                if (estoque != null)
                {
                    foreach (var item in estoque)
                    {
                        var localizaEstoque = await _produtosEstoque.getProdutoEstoqueTamanho(item.idProduto, item.idTamanho);

                        if (localizaEstoque != null)
                        {
                            localizaEstoque.quantidade = item.quantidade;

                            await _produtosEstoque.Update(localizaEstoque);
                        }
                        else
                        {
                            message += "Produtos não encontrados '" + item.idProduto + "'";
                        }                        
                    }

                    return Ok(new { message = "Estoque atualizado com sucesso", wrongData = message, result = true });
                }
                else
                {
                    return BadRequest(new { message = "Nenhuma atualização em estoque enviado", result = false });
                }
               
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Ativa/Desativa Produto do estoque
        /// </summary>
        /// <returns></returns>
        [HttpPut("status/{idProduto}/{status}")]
        public async Task<IActionResult> ativaDesativaProdutoEstoque(int idProduto, string status)
        {
            try
            {
                var localizaProduto = await _produtos.localizaProduto(idProduto);

                if (localizaProduto != null)
                {
                    localizaProduto.ativo = status;

                    await _produtos.Update(localizaProduto);

                    if (status == "S")
                    {
                        return Ok(new { message = "Produto ativado com sucesso!!!", result = true });
                    }
                    else if (status == "N")
                    {
                        return Ok(new { message = "Produto desativado com sucesso!!!", result = true });
                    }
                    else
                    {
                        return BadRequest(new { message = "Erro ao atualizar status do produto", result = false });
                    }
                }
                else
                {
                    return BadRequest(new { message = "Produto em estoque não encontrado ", result = false });
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
        [HttpGet]
        public async Task<IActionResult> listaTodosProdutosEmEstoque()
        {
            try
            {
                var listaDeProdutos = await _produtos.getProdutos();

                if (listaDeProdutos != null)
                {
                    List<object> gerenciaEstoque = new List<object>();

                    foreach (var item in listaDeProdutos)
                    {
                        var localizaCertificado = await _certificado.getCertificado(item.idCertificado);
                        var quantidadeEstoque = await _produtosEstoque.getProdutosExistentes(item.id);

                        gerenciaEstoque.Add(new
                        {
                            idProduto = item.id,
                            quantidade = quantidadeEstoque,
                            produto = item.nomeProduto,
                            preco = item.preco,
                            certificado = localizaCertificado.numero,
                            validadeCertificado = localizaCertificado.validade,
                            ativo = item.ativo
                        });
                    }

                    return Ok(new { message = "Produtos encontrados", result = true, lista = gerenciaEstoque });
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
        [HttpGet("{id}")]
        public async Task<IActionResult> selecionaProduto(int id)
        {
            try
            {
                var localizaProduto = await _produtos.getProduto(id);
                var quantidadeEstoque = await _produtosEstoque.getProdutoExistente(localizaProduto.id);
                var localizaCertificado = await _certificado.getCertificado(localizaProduto.idCertificado);

                List<object> gerenciaEstoque = new List<object>();

                if (localizaProduto != null)
                {
                    gerenciaEstoque.Add(new
                    {
                        idProduto = localizaProduto.id,
                        quantidade = quantidadeEstoque,
                        produto = localizaProduto.nomeProduto,
                        preco = localizaProduto.preco,
                        certificado = localizaCertificado.numero,
                        validadeCertificado = localizaCertificado.validade,
                        ativo = localizaProduto.ativo
                    });

                    return Ok(new { message = "Produto encontrado", result = true, produto = gerenciaEstoque });
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
    }
}
