using ControleEPI.DAL.EPIProdutos;
using ControleEPI.DTO;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ControleEPI.BLL.EPIProdutos
{
    public class EPIProdutosBLL : IEPIProdutosBLL
    {
        private readonly IEPIProdutosDAL _produto;
        public EPIProdutosBLL(IEPIProdutosDAL produto)
        {
            _produto = produto;
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

        public async Task<IList<EPIProdutosDTO>> produtosStatus(string status)
        {
            try
            {
                var statusProduto = await _produto.produtosStatus(status);

                if (statusProduto != null)
                {
                    return statusProduto;
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

        public async Task<EPIProdutosDTO> verificaCategoria(int idCategoria)
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
