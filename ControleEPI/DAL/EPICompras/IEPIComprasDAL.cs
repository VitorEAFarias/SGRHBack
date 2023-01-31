using ControleEPI.DTO;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ControleEPI.DAL.EPICompras
{
    public interface IEPIComprasDAL
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
