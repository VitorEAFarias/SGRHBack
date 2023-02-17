using ControleEPI.DTO;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ControleEPI.DAL.EPIHistorico
{
    public interface IEPIHistoricoDAL
    {
        Task<IList<EPIVinculoDTO>> dadosPDF(int idUsuario);
    }
}
