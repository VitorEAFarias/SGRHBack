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
    }
}
