namespace ControleEPI.DTO
{
    public class EPIProdutosDTO
    {
        public int id { get; set; }
        public string nome { get; set; }
        public int idCategoria { get; set; }
        public decimal preco { get; set; }
        public int idCertificadoAprovacao { get; set; }
        public int validadeEmUso { get; set; }
        public string ativo { get; set; }
        public byte[] foto { get; set; }
        public int maximo { get; set; }
    }
}
