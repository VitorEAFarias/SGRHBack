using System.Threading.Tasks;

namespace ControleEPI.BLL.EPIHistorico
{
    public interface IEPIHistoricoBLL
    {
        Task<string> dadosPDF(int idUsuario);
        Task<string> relatorioCompra(int idCompra);
    }
}
