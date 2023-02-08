using System.Collections.Generic;
using System.Threading.Tasks;
using Vestimenta.DTO;

namespace Vestimenta.BLL.VestVestimenta
{
    public interface IVestVestimentaBLL
    {
        Task<TamanhosRam> getVestimenta(int Id);
        Task<VestVestimentaDTO> desativaVestimenta(int id, int status);
        Task<VestVestimentaDTO> getNomeVestimenta(VestVestimentaDTO vestimenta);
        Task<IList<TamanhoTotalDTO>> getVestimentas();
        Task<VestVestimentaDTO> Update(VestVestimentaDTO vestimenta);
    }
}
