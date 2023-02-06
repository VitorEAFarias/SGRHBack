using ControleEPI.DAL.EPICategorias;
using ControleEPI.DAL.EPIPedidos;
using ControleEPI.DAL.EPIProdutos;
using ControleEPI.DAL.EPITamanhos;
using ControleEPI.DTO;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Vestimenta.DTO;

namespace ControleEPI.BLL.EPITamanhos
{
    public class EPITamanhosBLL : IEPITamanhosBLL
    {
        private readonly IEPITamanhosDAL _tamanho;
        private readonly IEPICategoriasDAL _categoria;
        private readonly IEPIProdutosDAL _produtos;

        public EPITamanhosBLL(IEPITamanhosDAL tamanho, IEPICategoriasDAL categoria, IEPIProdutosDAL produtos)
        {
            _tamanho = tamanho;
            _categoria = categoria;
            _produtos = produtos;
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

                    if (verificaTamanho != null)
                    {
                        return null;
                    }
                    else
                    {
                        var insereTamanho = await _tamanho.insereTamanho(tamanho);

                        if (insereTamanho != null)
                        {
                            return insereTamanho;
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

        public async Task<TamanhosDTO> localizaTamanho(int Id)
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
                    return null;
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

        public Task<EPITamanhosDTO> verificaTamanho(string nome)
        {
            throw new NotImplementedException();
        }
    }
}
