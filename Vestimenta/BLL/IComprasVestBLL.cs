using System.Collections.Generic;
using System.Threading.Tasks;
using Vestimenta.DTO;

namespace Vestimenta.BLL
{
    public interface IComprasVestBLL
    {
        Task<VestComprasDTO> Insert(VestComprasDTO compra);
        Task<VestComprasDTO> getCompra(int Id);
        Task<IList<VestComprasDTO>> getCompras();
        Task Update(VestComprasDTO compra);
        Task Delete(int id);
    }
}
