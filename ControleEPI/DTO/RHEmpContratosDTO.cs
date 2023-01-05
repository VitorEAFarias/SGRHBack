namespace ControleEPI.DTO
{
    public class RHEmpContratosDTO
    {
        public int id { get; set; }
        public int id_empregado { get; set; }
        public int id_departamento { get; set; }
        public int id_cargo { get; set; }
        public int id_empregado_superior { get; set; }
        public int contrato_atual { get; set; }
        public int contrato_principal { get; set; }
    }
}
