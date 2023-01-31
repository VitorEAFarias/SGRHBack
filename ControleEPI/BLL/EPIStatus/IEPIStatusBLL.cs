using System.Threading.Tasks;
using ControleEPI.DTO;

namespace ControleEPI.BLL.EPIStatus
{
    public interface IEPIStatusBLL
    {
        Task<EPIStatusDTO> getStatus(int Id);
    }
}
