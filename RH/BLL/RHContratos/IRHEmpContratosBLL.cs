using System.Collections.Generic;
using System.Threading.Tasks;
using RH.DTO;

namespace RH.BLL.RHContratos
{
    public interface IRHEmpContratosBLL
    {
        Task<IEnumerable<RHEmpContratosDTO>> getContratos();
        Task<RHEmpContratosDTO> getContrato(int Id);
        Task<RHEmpContratosDTO> getEmpContrato(int Id);
    }
}
