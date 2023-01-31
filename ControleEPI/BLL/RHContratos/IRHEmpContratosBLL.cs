using System.Collections.Generic;
using System.Threading.Tasks;
using ControleEPI.DTO;

namespace ControleEPI.BLL.RHContratos
{
    public interface IRHEmpContratosBLL
    {
        Task<IEnumerable<RHEmpContratosDTO>> getContratos();
        Task<RHEmpContratosDTO> getContrato(int Id);
        Task<RHEmpContratosDTO> getEmpContrato(int Id);
    }
}
