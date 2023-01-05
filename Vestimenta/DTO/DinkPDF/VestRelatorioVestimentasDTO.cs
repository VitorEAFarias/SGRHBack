using System;

namespace Vestimenta.DTO.DinkPDF
{    
    public class VestRelatorioVestimentasDTO
    {
        public int numeroPedido { get; set; }
        public DateTime dataPedido { get; set; }
        public string colaborador { get; set; }
        public string departamento { get; set; }
        public string vestimenta { get; set; }
        public string tamanho { get; set; }
        public int quantidade { get; set; }
    }
}
