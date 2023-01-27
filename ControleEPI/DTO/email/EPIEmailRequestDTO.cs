using ControleEPI.DTO.email;
using System.Collections.Generic;

namespace ControleEPI.DTO.E_Mail
{
    public class EPIEmailRequestDTO 
    {
        public string EmailDe { get; set; }
        public string EmailPara { get; set; }
        public string Assunto { get; set; }
        public EPIConteudoEmailColaboradorDTO ConteudoColaborador { get; set; }
        public List<EPIConteudoEmailDTO> Conteudo { get; set; }
    }
}
