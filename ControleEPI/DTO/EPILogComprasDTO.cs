using System;

namespace ControleEPI.DTO
{
    public class EPILogComprasDTO
    {
        public int id { get; set; }
        public decimal valor { get; set; }
        public int idCompra { get; set; }
        public int idUsuario { get; set; }
        public DateTime dataCompra { get; set; }
    }
}
