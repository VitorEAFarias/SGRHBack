using ControleEPI.DAL.EPITamanhos;
using ControleEPI.DTO;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ControleEPI.BLL.EPITamanhos
{
    public class EPITamanhosBLL : IEPITamanhosBLL
    {
        private readonly IEPITamanhosDAL _tamanho;

        public EPITamanhosBLL(IEPITamanhosDAL tamanho)
        {
            _tamanho = tamanho;
        }

        public async Task<EPITamanhosDTO> Delete(int id)
        {
            try
            {
                var deletaTamanho = await _tamanho.Delete(id);

                if (deletaTamanho != null)
                {
                    return deletaTamanho;
                }
                else
                {
                    return null;
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<EPITamanhosDTO> insereTamanho(EPITamanhosDTO tamanho)
        {
            try
            {
                var insereTamanho = await _tamanho.insereTamanho(tamanho);

                if (insereTamanho != null)
                {
                    return insereTamanho;
                }
                else
                {
                    return null;
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<EPITamanhosDTO> localizaTamanho(int Id)
        {
            try
            {
                var localizaTamanho = await _tamanho.localizaTamanho(Id);

                if (localizaTamanho != null)
                {
                    return localizaTamanho;
                }
                else
                {
                    return null;
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<IList<EPITamanhosDTO>> localizaTamanhos()
        {
            try
            {
                var localizaTamanhos = await _tamanho.localizaTamanhos();

                if (localizaTamanhos != null)
                {
                    return localizaTamanhos;
                }
                else
                {
                    return null;
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<EPITamanhosDTO> Update(EPITamanhosDTO tamanho)
        {
            try
            {
                var atualizaTamanho = await _tamanho.Update(tamanho);

                if (atualizaTamanho != null)
                {
                    return atualizaTamanho;
                }
                else
                {
                    return null;
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public Task<EPITamanhosDTO> verificaTamanho(string nome)
        {
            throw new NotImplementedException();
        }
    }
}
