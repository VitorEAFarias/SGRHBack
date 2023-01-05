using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Vestimenta.DTO
{
    public class VestEstoqueDTO
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int id { get; set; }
        public int idItem { get; set; }
        public int quantidade { get; set; }
        public string tamanho { get; set; }
        public DateTime dataAlteracao{ get; set; }
        public int quantidadeVinculado { get; set; }
        public int quantidadeUsado { get; set; }
        public string ativado { get; set; }
    }
}
