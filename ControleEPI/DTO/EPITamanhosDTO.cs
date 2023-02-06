namespace ControleEPI.DTO
{
    public class EPITamanhosDTO
    {
        public int id { get; set; }
        public string tamanho { get; set; }
        public int idCategoriaProduto { get; set; }
        public string ativo { get; set; }
    }

    public class TamanhosDTO
    {
        public int id { get; set; }
        public string tamanho { get; set; }
        public int idCategoriaProduto { get; set; }
        public string nome { get; set; }
        public string ativo { get; set; }
    }
}
