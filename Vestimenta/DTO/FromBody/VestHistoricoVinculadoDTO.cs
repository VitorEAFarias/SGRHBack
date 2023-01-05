using System;

namespace Vestimenta.DTO.FromBody
{
    public class VestHistoricoVinculadoDTO
    {
        public int idUsuario { get; set; }
        public int idVestimenta { get; set; }
        public string tamanho { get; set; }
        public string usado { get; set; }
        public DateTime dataVinculo { get; set; }
        public int quantidade { get; set; }
    }
}
