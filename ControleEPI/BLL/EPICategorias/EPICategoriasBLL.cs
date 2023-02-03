using ControleEPI.DAL.EPICategorias;
using ControleEPI.DAL.EPIProdutos;
using ControleEPI.DTO;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ControleEPI.BLL.EPICategorias
{
    public class EPICategoriasBLL : IEPICategoriasBLL
    {
        private readonly IEPICategoriasDAL _categoria;
        private readonly IEPIProdutosDAL _produto;

        public EPICategoriasBLL(IEPICategoriasDAL categoria, IEPIProdutosDAL produto)
        {
            _categoria = categoria;
            _produto = produto;
        }

        public async Task<EPICategoriasDTO> Delete(int id)
        {
            try
            {
                var localizaCategoria = await _categoria.getCategoria(id);

                if (localizaCategoria != null)
                {
                    var verificaProduto = await _produto.verificaCategoria(localizaCategoria.id);

                    if (verificaProduto == null)
                    {
                        var deletaVinculoProduto = await _categoria.Delete(localizaCategoria.id);

                        if (deletaVinculoProduto != null)
                        {
                            return localizaCategoria;
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

        public async Task<EPICategoriasDTO> getCategoria(int Id)
        {
            try
            {
                var localizaCategoria = await _categoria.getCategoria(Id);

                if (localizaCategoria != null)
                {
                    return localizaCategoria;
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

        public async Task<IList<EPICategoriasDTO>> getCategorias()
        {
            try
            {
                var localizaCategorias = await _categoria.getCategorias();

                if (localizaCategorias != null)
                {
                    return localizaCategorias;
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
            throw new NotImplementedException();
        }

        public Task<EPICategoriasDTO> Insert(EPICategoriasDTO categoria)
        {
            try
            {
                var insereCategoria = _categoria.Insert(categoria);

                if (insereCategoria != null)
                {
                    return insereCategoria;
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

        public async Task<EPICategoriasDTO> Update(EPICategoriasDTO categoria)
        {
            try
            {
                var atualizaCategoria = await _categoria.Update(categoria);

                if (atualizaCategoria != null)
                {
                    return atualizaCategoria;
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

        public async Task<EPICategoriasDTO> verificaCategoria(string nome)
        {
            try
            {
                var verificaNomeCategoria = await _categoria.verificaCategoria(nome);

                if (verificaNomeCategoria != null)
                {
                    return verificaNomeCategoria;
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
