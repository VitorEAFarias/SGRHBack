using System.Collections.Generic;
using System.Threading.Tasks;
using Vestimenta.DTO;

namespace Vestimenta.DAL.VestPDF
{
    public interface IVestDinkPDFDAL
    {
        Task<IList<VestVinculoDTO>> dadosPDF(int idUsuario);
    }
}
