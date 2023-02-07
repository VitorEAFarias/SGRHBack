using RH.DTO;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RH.DAL.RHCargos
{
    public interface IRHCargosDAL
    {
        Task<IEnumerable<RHCargosDTO>> getCargos();
        Task<RHCargosDTO> getCargo(int Id);
    }
}
