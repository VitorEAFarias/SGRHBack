using System.Collections.Generic;
using System.Threading.Tasks;
using ControleEPI.DTO;

namespace ControleEPI.BLL.EPICompras
{
    public interface IEPIComprasBLL
    {
        Task<EPIComprasDTO> Insert(EPIComprasDTO compra);
        Task<EPIComprasDTO> getCompra(int Id);
        Task<IList<EPIComprasDTO>> getTodasCompras();
        Task<IList<EPIComprasDTO>> getCompras(string status);
        Task<IList<EPIComprasDTO>> getStatusCompras(int status);
        Task<EPIComprasDTO> Update(EPIComprasDTO compra);
        Task<EPIComprasDTO> Delete(int id);
    }
}
