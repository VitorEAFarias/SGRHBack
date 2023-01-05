using System.Collections.Generic;
using System.Threading.Tasks;
using Vestimenta.DTO;

namespace Vestimenta.BLL
{
    public interface IStatusVestBLL
    {
        Task<VestStatusDTO> Insert(VestStatusDTO status);
        Task<VestStatusDTO> getStatus(int Id);
        Task<VestStatusDTO> getNomeStatus(string nome);
        Task<IList<VestStatusDTO>> getTodosStatus();
        Task Update(VestStatusDTO status);
        Task Delete(int id);
    }
}
