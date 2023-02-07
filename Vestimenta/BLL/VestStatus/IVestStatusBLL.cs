using System.Collections.Generic;
using System.Threading.Tasks;
using Vestimenta.DTO;

namespace Vestimenta.BLL.VestStatus
{
    public interface IVestStatusBLL
    {
        Task<VestStatusDTO> Insert(VestStatusDTO status);
        Task<VestStatusDTO> getStatus(int Id);
        Task<VestStatusDTO> getNomeStatus(string nome);
        Task<IList<VestStatusDTO>> getTodosStatus();
        Task<VestStatusDTO> Update(VestStatusDTO status);
        Task<VestStatusDTO> Delete(int id);
    }
}
