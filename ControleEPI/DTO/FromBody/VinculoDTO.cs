using System;

namespace ControleEPI.DTO.FromBody
{
    public class VinculoDTO
    {        
        public string nomeUsuario{ get; set; }
        public string nomeItem { get; set; }
        public DateTime? dataVinculo { get; set; }
        public DateTime? dataDevolucao { get; set; }
        public string status { get; set; }
        public DateTime? validade { get; set; }
    }
}
