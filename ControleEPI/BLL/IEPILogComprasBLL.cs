using ControleEPI.DTO;
using System.Threading.Tasks;

namespace ControleEPI.BLL
{
    public interface IEPILogComprasBLL
    {
        Task<EPILogComprasDTO> insereLogCompra(EPILogComprasDTO logCompras);
    }
}
