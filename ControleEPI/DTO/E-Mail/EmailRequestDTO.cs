using System.Collections.Generic;
using Vestimenta.DTO.FromBody;

namespace ControleEPI.DTO.E_Mail
{
    public class EmailRequestDTO 
    {
        public string EmailDe { get; set; }
        public string EmailPara { get; set; }
        public string Assunto { get; set; }
        public VestConteudoEmailColaboradorDTO ConteudoColaborador { get; set; }
        public List<VestConteudoEmailDTO> Conteudo { get; set; }
    }
}
