using System.Threading.Tasks;
using ControleEPI.DTO;

namespace ControleEPI.BLL
{
    public interface IEPIStatusBLL
    {
        Task<EPIStatusDTO> getStatus(int Id);
    }
}
