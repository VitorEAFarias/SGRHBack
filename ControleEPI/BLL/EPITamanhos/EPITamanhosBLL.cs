using ControleEPI.DAL.EPICategorias;
using ControleEPI.DAL.EPIProdutos;
using ControleEPI.DAL.EPIProdutosEstoque;
using ControleEPI.DAL.EPITamanhos;
using ControleEPI.DTO;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ControleEPI.BLL.EPITamanhos
{
    public class EPITamanhosBLL : IEPITamanhosBLL
    {
        private readonly IEPITamanhosDAL _tamanho;
        private readonly IEPICategoriasDAL _categoria;
        private readonly IEPIProdutosDAL _produtos;
        private readonly IEPIProdutosEstoqueDAL _estoque;

        public EPITamanhosBLL(IEPITamanhosDAL tamanho, IEPICategoriasDAL categoria, IEPIProdutosDAL produtos, IEPIProdutosEstoqueDAL estoque)
        {
            _tamanho = tamanho;
            _categoria = categoria;
            _produtos = produtos;
            _estoque = estoque;
        }

        public async Task<EPITamanhosDTO> Delete(int id)
        {
            try
            {
                var localizaTamanho = await _tamanho.localizaTamanho(id);

                if (localizaTamanho != null)
                {
                    var localizaProduto = await _produtos.verificaCategoria(localizaTamanho.idCategoriaProduto);

                    if (localizaProduto == null)
                    {
                        var deletarTamanho = await _tamanho.Delete(id);

                        if (deletarTamanho != null)
                        {
                            return deletarTamanho;
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

        public async Task<EPITamanhosDTO> insereTamanho(EPITamanhosDTO tamanho)
        {
            try
            {
                try
                {
                    var verificaTamanho = await _tamanho.verificaTamanho(tamanho.tamanho);                    

                    if (verificaTamanho != null && verificaTamanho.idCategoriaProduto == tamanho.idCategoriaProduto)
                    {
                        return null;
                    }
                    else
                    {
                        var insereTamanho = await _tamanho.insereTamanho(tamanho);

                        if (insereTamanho != null)
                        {
                            var localizaCategoria = await _categoria.getCategoria(insereTamanho.idCategoriaProduto);

                            if (localizaCategoria != null)
                            {
                                var verificaCategoriaProduto = await _produtos.verificaCategorias(localizaCategoria.id);

                                if (verificaCategoriaProduto != null)
                                {
                                    foreach (var item in verificaCategoriaProduto)
                                    {
                                        EPIProdutosEstoqueDTO adicionaEstoque = new EPIProdutosEstoqueDTO();

                                        adicionaEstoque.idProduto = item.id;
                                        adicionaEstoque.quantidade = 0;
                                        adicionaEstoque.idTamanho = insereTamanho.id;
                                        adicionaEstoque.ativo = "S";

                                        await _estoque.Insert(adicionaEstoque);
                                    }

                                    return insereTamanho;
                                }
                                else
                                {
                                    return insereTamanho;
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
                }
                catch (Exception ex)
                {
                    throw new Exception(ex.Message);
                }                
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<EPITamanhosDTO> localizaTamanho(int id)
        {
            try
            {
                var localizaTamanho = await _tamanho.localizaTamanho(id);

                if (localizaTamanho != null )
                {
                    return localizaTamanho;
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

        public async Task<TamanhosDTO> localizarTamanho(int Id)
        {
            try
            {
                var localizaTamanho = await _tamanho.localizaTamanho(Id);

                if (localizaTamanho != null)
                {
                    TamanhosDTO tamanhos = new TamanhosDTO();

                    var localizaCategoria = await _categoria.getCategoria(localizaTamanho.idCategoriaProduto);

                    tamanhos = new TamanhosDTO
                    {
                        id = localizaTamanho.id,
                        tamanho = localizaTamanho.tamanho,
                        idCategoriaProduto = localizaTamanho.idCategoriaProduto,
                        nome = localizaCategoria.nome,
                        ativo = localizaTamanho.ativo
                    };

                    return tamanhos;
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

        public async Task<IList<TamanhosDTO>> localizaTamanhos()
        {
            try
            {
                var localizaTamanhos = await _tamanho.localizaTamanhos();

                if (localizaTamanhos != null)
                {
                    List<TamanhosDTO> tamanhos = new List<TamanhosDTO>();

                    foreach (var item in localizaTamanhos)
                    {
                        var localizaCategoria = await _categoria.getCategoria(item.idCategoriaProduto);

                        tamanhos.Add(new TamanhosDTO
                        {
                            id = item.id,
                            tamanho = item.tamanho,
                            idCategoriaProduto = item.idCategoriaProduto,
                            nome = localizaCategoria.nome,
                            ativo = item.ativo
                        });
                    }

                    if (tamanhos != null)
                    {
                        return tamanhos;
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

        public async Task<IList<EPITamanhosDTO>> tamanhosCategoria(int idCategoria)
        {
            try
            {
                var localizaTamanhosCategoria = await _tamanho.tamanhosCategoria(idCategoria);

                if (localizaTamanhosCategoria != null)
                {
                    return localizaTamanhosCategoria;
                }
                else
                {
                    var localizaCategoria = await _categoria.getCategoria(idCategoria);

                    if (localizaCategoria != null)
                    {
                        return (IList<EPITamanhosDTO>)localizaCategoria;
                    }
                    else
                    {
                        return null;
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<EPITamanhosDTO> Update(TamanhosDTO tamanho)
        {
            try
            {
                EPITamanhosDTO atualizarTamanho = new EPITamanhosDTO();

                atualizarTamanho.id = tamanho.id;
                atualizarTamanho.tamanho = tamanho.tamanho;
                atualizarTamanho.idCategoriaProduto = tamanho.idCategoriaProduto;
                atualizarTamanho.ativo = tamanho.ativo;                

                var atualizaTamanho = await _tamanho.Update(atualizarTamanho);

                if (atualizaTamanho != null)
                {
                    return atualizaTamanho;
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

        public async Task<EPITamanhosDTO> verificaTamanho(string nome)
        {
            try
            {
                var verificarTamanho = await _tamanho.verificaTamanho(nome);

                if (verificarTamanho != null)
                {
                    return verificarTamanho;
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
