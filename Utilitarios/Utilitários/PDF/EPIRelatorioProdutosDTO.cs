using System;

namespace Utilitarios.Utilitários.PDF
{
    public class EPIRelatorioProdutosDTO
    {
        public int numeroPedido { get; set; }
        public DateTime dataPedido { get; set; }
        public string colaborador { get; set; }
        public string departamento { get; set; }
        public string produto { get; set; }
        public string tamanho { get; set; }
        public int quantidade { get; set; }
    }
}
