using System;

namespace ControleEPI.DTO
{
    public class EPICertificadoAprovacaoDTO
    {
        public int id { get; set; }
        public string numero { get; set; }
        public DateTime validade { get; set; }
        public string ativo { get; set; }
    }
}
