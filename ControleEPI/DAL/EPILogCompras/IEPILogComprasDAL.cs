using ControleEPI.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ControleEPI.DAL.EPILogCompras
{
    public interface IEPILogComprasDAL
    {
        Task<EPILogComprasDTO> insereLogCompra(EPILogComprasDTO logCompras);
    }
}
