namespace RH.DTO
{
    public class RHEmpregadoDTO
    {
        public int id { get; set; }
        public string nome { get; set; }
        public int ativo { get; set; }
    }

    public class EmpregadoDTO
    {
        public int id { get; set; }
        public string nome { get; set; }
        public int ativo { get; set; }
        public string departamento { get; set; }
        public string cargo { get; set; }
    }
}
