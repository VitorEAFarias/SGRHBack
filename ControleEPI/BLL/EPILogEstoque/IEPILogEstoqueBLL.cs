using System.Collections.Generic;
using System.Threading.Tasks;
using ControleEPI.DTO;

namespace ControleEPI.BLL.EPILogEstoque
{
    public interface IEPILogEstoqueBLL
    {
        Task<EPILogEstoqueDTO> Insert(EPILogEstoqueDTO logEstoque);
        Task<EPILogEstoqueDTO> GetLogEstoque(int Id);
        Task<IEnumerable<EPILogEstoqueDTO>> GetLogsEstoque();
    }
}
