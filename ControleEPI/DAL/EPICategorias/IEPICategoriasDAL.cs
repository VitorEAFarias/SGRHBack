using ControleEPI.DTO;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ControleEPI.DAL.EPICategorias
{
    public interface IEPICategoriasDAL
    {
        Task<EPICategoriasDTO> Insert(EPICategoriasDTO categoria);
        Task<EPICategoriasDTO> getCategoria(int Id);
        Task<EPICategoriasDTO> verificaCategoria(string nome);
        Task<IList<EPICategoriasDTO>> getCategorias();
        Task<EPICategoriasDTO> Update(EPICategoriasDTO categoria);
        Task<EPICategoriasDTO> Delete(int id);
    }
}
