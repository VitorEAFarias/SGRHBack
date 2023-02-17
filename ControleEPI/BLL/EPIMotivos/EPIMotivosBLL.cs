using ControleEPI.DAL.EPIMotivos;
using ControleEPI.DTO;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;

namespace ControleEPI.BLL.EPIMotivos
{
    public class EPIMotivosBLL : IEPIMotivosBLL
    {
        private readonly IEPIMotivosDAL _motivo;

        public EPIMotivosBLL(IEPIMotivosDAL motivo)
        {
            _motivo = motivo;
        }

        public async Task<EPIMotivoDTO> insereMotivo(EPIMotivoDTO motivo)
        {
            try
            {
                var verificaMotivo = await _motivo.verificaNome(motivo.nome);

                if (verificaMotivo != null)
                {
                    return null;
                }
                else
                {
                    var insereMotivo = await _motivo.insereMotivo(motivo);

                    if (insereMotivo != null)
                    {
                        return insereMotivo;
                    }
                    else
                    {
                        return null;
                    }                    
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
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
                    return localizaMotivos.OrderBy(x => x.nome);
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

        public async Task<EPIMotivoDTO> atualizaMotivo(EPIMotivoDTO motivo)
        {
            try
            {
                var localizaMotivo = await _motivo.getMotivo(motivo.id);               

                if (localizaMotivo != null)
                {
                    var verificaNome = await _motivo.verificaNome(motivo.nome);

                    if (verificaNome == null)
                    {
                        var atualizaMotivo = await _motivo.atualizaMotivo(motivo);

                        if (atualizaMotivo != null)
                        {
                            return atualizaMotivo;
                        }
                        else
                        {
                            return null;
                        }
                    }
                    else
                    {                        
                        return null;                        
                    }
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
