using System;

namespace Vestimenta.DTO
{
    public class VestLogDTO
    {
        public int id { get; set; }
        public DateTime data { get; set; }
        public int idUsuario { get; set; }
        public int idItem { get; set; }
        public int quantidadeAnt { get; set; }
        public int quantidadeDep { get; set; }
        public string tamanho { get; set; }
        public string usado { get; set; }
    }
}
