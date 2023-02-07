using System.Collections.Generic;
using System.Threading.Tasks;
using Vestimenta.DTO;

namespace Vestimenta.DAL.VestVestimenta
{
    public interface IVestVestimentaDAL
    {
        Task<VestVestimentaDTO> Insert(VestVestimentaDTO vestimenta);
        Task<VestVestimentaDTO> getVestimenta(int Id);
        Task<VestVestimentaDTO> getNomeVestimenta(string Nome);
        Task<IList<VestVestimentaDTO>> getVestimentas();
        Task<IList<VestVestimentaDTO>> getItens(int idVestimenta);
        Task<VestVestimentaDTO> Update(VestVestimentaDTO vestimenta);
        Task Delete(int id);
    }
}
