using System.Collections.Generic;
using System.Threading.Tasks;
using ControleEPI.DTO;

namespace ControleEPI.BLL
{
    public interface IRHCargosBLL
    {
        Task<IEnumerable<RHCargosDTO>> getCargos();
        Task<RHCargosDTO> getCargo(int Id);
    }
}
