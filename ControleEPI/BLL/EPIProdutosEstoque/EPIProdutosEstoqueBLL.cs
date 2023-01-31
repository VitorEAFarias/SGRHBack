using ControleEPI.DAL.EPIProdutosEstoque;
using ControleEPI.DTO;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ControleEPI.BLL.EPIProdutosEstoque
{
    public class EPIProdutosEstoqueBLL : IEPIProdutosEstoqueBLL
    {
        private readonly IEPIProdutosEstoqueDAL _produtosEstoque;

        public EPIProdutosEstoqueBLL(IEPIProdutosEstoqueDAL produtosEstoque)
        {
            _produtosEstoque = produtosEstoque;
        }

        public async Task<EPIProdutosEstoqueDTO> getProdutoEstoque(int id)
        {
            try
            {
                var localizaProdutoEstoque = await _produtosEstoque.getProdutoEstoque(id);

                if (localizaProdutoEstoque != null)
                {
                    return localizaProdutoEstoque;
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

        public async Task<EPIProdutosEstoqueDTO> getProdutoEstoqueTamanho(int id, int idTamanho)
        {
            try
            {
                var localizaProdutoEstoqueTamanho = await _produtosEstoque.getProdutoEstoqueTamanho(id, idTamanho);

                if (localizaProdutoEstoqueTamanho != null)
                {
                    return localizaProdutoEstoqueTamanho;
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

        public async Task<EPIProdutosEstoqueDTO> getProdutoExistente(int idProduto)
        {
            try
            {
                var localizaProdutoEstoque = await _produtosEstoque.getProdutoExistente(idProduto);

                if (localizaProdutoEstoque != null)
                {
                    return localizaProdutoEstoque;
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

        public async Task<IList<EPIProdutosEstoqueDTO>> getProdutosEstoque()
        {
            try
            {
                var localizaProdutosEstoque = await _produtosEstoque.getProdutosEstoque();

                if (localizaProdutosEstoque != null)
                {
                    return localizaProdutosEstoque;
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

        public async Task<IList<EPIProdutosEstoqueDTO>> getProdutosExistentes(int idProdutos)
        {
            try
            {
                var localizaProdutosEstoque = await _produtosEstoque.getProdutosExistentes(idProdutos);

                if (localizaProdutosEstoque != null)
                {
                    return localizaProdutosEstoque;
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
                var atualizaProdutoEstoque = await _produtosEstoque.Update(produto);

                if (atualizaProdutoEstoque != null)
                {
                    return atualizaProdutoEstoque;
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
