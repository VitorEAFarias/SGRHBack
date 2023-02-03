using System;

namespace ControleEPI.DTO
{
    public class EPIPedidosAprovadosDTO
    {
        public int id { get; set; }
        public int idProduto { get; set; }
        public int idPedido { get; set; }
        public int idTamanho { get; set; }
        public int quantidade { get; set; }
        public string enviadoCompra { get; set; }
        public string liberadoVinculo { get; set; }
    }

    public class PedidosAprovadosDTO
    {
        public int idProdutoAprovado { get; set; }
        public string enviadoCompra { get; set; }
        public int idPedido { get; set; }
        public int idProduto { get; set; }
        public string nome { get; set; }
        public int idTamanho { get; set; }
        public string tamanho { get; set; }
        public int quantidade { get; set; }
        public int idUsuario { get; set; }
        public string usuario { get; set; }
        public DateTime? dataPedido { get; set; }
        public int estoque { get; set; }
        public string liberadoVinculo { get; set; }
    }
}
