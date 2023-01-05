using System.Collections.Generic;
using System.Threading.Tasks;
using ControleEPI.DTO;

namespace ControleEPI.BLL
{
    public interface IRHDepartamentosBLL
    {
        Task<IEnumerable<RHDepartamentosDTO>> getDepartamentos();
        Task<RHDepartamentosDTO> getDepartamento(int Id);
    }
}
