using System;

namespace Vestimenta.DTO
{
    public class VestRepositorioDTO
    {
        public int id { get; set; }
        public int idItem { get; set; }
        public int idPedido { get; set; }        
        public string enviadoCompra { get; set; }
        public DateTime dataAtualizacao { get; set; }
        public string tamanho { get; set; }
        public int quantidade { get; set; }
        public string ativo { get; set; }

        public VestRepositorioDTO(string ativo = "Y")
        {
            this.ativo = ativo;
        }
    }    
}
