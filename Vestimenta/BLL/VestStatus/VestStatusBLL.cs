using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Vestimenta.DAL.VestStatus;
using Vestimenta.DTO;

namespace Vestimenta.BLL.VestStatus
{
    public class VestStatusBLL : IVestStatusBLL
    {
        private readonly IVestStatusDAL _status;

        public VestStatusBLL(IVestStatusDAL status)
        {
            _status = status;
        }

        public async Task<VestStatusDTO> Delete(int id)
        {
            try
            {
                var localizaStatus = await _status.getStatus(id);

                if (localizaStatus == null)
                {
                    var deletaStatus = await _status.Delete(localizaStatus.id);

                    if (deletaStatus != null)
                    {
                        return localizaStatus;
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

        public async Task<VestStatusDTO> getStatus(int Id)
        {
            try
            {
                var status = await _status.getStatus(Id);

                if (status == null)
                {
                    return status;
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

        public async Task<IList<VestStatusDTO>> getTodosStatus()
        {
            try
            {
                var status = await _status.getTodosStatus();

                if (status != null)
                {
                    return status;
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

        public async Task<VestStatusDTO> Insert(VestStatusDTO status)
        {
            try
            {                
                var checkStatus = await _status.getNomeStatus(status.nome);

                if (checkStatus != null)
                {
                    return null;
                }
                else
                {
                    var novoStatus = await _status.Insert(status);

                    if (novoStatus != null)
                    {
                        return novoStatus;
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

        public async Task<VestStatusDTO> Update(VestStatusDTO status)
        {
            try
            {
                var atualizaStatusVestimenta = await _status.Update(status);

                if (atualizaStatusVestimenta != null)
                {
                    return atualizaStatusVestimenta;
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
