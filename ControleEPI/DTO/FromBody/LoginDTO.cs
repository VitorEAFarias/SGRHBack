namespace ControleEPI.DTO.FromBody
{
    public class LoginDTO
    {
        public int id { get; set; }
        public string cpf { get; set; }
        public string senha { get; set; }
        public string usuario { get; set; }
        public string email { get; set; }
        public bool adm{ get; set; }
        public bool comprador { get; set; }
    }
}
