using ControleEPI.DTO;
using System.Threading.Tasks;

namespace ControleEPI.BLL.EPILogCompras
{
    public interface IEPILogComprasBLL
    {
        Task<EPILogComprasDTO> insereLogCompra(EPILogComprasDTO logCompras);
    }
}
