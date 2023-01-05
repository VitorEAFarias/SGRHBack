using ControleEPI.DTO.E_Mail;
using System.Threading.Tasks;

namespace ApiSMT.Utilitários
{
    /// <summary>
    /// Interface IMailService
    /// </summary>
    public interface IMailService
    {
        /// <summary>
        /// Chamada do envio de email
        /// </summary>
        /// <param name="emailRequest"></param>
        /// <returns></returns>
        Task SendEmailAsync(EmailRequestDTO emailRequest);
    }
}
