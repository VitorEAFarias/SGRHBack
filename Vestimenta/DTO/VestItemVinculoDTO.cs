using System;

namespace Vestimenta.DTO
{
    public class VestItemVinculoDTO
    {
        public int id { get; set; }
        public string nome { get; set; }
        public string tamanho { get; set; }
        public int quantidade { get; set; }
        public int status { get; set; }
        public DateTime dataAlteracao { get; set; }
        public int idItem { get; set; }
        public int idPedido { get; set; }
        public string usado { get; set; }
    }
}
