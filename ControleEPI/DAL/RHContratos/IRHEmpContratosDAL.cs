using ControleEPI.DTO;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ControleEPI.DAL.RHContratos
{
    public interface IRHEmpContratosDAL
    {
        Task<IEnumerable<RHEmpContratosDTO>> getContratos();
        Task<RHEmpContratosDTO> getContrato(int Id);
        Task<RHEmpContratosDTO> getEmpContrato(int Id);
    }
}
