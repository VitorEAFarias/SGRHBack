using System.Collections.Generic;
using System.Threading.Tasks;
using ControleEPI.DTO;

namespace ControleEPI.BLL.RHDepartamentos
{
    public interface IRHDepartamentosBLL
    {
        Task<IEnumerable<RHDepartamentosDTO>> getDepartamentos();
        Task<RHDepartamentosDTO> getDepartamento(int Id);
    }
}
