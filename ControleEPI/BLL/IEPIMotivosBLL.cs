using System.Collections.Generic;
using System.Threading.Tasks;
using ControleEPI.DTO;

namespace ControleEPI.BLL
{
    public interface IEPIMotivosBLL
    {
        Task<IEnumerable<EPIMotivoDTO>> getMotivos();
        Task<EPIMotivoDTO> getMotivo(int Id);
    }
}
