using ControleEPI.DAL.EPICategorias;
using ControleEPI.DTO;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ControleEPI.BLL.EPICategorias
{
    public class EPICategoriasBLL : IEPICategoriasBLL
    {
        private readonly IEPICategoriasDAL _categoria;

        public EPICategoriasBLL(IEPICategoriasDAL categoria)
        {
            _categoria = categoria;
        }

        public async Task<EPICategoriasDTO> Delete(int id)
        {
            try
            {
                var deletaCategoria = await _categoria.Delete(id);

                if (deletaCategoria != null)
                {
                    return deletaCategoria;
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
