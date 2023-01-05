using System.Collections.Generic;
using System.Threading.Tasks;
using ControleEPI.DTO;

namespace ControleEPI.BLL
{
    public interface IEPIComprasBLL
    {
        Task<EPIComprasDTO> Insert(EPIComprasDTO compra);
        Task<EPIComprasDTO> getCompra(int Id);
        Task<IList<EPIComprasDTO>> getCompras(string status);
        Task<IList<EPIComprasDTO>> getStatusCompras(int status);
        Task Update(EPIComprasDTO compra);
        Task Delete(int id);
    }
}
