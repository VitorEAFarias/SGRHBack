using RH.DTO;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RH.DAL.RHContratos
{
    public interface IRHEmpContratosDAL
    {
        Task<IEnumerable<RHEmpContratosDTO>> getContratos();
        Task<RHEmpContratosDTO> getContrato(int Id);
        Task<RHEmpContratosDTO> getEmpContrato(int Id);
    }
}
