using System.Collections.Generic;
using System.Threading.Tasks;
using RH.DTO;

namespace RH.BLL.RHCargos
{
    public interface IRHCargosBLL
    {
        Task<IEnumerable<RHCargosDTO>> getCargos();
        Task<RHCargosDTO> getCargo(int Id);
    }
}
