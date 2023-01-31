using System.Collections.Generic;
using System.Threading.Tasks;
using ControleEPI.DTO;

namespace ControleEPI.BLL.EPITamanhos
{
    public interface IEPITamanhosBLL
    {
        Task<EPITamanhosDTO> insereTamanho(EPITamanhosDTO tamanho);
        Task<EPITamanhosDTO> localizaTamanho(int Id);
        Task<EPITamanhosDTO> verificaTamanho(string nome);
        Task<IList<EPITamanhosDTO>> localizaTamanhos();
        Task<EPITamanhosDTO> Update(EPITamanhosDTO tamanho);
        Task<EPITamanhosDTO> Delete(int id);
    }
}
