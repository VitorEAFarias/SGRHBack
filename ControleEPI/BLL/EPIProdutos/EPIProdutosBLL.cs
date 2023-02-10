using ControleEPI.DAL.EPICategorias;
using ControleEPI.DAL.EPICertificados;
using ControleEPI.DAL.EPIProdutos;
using ControleEPI.DAL.EPIProdutosEstoque;
using ControleEPI.DAL.EPITamanhos;
using ControleEPI.DTO;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ControleEPI.BLL.EPIProdutos
{
    public class EPIProdutosBLL : IEPIProdutosBLL
    {
        private readonly IEPIProdutosDAL _produto;
        private readonly IEPITamanhosDAL _tamanhos;
        private readonly IEPIProdutosEstoqueDAL _estoque;
        private readonly IEPICategoriasDAL _categoria;
        private readonly IEPICertificadoAprovacaoDAL _certificado;

        public EPIProdutosBLL(IEPIProdutosDAL produto, IEPITamanhosDAL tamanhos, IEPIProdutosEstoqueDAL estoque, IEPICategoriasDAL categoria, IEPICertificadoAprovacaoDAL certificado)
        {
            _produto = produto;
            _tamanhos = tamanhos;
            _estoque = estoque;
            _categoria = categoria;
            _certificado = certificado;
        }

        public async Task<EPIProdutosDTO> ativaDesativaProduto(string status, int id)
        {
            try
            {
                var localizaProduto = await _produto.localizaProduto(id);

                if (localizaProduto != null)
                {
                    localizaProduto.ativo = status;

                    await _produto.Update(localizaProduto);

                    if (status == "S")
                    {
                        return localizaProduto;
                    }
                    else
                    {
                        return localizaProduto;
                    }
                }
                else
                {
                    return null;
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<IList<ProdutosDTO>> getProdutosSolicitacao()
        {
            try
            {
                var localizaProdutos = await _produto.getProdutosSolicitacao();

                if (localizaProdutos != null)
                {
                    List<ProdutosDTO> produtoRetorno = new List<ProdutosDTO>();

                    foreach (var item in localizaProdutos)
                    {
                        var verificaCategoria = await _categoria.getCategoria(item.idCategoria);
                        var verificaCertificado = await _certificado.getCertificado(item.idCertificadoAprovacao);

                        produtoRetorno.Add(new ProdutosDTO
                        {
                            idProduto = item.id,
                            idCategoria = verificaCategoria.id,
                            idCertificadoAprovacao = verificaCertificado.id,
                            produto = item.nome,
                            categoria = verificaCategoria.nome,
                            certificado = verificaCertificado.numero,
                            ativo = item.ativo,
                            foto = item.foto,
                            validadeEmUso = item.validadeEmUso,
                            preco = item.preco,
                            maximo = item.maximo
                        });
                    }

                    return produtoRetorno;
                }
                else
                {
                    return null;
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<EPIProdutosDTO> Insert(EPIProdutosDTO produto)
        {
            try
            {
                if (produto.validadeEmUso >= 0 && produto.validadeEmUso <= 5)
                {
                    var localizaProduto = await _produto.getNomeProduto(produto.nome);

                    if (localizaProduto == null)
                    {
                        produto.ativo = "S";

                        var insereProduto = await _produto.Insert(produto);

                        if (insereProduto != null)
                        {
                            EPIProdutosEstoqueDTO novoEstoque = new EPIProdutosEstoqueDTO();

                            var localizaTamanhosProduto = await _tamanhos.tamanhosCategoria(insereProduto.idCategoria);

                            foreach (var item in localizaTamanhosProduto)
                            {
                                novoEstoque.idProduto = insereProduto.id;
                                novoEstoque.quantidade = 0;
                                novoEstoque.idTamanho = item.id;
                                novoEstoque.ativo = "S";

                                await _estoque.Insert(novoEstoque);
                            }

                            return insereProduto;
                        }
                        else
                        {
                            return null;
                        }
                    }
                    else
                    {
                        return null;
                    }
                }
                else
                {
                    return null;
                }                
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<ProdutosDTO> localizaProduto(int id)
        {
            try
            {
                var localizaProduto = await _produto.localizaProduto(id);

                if (localizaProduto != null)
                {
                    ProdutosDTO produtoRetorno = new ProdutosDTO();

                    var verificaCategoria = await _categoria.getCategoria(localizaProduto.idCategoria);
                    var verificaCertificado = await _certificado.getCertificado(localizaProduto.idCertificadoAprovacao);

                    produtoRetorno = new ProdutosDTO
                    {
                        idProduto = localizaProduto.id,
                        idCategoria = verificaCategoria.id,
                        idCertificadoAprovacao = verificaCertificado.id,
                        produto = localizaProduto.nome,
                        categoria = verificaCategoria.nome,
                        certificado = verificaCertificado.numero,
                        ativo = localizaProduto.ativo,
                        foto = localizaProduto.foto,
                        validadeEmUso = localizaProduto.validadeEmUso,
                        preco = localizaProduto.preco,
                        maximo = localizaProduto.maximo
                    };

                    return produtoRetorno;
                }
                else
                {
                    return null;
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<IList<ProdutosTamanhosDTO>> produtosTamanhos(string status)
        {
            try
            {
                var localizaTamanhosCategoria = await _tamanhos.localizaTamanhos();
                var LocalizaProdutosAtivos = await _produto.produtosStatus(status);

                List<ProdutosTamanhosDTO> produtos = new List<ProdutosTamanhosDTO>();                

                foreach (var produto in LocalizaProdutosAtivos)
                {
                    List<Tamanho> tamanhos = new List<Tamanho>();

                    foreach (var tamanho in localizaTamanhosCategoria)
                    {
                        if (produto.idCategoria == tamanho.idCategoriaProduto)
                        {
                            tamanhos.Add(new Tamanho
                            {
                                idTamanho = tamanho.id,
                                tamanho = tamanho.tamanho
                            });                            
                        }
                    }

                    produtos.Add(new ProdutosTamanhosDTO
                    {
                        id = produto.id,
                        nome = produto.nome,
                        idCategoria = produto.idCategoria,
                        preco = produto.preco,
                        idCertificadoAprovacao = produto.idCertificadoAprovacao,
                        validadeEmUso = produto.validadeEmUso,
                        ativo = produto.ativo,
                        foto = produto.foto,
                        maximo = produto.maximo,
                        tamanhos = tamanhos
                    });
                }

                if (produtos != null)
                {
                    return produtos;
                }
                else
                {
                    return null;
                }
            }
            catch (Exception ex)
            {

                throw new Exception(ex.Message);
            }
        }

        public async Task<EPIProdutosDTO> Update(EPIProdutosDTO produtos)
        {
            try
            {
                var localizaProduto = await _produto.localizaProduto(produtos.id);

                if (localizaProduto != null)
                {
                    if (localizaProduto.nome == produtos.nome)
                    {
                        await _produto.Update(produtos);

                        return localizaProduto;
                    }
                    else
                    {
                        var verificaNomeProduto = await _produto.getNomeProduto(produtos.nome);

                        if (verificaNomeProduto == null)
                        {
                            await _produto.Update(produtos);

                            return localizaProduto;
                        }
                        else
                        {
                            return null;
                        }
                    }
                }
                else
                {
                    return null;
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<ProdutosTamanhosDTO> localizaProdutoTamanhos(int idProduto)
        {
            try
            {
                var localizaProduto = await _produto.localizaProduto(idProduto);

                if (localizaProduto != null)
                {
                    var localizaTamanhos = await _tamanhos.tamanhosCategoria(localizaProduto.idCategoria);

                    if (localizaTamanhos != null)
                    {
                        ProdutosTamanhosDTO produtos = new ProdutosTamanhosDTO();
                        List<Tamanho> tamanhos = new List<Tamanho>();

                        foreach (var item in localizaTamanhos)
                        {
                            tamanhos.Add(new Tamanho { 
                                idTamanho = item.id,
                                tamanho = item.tamanho
                            });
                        }

                        produtos = new ProdutosTamanhosDTO
                        {
                            id = localizaProduto.id,
                            nome = localizaProduto.nome,
                            tamanhos = tamanhos
                        };

                        if (produtos != null)
                        {
                            return produtos;
                        }
                        else
                        {
                            return null;
                        }
                    }
                    else
                    {
                        return null;
                    }                    
                }
                else
                {
                    return null;
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<IList<EPIProdutosDTO>> verificaCategorias(int idCategoria)
        {
            try
            {
                var verificaVinculoProduto = await _produto.verificaCategorias(idCategoria);

                if (verificaVinculoProduto != null)
                {
                    return verificaVinculoProduto;
                }
                else
                {
                    return null;
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}
