using System.Collections.Generic;
using System.Threading.Tasks;
using ControleEPI.DTO;

namespace ControleEPI.BLL
{
    public interface IEPITamanhosBLL
    {
        Task<EPITamanhosDTO> insereTamanho(EPITamanhosDTO tamanho);
        Task<EPITamanhosDTO> localizaTamanho(int Id);
        Task<EPITamanhosDTO> verificaTamanho(string nome);
        Task<IList<EPITamanhosDTO>> localizaTamanhos();
        Task Update(EPITamanhosDTO tamanho);
        Task Delete(int id);
    }
}
