using RH.DTO;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RH.DAL.RHDepartamentos
{
    public interface IRHDepartamentosDAL
    {
        Task<IEnumerable<RHDepartamentosDTO>> getDepartamentos();
        Task<RHDepartamentosDTO> getDepartamento(int Id);
    }
}
