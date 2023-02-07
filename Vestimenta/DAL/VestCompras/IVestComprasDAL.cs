using System.Collections.Generic;
using System.Threading.Tasks;
using Vestimenta.DTO;

namespace Vestimenta.DAL.VestCompras
{
    public interface IVestComprasDAL
    {
        Task<VestComprasDTO> Insert(VestComprasDTO compra);
        Task<VestComprasDTO> getCompra(int Id);
        Task<IList<VestComprasDTO>> getCompras();
        Task<IList<VestComprasDTO>> localizaProcessoCompra();
        Task Update(VestComprasDTO compra);
        Task Delete(int id);
    }
}
