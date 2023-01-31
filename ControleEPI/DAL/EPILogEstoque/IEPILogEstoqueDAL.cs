using ControleEPI.DTO;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ControleEPI.DAL.EPILogEstoque
{
    public interface IEPILogEstoqueDAL
    {
        Task<EPILogEstoqueDTO> Insert(EPILogEstoqueDTO logEstoque);
        Task<EPILogEstoqueDTO> GetLogEstoque(int Id);
        Task<IEnumerable<EPILogEstoqueDTO>> GetLogsEstoque();
    }
}
