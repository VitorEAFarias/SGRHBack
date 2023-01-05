using System.Collections.Generic;
using System.Threading.Tasks;
using Vestimenta.DTO;

namespace Vestimenta.BLL
{
    public interface IVestimentaBLL
    {
        Task<VestVestimentaDTO> Insert(VestVestimentaDTO vestimenta);
        Task<VestVestimentaDTO> getVestimenta(int Id);
        Task<VestVestimentaDTO> getNomeVestimenta(string Nome);
        Task<IList<VestVestimentaDTO>> getVestimentas();
        Task<IList<VestVestimentaDTO>> getItens(int idVestimenta);
        Task<VestVestimentaDTO> Update(VestVestimentaDTO vestimenta);
        Task Delete(int id);
    }
}
