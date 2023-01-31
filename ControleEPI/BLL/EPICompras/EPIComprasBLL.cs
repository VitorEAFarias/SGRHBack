using ControleEPI.DAL.EPICompras;
using ControleEPI.DTO;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ControleEPI.BLL.EPICompras
{
    public class EPIComprasBLL : IEPIComprasBLL
    {
        private readonly IEPIComprasDAL _compras;

        public EPIComprasBLL(IEPIComprasDAL compras)
        {
            _compras = compras;
        }

        public async Task<EPIComprasDTO> Delete(int id)
        {
            try
            {
                var deletaCompra = await _compras.Delete(id);

                if (deletaCompra != null)
                {
                    return deletaCompra;
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

        public async Task<EPIComprasDTO> getCompra(int Id)
        {
            try
            {
                var localizaCompra = await _compras.getCompra(Id);

                if (localizaCompra != null)
                {
                    return localizaCompra;
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

        public async Task<IList<EPIComprasDTO>> getCompras(string status)
        {
            try
            {
                var localizaCompras = await _compras.getCompras(status);

                if (localizaCompras != null)
                {
                    return localizaCompras;
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

        public async Task<IList<EPIComprasDTO>> getStatusCompras(int status)
        {
            try
            {
                var localizaStatusCompras = await _compras.getStatusCompras(status);

                if (localizaStatusCompras != null)
                {
                    return localizaStatusCompras;
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

        public async Task<IList<EPIComprasDTO>> getTodasCompras()
        {
            try
            {
                var localizaTodasCompras = await _compras.getTodasCompras();

                if (localizaTodasCompras != null)
                {
                    return localizaTodasCompras;
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

        public async Task<EPIComprasDTO> Insert(EPIComprasDTO compra)
        {
            try
            {
                var insereCompra = await _compras.Insert(compra);

                if (insereCompra != null)
                {
                    return insereCompra;
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

        public Task<EPIComprasDTO> Update(EPIComprasDTO compra)
        {
            try
            {
                var atualizaCompra = _compras.Update(compra);

                if (atualizaCompra != null)
                {
                    return atualizaCompra;
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
