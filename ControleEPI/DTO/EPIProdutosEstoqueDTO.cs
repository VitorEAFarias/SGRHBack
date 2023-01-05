namespace ControleEPI.DTO
{    
    public class EPIProdutosEstoqueDTO
    {        
        public int id { get; set; }
        public int idProduto { get; set; }
        public int quantidade { get; set; }
        public int idTamanho { get; set; }
                
        public string ativo { get; set; }

        public EPIProdutosEstoqueDTO(string ativo = "S")
        {
            this.ativo = ativo;
        }
    }
}
