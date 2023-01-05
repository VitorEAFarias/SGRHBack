using System;

namespace Vestimenta.DTO
{
    public class VestVinculoDTO
    {
        public int id { get; set; }
        public int idUsuario { get; set; }
        public int idVestimenta { get; set; }
        public DateTime dataVinculo { get; set; }
        public int status { get; set; }
        public string tamanhoVestVinculo { get; set; }
        public string usado { get; set; }
        public DateTime dataDesvinculo { get; set; }
        public string statusAtual { get; set; }
        public int idPedido { get; set; }
        public int quantidade { get; set; }
    }
}
