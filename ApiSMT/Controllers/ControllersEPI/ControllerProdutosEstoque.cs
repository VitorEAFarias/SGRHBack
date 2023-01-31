using System;
using ControleEPI.DTO;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.AspNetCore.Authorization;
using Vestimenta.DTO;
using ControleEPI.BLL.Produtos;
using ControleEPI.BLL.Certificado;
using ControleEPI.BLL.LogEstoque;
using ControleEPI.BLL.ProdutosEstoque;
using ControleEPI.BLL.Tamanhos;
using ControleEPI.BLL.RHUsuarios;

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
        [Authorize]
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
        [Authorize]
        [HttpPut("estoque")]
        public async Task<IActionResult> atualizaEstoque([FromBody] EPIProdutosEstoqueDTO estoque)
        {
            try
            {
                string message = string.Empty;

                if (estoque != null)
                {                    
                    var localizaEstoque = await _produtosEstoque.getProdutoEstoqueTamanho(estoque.idProduto, estoque.idTamanho);

                    if (localizaEstoque != null)
                    {
                        localizaEstoque.quantidade = estoque.quantidade;

                        await _produtosEstoque.Update(localizaEstoque);
                    }
                    else
                    {
                        message += "Produtos não encontrados '" + estoque.idProduto + "'";
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
        [Authorize]
        [HttpPut("status/{idEstoque}/{status}")]
        public async Task<IActionResult> ativaDesativaProdutoEstoque(int idEstoque, string status)
        {
            try
            {
                var localizaProduto = await _produtosEstoque.getProdutoEstoque(idEstoque);

                if (localizaProduto != null)
                {
                    localizaProduto.ativo = status;

                    await _produtosEstoque.Update(localizaProduto);

                    if (status == "S")
                    {
                        return Ok(new { message = "Produto do estoque ativado com sucesso!!!", result = true });
                    }
                    else if (status == "N")
                    {
                        return Ok(new { message = "Produto do estoque desativado com sucesso!!!", result = true });
                    }
                    else
                    {
                        return BadRequest(new { message = "Erro ao atualizar status do produto no estoque", result = false });
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
        [Authorize]
        [HttpGet]
        public async Task<IActionResult> listaTodosProdutosEmEstoque()
        {
            try
            {
                var listaDeProdutosEstoque = await _produtosEstoque.getProdutosEstoque();

                if (listaDeProdutosEstoque != null)
                {
                    List<object> gerenciaEstoque = new List<object>();

                    foreach (var item in listaDeProdutosEstoque)
                    {
                        var nomeProduto = await _produtos.localizaProduto(item.idProduto);
                        var localizaCertificado = await _certificado.getCertificado(nomeProduto.idCertificadoAprovacao);                        
                        var tamanho = await _tamanhos.localizaTamanho(item.idTamanho);

                        gerenciaEstoque.Add(new
                        {
                            idEstoque = item.id,
                            quantidade = item.quantidade,
                            tamanho = tamanho.tamanho,
                            produto = nomeProduto.nome,
                            preco = nomeProduto.preco,
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
        [Authorize]
        [HttpGet("{id}")]
        public async Task<IActionResult> selecionaProduto(int id)
        {
            try
            {
                var localizaProdutoEstoque = await _produtosEstoque.getProdutoEstoque(id);
                var localizaProduto = await _produtos.getProduto(localizaProdutoEstoque.idProduto);
                var localizaCertificado = await _certificado.getCertificado(localizaProduto.idCertificado);
                var tamanho = await _tamanhos.localizaTamanho(localizaProdutoEstoque.idTamanho);

                List<object> gerenciaEstoque = new List<object>();

                if (localizaProduto != null)
                {
                    gerenciaEstoque.Add(new
                    {
                        idEstoque = localizaProdutoEstoque.id,
                        idProduto = localizaProduto.id,
                        quantidade = localizaProdutoEstoque.quantidade,
                        idTamanho = tamanho.id,
                        tamanho = tamanho.tamanho,
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
