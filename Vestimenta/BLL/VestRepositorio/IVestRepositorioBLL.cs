using System.Collections.Generic;
using System.Threading.Tasks;
using Vestimenta.DTO;
using Vestimenta.DTO.FromBody;

namespace Vestimenta.BLL.VestRepositorio
{
    public interface IVestRepositorioBLL
    {
        Task<VestRepositorioDTO> Insert(VestRepositorioDTO repo);
        Task<VestRepositorioDTO> getRepositorio(int Id);
        Task<IList<VestSortListDTO>> getRepositorioStatus(string status);
    }
}
