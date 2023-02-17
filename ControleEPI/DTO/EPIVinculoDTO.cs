using System;

namespace ControleEPI.DTO
{
    public class EPIVinculoDTO
    {
        public int id { get; set; }
        public int idUsuario { get; set; }
        public int idItem { get; set; }
        public int idTamanho { get; set; }
        public int idPedido { get; set; }
        public DateTime dataVinculo { get; set; }
        public int status { get; set; }
        public DateTime dataDevolucao { get; set; }
        public DateTime validade { get; set; }
    }

    public class VinculoDTO
    {
        public int idVinculo { get; set; }
        public string certificado { get; set; }
        public string categoria { get; set; }
        public int idUsuario { get; set; }
        public string nomeUsuario { get; set; }
        public int idPedido { get; set; }
        public int idItem { get; set; }
        public string nomeItem { get; set; }
        public int idTamanho { get; set; }
        public string tamanho { get; set; }
        public DateTime? dataVinculo { get; set; }
        public DateTime? dataDevolucao { get; set; }
        public int idStatus { get; set; }
        public string status { get; set; }
        public DateTime? validade { get; set; }
    }
}
