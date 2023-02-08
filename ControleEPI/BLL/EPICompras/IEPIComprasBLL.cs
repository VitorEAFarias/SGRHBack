using System.Collections.Generic;
using System.Threading.Tasks;
using ControleEPI.DTO;

namespace ControleEPI.BLL.EPICompras
{
    public interface IEPIComprasBLL
    {
        Task<ComprasDTO> getCompra(int Id);
        Task<IList<ComprasDTO>> getCompras(string status);
        Task<EPIComprasDTO> efetuarCompra(EPIComprasDTO compra);
        Task<EPIComprasDTO> reprovaCompra(EPIComprasDTO compra);
    }
}
