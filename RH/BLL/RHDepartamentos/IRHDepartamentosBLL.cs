using System.Collections.Generic;
using System.Threading.Tasks;
using RH.DTO;

namespace RH.BLL.RHDepartamentos
{
    public interface IRHDepartamentosBLL
    {
        Task<IEnumerable<RHDepartamentosDTO>> getDepartamentos();
        Task<RHDepartamentosDTO> getDepartamento(int Id);
    }
}
