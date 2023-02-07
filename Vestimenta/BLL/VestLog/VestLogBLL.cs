using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Vestimenta.DTO;

namespace Vestimenta.BLL.VestLog
{
    public class VestLogBLL : IVestLogBLL
    {
        public Task Delete(int id)
        {
            throw new NotImplementedException();
        }

        public Task<VestLogDTO> getLog(int Id)
        {
            throw new NotImplementedException();
        }

        public Task<IList<VestLogDTO>> getLogs()
        {
            throw new NotImplementedException();
        }

        public Task<VestLogDTO> Insert(VestLogDTO log)
        {
            throw new NotImplementedException();
        }

        public Task Update(VestLogDTO log)
        {
            throw new NotImplementedException();
        }
    }
}
