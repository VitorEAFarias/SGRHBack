using ControleEPI.DAL.EPICertificados;
using ControleEPI.DAL.EPIProdutos;
using ControleEPI.DAL.EPIProdutosEstoque;
using ControleEPI.DAL.EPITamanhos;
using ControleEPI.DTO;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ControleEPI.BLL.EPIProdutosEstoque
{
    public class EPIProdutosEstoqueBLL : IEPIProdutosEstoqueBLL
    {
        private readonly IEPIProdutosEstoqueDAL _produtosEstoque;
        private readonly IEPIProdutosDAL _produtos;
        private readonly IEPITamanhosDAL _tamanhos;
        private readonly IEPICertificadoAprovacaoDAL _certificado;

        public EPIProdutosEstoqueBLL(IEPIProdutosEstoqueDAL produtosEstoque, IEPIProdutosDAL produtos, IEPITamanhosDAL tamanhos, IEPICertificadoAprovacaoDAL certificado)
        {
            _produtosEstoque = produtosEstoque;
            _produtos = produtos;
            _tamanhos = tamanhos;
            _certificado = certificado;
        }

        public async Task<EPIProdutosEstoqueDTO> ativaDesativaProdutoEstoque(int idEstoque)
        {
            try
            {
                var localizaProduto = await _produtosEstoque.getProdutoEstoque(idEstoque);

                if (localizaProduto != null || localizaProduto.quantidade > 0)
                {
                    var atualizaEstoque = await _produtosEstoque.Update(localizaProduto);

                    if (atualizaEstoque != null)
                    {
                        return atualizaEstoque;
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

        public async Task<TodosProdutosEstoqueDTO> getProdutoEstoque(int id)
        {
            try
            {
                var localizaProdutoEstoque = await _produtosEstoque.getProdutoEstoque(id);

                if (localizaProdutoEstoque != null)
                {
                    var localizaProduto = await _produtos.localizaProduto(localizaProdutoEstoque.idProduto);
                    var localizaCertificado = await _certificado.getCertificado(localizaProduto.idCertificadoAprovacao);
                    var tamanho = await _tamanhos.localizaTamanho(localizaProdutoEstoque.idTamanho);

                    TodosProdutosEstoqueDTO gerenciaEstoque = new TodosProdutosEstoqueDTO();

                    if (tamanho != null)
                    {
                        gerenciaEstoque = new TodosProdutosEstoqueDTO
                        {
                            id = localizaProdutoEstoque.id,
                            quantidade = localizaProdutoEstoque.quantidade,
                            idTamanho = tamanho.id,
                            tamanho = tamanho.tamanho,
                            idProduto = localizaProduto.id,
                            produto = localizaProduto.nome,
                            preco = localizaProduto.preco,
                            certificado = localizaCertificado.numero,
                            validadeCertificado = localizaCertificado.validade,
                            ativo = localizaProduto.ativo
                        };
                    }
                    else
                    {
                        gerenciaEstoque = new TodosProdutosEstoqueDTO
                        {
                            id = localizaProdutoEstoque.id,
                            quantidade = localizaProdutoEstoque.quantidade,
                            idTamanho = 0,
                            tamanho = "Tamanho Único",
                            idProduto = localizaProduto.id,
                            produto = localizaProduto.nome,
                            preco = localizaProduto.preco,
                            certificado = localizaCertificado.numero,
                            validadeCertificado = localizaCertificado.validade,
                            ativo = localizaProduto.ativo
                        };
                    }
                    

                    if (gerenciaEstoque != null)
                    {
                        return gerenciaEstoque;
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

        public async Task<IList<TodosProdutosEstoqueDTO>> getProdutosEstoque()
        {
            try
            {
                var listaDeProdutosEstoque = await _produtosEstoque.getProdutosEstoque();

                if (listaDeProdutosEstoque != null)
                {
                    List<TodosProdutosEstoqueDTO> gerenciaEstoque = new List<TodosProdutosEstoqueDTO>();

                    foreach (var item in listaDeProdutosEstoque)
                    {
                        var nomeProduto = await _produtos.localizaProduto(item.idProduto);
                        var localizaCertificado = await _certificado.getCertificado(nomeProduto.idCertificadoAprovacao);
                        var tamanho = await _tamanhos.localizaTamanho(item.idTamanho);

                        if (tamanho != null)
                        {
                            gerenciaEstoque.Add(new TodosProdutosEstoqueDTO
                            {
                                id = item.id,
                                quantidade = item.quantidade,
                                idTamanho = tamanho.id,
                                tamanho = tamanho.tamanho,
                                idProduto = nomeProduto.id,
                                produto = nomeProduto.nome,
                                preco = nomeProduto.preco,
                                certificado = localizaCertificado.numero,
                                validadeCertificado = localizaCertificado.validade
                            });
                        }
                        else
                        {
                            gerenciaEstoque.Add(new TodosProdutosEstoqueDTO
                            {
                                id = item.id,
                                quantidade = item.quantidade,
                                idTamanho = 0,
                                tamanho = "Tamanho Único",
                                idProduto = nomeProduto.id,
                                produto = nomeProduto.nome,
                                preco = nomeProduto.preco,
                                certificado = localizaCertificado.numero,
                                validadeCertificado = localizaCertificado.validade
                            });
                        }
                        
                    }

                    return gerenciaEstoque;
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

        public async Task<EPIProdutosEstoqueDTO> Insert(EPIProdutosEstoqueDTO produto)
        {
            try
            {
                var insereProdutoEstoque = await _produtosEstoque.Insert(produto);

                if (insereProdutoEstoque != null)
                {
                    return insereProdutoEstoque;
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

        public async Task<EPIProdutosEstoqueDTO> Update(EPIProdutosEstoqueDTO produto)
        {
            try
            {                
                var localizaEstoque = await _produtosEstoque.getProdutoEstoqueTamanho(produto.idProduto, produto.idTamanho);

                if (localizaEstoque != null)
                {
                    localizaEstoque.quantidade = produto.quantidade;

                    var atualizaEstoque = await _produtosEstoque.Update(localizaEstoque);

                    if (atualizaEstoque != null)
                    {
                        return atualizaEstoque;
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
    }
}
