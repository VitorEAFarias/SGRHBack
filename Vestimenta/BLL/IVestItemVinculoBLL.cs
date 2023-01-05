using System.Collections.Generic;
using System.Threading.Tasks;
using Vestimenta.DTO;

namespace Vestimenta.BLL
{
    public interface IVestItemVinculoBLL
    {
        Task<VestItemVinculoDTO> Insert(VestItemVinculoDTO itemVinculo);
        Task<VestItemVinculoDTO> getItemVinculo(int Id);        
        Task<IList<VestItemVinculoDTO>> getItensVinculo();
        Task<IList<VestItemVinculoDTO>> getItensPedido(int idPedido);
        Task Update(VestItemVinculoDTO itemVinculo);
        Task Delete(int id);
    }
}
