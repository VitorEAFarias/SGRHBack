using ControleEPI.DAL.EPIProdutos;
using ControleEPI.DAL.EPITamanhos;
using ControleEPI.DTO;
using Microsoft.EntityFrameworkCore.Metadata.Conventions;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ControleEPI.BLL.EPIProdutos
{
    public class EPIProdutosBLL : IEPIProdutosBLL
    {
        private readonly IEPIProdutosDAL _produto;
        private readonly IEPITamanhosDAL _tamanhos;

        public EPIProdutosBLL(IEPIProdutosDAL produto, IEPITamanhosDAL tamanhos)
        {
            _produto = produto;
            _tamanhos = tamanhos;
        }

        public Task<EPIProdutosDTO> ativaDesativaProduto(int id)
        {
            throw new NotImplementedException();
        }

        public async Task<EPIProdutosDTO> getCertificadoProduto(int idCertificado)
        {
            try
            {
                var localizaCertificado = await _produto.getCertificadoProduto(idCertificado);

                if (localizaCertificado != null)
                {
                    return localizaCertificado;
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

        public async Task<EPIProdutosDTO> getNomeProduto(string nome)
        {
            try
            {
                var verificaNomeProduto = await _produto.getNomeProduto(nome);

                if (verificaNomeProduto != null)
                {
                    return verificaNomeProduto;
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

        public async Task<IList<EPIProdutosDTO>> getProdutosSolicitacao()
        {
            try
            {
                var localizaProdutos = await _produto.getProdutosSolicitacao();

                if (localizaProdutos != null)
                {
                    return localizaProdutos;
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
                var insereProduto = await _produto.Insert(produto);

                if (insereProduto != null)
                {
                    return insereProduto;
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

        public async Task<EPIProdutosDTO> localizaProduto(int id)
        {
            try
            {
                var localizaProduto = await _produto.localizaProduto(id);

                if (localizaProduto != null)
                {
                    return localizaProduto;
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

        public async Task<EPIProdutosDTO> Update(EPIProdutosDTO produto)
        {
            try
            {
                var atualizaProduto = await _produto.Update(produto);

                if (atualizaProduto != null)
                {
                    return atualizaProduto;
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

        public async Task<IList<EPIProdutosDTO>> verificaCategoria(int idCategoria)
        {
            try
            {
                var verificaVinculoProduto = await _produto.verificaCategoria(idCategoria);

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
