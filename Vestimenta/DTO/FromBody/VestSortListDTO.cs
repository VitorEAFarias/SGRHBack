namespace Vestimenta.DTO.FromBody
{
    public class VestSortListDTO
    {
        public int id { get; set; }
        public int idItem { get; set; }
        public string nome { get; set; }
        public decimal preco { get; set; }
        public decimal precoTotal { get; set; }
        public int idPedido { get; set; }
        public string tamanho { get; set; }
        public int quantidade { get; set; }       
    }
}
