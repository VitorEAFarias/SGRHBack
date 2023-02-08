using ControleEPI.DTO;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ControleEPI.BLL.EPIPedidosAprovados
{
    public interface IEPIPedidosAprovadosBLL
    {
        Task<IList<EPIPedidosAprovadosDTO>> insereProdutosAprovados(List<EPIPedidosAprovadosDTO> produtoAprovado);
        Task<IList<EPIPedidosAprovadosDTO>> enviaParaCompras(List<EPIPedidosAprovadosDTO> enviaCompras, int idUsuario);
        Task<IList<PedidosAprovadosDTO>> getProdutosAprovados(string statusCompra, string statusVinculo);
        Task<IList<EPIPedidosAprovadosDTO>> atualizaVinculos(List<EPIPedidosAprovadosDTO> produtosAprovados);
    }
}
