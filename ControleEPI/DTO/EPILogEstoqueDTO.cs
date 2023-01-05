using System;

namespace ControleEPI.DTO
{
    public class EPILogEstoqueDTO
    {
        public int id { get; set; }
        public int idProduto { get; set; }
        public int idUsuario { get; set; }
        public int de { get; set; }
        public int para { get; set; }
        public int quantidadeMovimentada { get; set; }
        public DateTime dataAlteracao { get; set; }
        public bool retirada { get; set; }
        public bool automatico { get; set; }
    }
}
