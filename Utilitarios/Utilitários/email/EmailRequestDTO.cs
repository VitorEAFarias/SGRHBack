using System.Collections.Generic;

namespace Utilitarios.Utilitários.email
{
    public class EmailRequestDTO
    {
        public string EmailDe { get; set; }
        public string EmailPara { get; set; }
        public string Assunto { get; set; }
        public ConteudoEmailColaboradorDTO ConteudoColaborador { get; set; }
        public List<ConteudoEmailDTO> Conteudo { get; set; }
    }
}
