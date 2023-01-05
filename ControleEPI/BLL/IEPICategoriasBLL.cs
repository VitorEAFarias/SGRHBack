using ControleEPI.DTO;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ControleEPI.BLL
{
    public interface IEPICategoriasBLL
    {
        Task<EPICategoriasDTO> Insert(EPICategoriasDTO categoria);
        Task<EPICategoriasDTO> getCategoria(int Id);
        Task<EPICategoriasDTO> verificaCategoria(string nome);
        Task<IList<EPICategoriasDTO>> getCategorias();
        Task Update(EPICategoriasDTO categoria);
    }
}
