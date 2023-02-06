using System.Collections.Generic;
using System.Threading.Tasks;
using ControleEPI.DTO;

namespace ControleEPI.BLL.EPICompras
{
    public interface IEPIComprasBLL
    {
        Task<EPIComprasDTO> Insert(EPIComprasDTO compra);
        Task<ComprasDTO> getCompra(int Id);
        Task<IList<EPIComprasDTO>> getTodasCompras();
        Task<IList<ComprasDTO>> getCompras(string status);
        Task<IList<EPIComprasDTO>> getStatusCompras(int status);
        Task<EPIComprasDTO> efetuarCompra(EPIComprasDTO compra);
        Task<EPIComprasDTO> reprovaCompra(EPIComprasDTO compra);
        Task<EPIComprasDTO> Delete(int id);
    }
}
