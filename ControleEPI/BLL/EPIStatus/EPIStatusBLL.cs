using ControleEPI.DAL.EPIStatus;
using ControleEPI.DTO;
using System;
using System.Threading.Tasks;

namespace ControleEPI.BLL.EPIStatus
{
    public class EPIStatusBLL : IEPIStatusBLL
    {
        private readonly IEPIStatusDAL _status;

        public EPIStatusBLL(IEPIStatusDAL status)
        {
            _status = status;
        }

        public async Task<EPIStatusDTO> getStatus(int Id)
        {
            try
            {
                var localizaStatus = await _status.getStatus(Id);

                if (localizaStatus != null)
                {
                    return localizaStatus;
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
