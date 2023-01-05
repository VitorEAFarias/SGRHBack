using System;

namespace ControleEPI.DTO
{
    public class EPIVinculoDTO
    {
        public int id { get; set; }
        public int idUsuario { get; set; }
        public int idItem { get; set; }
        public DateTime dataVinculo { get; set; }
        public int status { get; set; }
        public DateTime dataDevolucao { get; set; }
        public DateTime validade { get; set; }
    }
}
