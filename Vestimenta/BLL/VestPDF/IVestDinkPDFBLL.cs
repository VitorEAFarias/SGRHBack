using System.Collections.Generic;
using System.Threading.Tasks;
using Vestimenta.DTO;

namespace Vestimenta.BLL.VestPDF
{
    public interface IVestDinkPDFBLL
    {
        Task<string> dadosPDF(int idUsuario);
    }
}
