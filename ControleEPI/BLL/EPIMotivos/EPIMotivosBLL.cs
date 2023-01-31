using ControleEPI.DAL.EPIMotivos;
using ControleEPI.DTO;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ControleEPI.BLL.EPIMotivos
{
    public class EPIMotivosBLL : IEPIMotivosBLL
    {
        private readonly IEPIMotivosDAL _motivo;

        public EPIMotivosBLL(IEPIMotivosDAL motivo)
        {
            _motivo = motivo;
        }

        public async Task<EPIMotivoDTO> getMotivo(int Id)
        {
            try
            {
                var localizaMotivo = await _motivo.getMotivo(Id);

                if (localizaMotivo != null)
                {
                    return localizaMotivo;
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

        public async Task<IEnumerable<EPIMotivoDTO>> getMotivos()
        {
            try
            {
                var localizaMotivos = await _motivo.getMotivos();

                if (localizaMotivos != null)
                {
                    return localizaMotivos;
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
    }
}
