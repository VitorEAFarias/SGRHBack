using ControleEPI.DTO;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ControleEPI.DAL.RHCargos
{
    public interface IRHCargosDAL
    {
        Task<IEnumerable<RHCargosDTO>> getCargos();
        Task<RHCargosDTO> getCargo(int Id);
    }
}
