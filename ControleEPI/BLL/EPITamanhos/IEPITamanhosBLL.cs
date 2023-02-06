using System.Collections.Generic;
using System.Threading.Tasks;
using ControleEPI.DTO;

namespace ControleEPI.BLL.EPITamanhos
{
    public interface IEPITamanhosBLL
    {
        Task<EPITamanhosDTO> insereTamanho(EPITamanhosDTO tamanho);
        Task<TamanhosDTO> localizaTamanho(int Id);
        Task<EPITamanhosDTO> verificaTamanho(string nome);
        Task<IList<TamanhosDTO>> localizaTamanhos();
        Task<IList<EPITamanhosDTO>> tamanhosCategoria(int idCategoria);
        Task<EPITamanhosDTO> Update(TamanhosDTO tamanho);
        Task<EPITamanhosDTO> Delete(int id);
    }
}
