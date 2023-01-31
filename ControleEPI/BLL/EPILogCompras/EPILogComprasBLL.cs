using ControleEPI.DAL.EPILogCompras;
using ControleEPI.DTO;
using System;
using System.Threading.Tasks;

namespace ControleEPI.BLL.EPILogCompras
{
    public class EPILogComprasBLL : IEPILogComprasBLL
    {
        private readonly IEPILogComprasDAL _logCompras;

        public EPILogComprasBLL(IEPILogComprasDAL logCompras)
        {
            _logCompras = logCompras;
        }

        public async Task<EPILogComprasDTO> insereLogCompra(EPILogComprasDTO logCompras)
        {
            try
            {
                var insereLogCompras = await _logCompras.insereLogCompra(logCompras);

                if (insereLogCompras != null)
                {
                    return insereLogCompras;
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
