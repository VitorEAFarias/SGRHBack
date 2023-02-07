using System.Collections.Generic;
using System.Threading.Tasks;
using Vestimenta.DTO;

namespace Vestimenta.BLL.VestVestimenta
{
    public interface IVestVestimentaBLL
    {
        Task<VestVestimentaDTO> Insert(VestVestimentaDTO vestimenta);
        Task<VestVestimentaDTO> ativaVestimenta(int Id);
        Task<TamanhosRam> getVestimenta(int Id);
        Task<VestVestimentaDTO> desativaVestimenta(int id, int status);
        Task<VestVestimentaDTO> getNomeVestimenta(VestVestimentaDTO vestimenta);
        Task<IList<TamanhoTotalDTO>> getVestimentas();
        Task<IList<VestVestimentaDTO>> getItens(int idVestimenta);
        Task<VestVestimentaDTO> Update(VestVestimentaDTO vestimenta);
        Task Delete(int id);
    }
}
