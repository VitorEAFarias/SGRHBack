using System.Collections.Generic;
using System.Threading.Tasks;
using Vestimenta.DTO;

namespace Vestimenta.BLL.VestCompras
{
    public interface IVestComprasBLL
    {
        Task<VestComprasDTO> Insert(VestComprasDTO compra);
        Task<VestComprasDTO> processoDeCompra(VestComprasDTO processoCompra);
        Task<VestComprasDTO> reprovarCompra(VestComprasDTO reprovarCompra);
        Task<VestComprasDTO> comprarItem(VestComprasDTO comprarItens);
        Task<RetornoCompraDTO> getCompra(int Id);
        Task<IList<ComprasDTO>> getCompras();
        Task<IList<VestComprasDTO>> localizaProcessoCompra();
        Task Update(VestComprasDTO compra);
        Task Delete(int id);
    }
}
