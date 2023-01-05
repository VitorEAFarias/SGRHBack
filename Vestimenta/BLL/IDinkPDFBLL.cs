using System.Collections.Generic;
using System.Threading.Tasks;
using Vestimenta.DTO;

namespace Vestimenta.BLL
{
    public interface IDinkPDFBLL
    {
        Task<IList<VestVinculoDTO>> dadosPDF(int idUsuario);
    }
}
