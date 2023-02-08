using System.Threading.Tasks;

namespace Vestimenta.BLL.VestPDF
{
    public interface IVestDinkPDFBLL
    {
        Task<string> dadosPDF(int idUsuario);
        Task<string> relatorioCompra(int idCompra);
    }
}
