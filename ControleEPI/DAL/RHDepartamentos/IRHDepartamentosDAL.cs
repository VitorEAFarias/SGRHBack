using ControleEPI.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ControleEPI.DAL.RHDepartamentos
{
    public interface IRHDepartamentosDAL
    {
        Task<IEnumerable<RHDepartamentosDTO>> getDepartamentos();
        Task<RHDepartamentosDTO> getDepartamento(int Id);
    }
}
