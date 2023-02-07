using System.Collections.Generic;
using System.Threading.Tasks;
using Vestimenta.DTO;

namespace Vestimenta.DAL.VestLog
{
    public interface IVestLogDAL
    {
        Task<VestLogDTO> Insert(VestLogDTO log);
        Task<VestLogDTO> getLog(int Id);
        Task<IList<VestLogDTO>> getLogs();
        Task Update(VestLogDTO log);
        Task Delete(int id);
    }
}
