namespace ControleEPI.DTO.FromBody
{
    public class CertificadoProdutoDTO
    {
        public int id { get; set; }
        public string nomeProduto { get; set; }
        public string categoria { get; set; }
        public int idCertificado { get; set; }
        public string ca { get; set; }
        public decimal preco { get; set; }
        public string ativo { get; set; }
        public int validadeEmUso { get; set; }
    }
}
