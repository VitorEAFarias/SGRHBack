using ControleEPI.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ControleEPI.DAL.EPIStatus
{
    public interface IEPIStatusDAL
    {
        Task<EPIStatusDTO> getStatus(int Id);
    }
}
