using System;

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

    public class TodosProdutosEstoqueDTO
    {
        public int id { get; set; }
        public int quantidade { get; set; }
        public int idTamanho{ get; set; }
        public string tamanho { get; set; }
        public int idProduto { get; set; }
        public string produto { get; set; }
        public decimal preco { get; set; }
        public string certificado { get; set; }
        public DateTime validadeCertificado { get; set; }
        public string ativo { get; set; }
    }
}
