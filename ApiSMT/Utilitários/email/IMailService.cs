using ControleEPI.DTO.E_Mail;
using System.Threading.Tasks;
using Vestimenta.DTO.email;

namespace ApiSMT.Utilitários
{
    /// <summary>
    /// Interface IMailService
    /// </summary>
    public interface IMailService
    {
        /// <summary>
        /// Chamada do envio de email para vestimenta
        /// </summary>
        /// <param name="emailRequest"></param>
        /// <returns></returns>
        Task SendEmailAsync(EmailRequestDTO emailRequest);

        /// <summary>
        /// Chamada do envio de email para EPI
        /// </summary>
        /// <param name="emailRequestDTO"></param>
        /// <returns></returns>
        Task SendEmailAsync(EPIEmailRequestDTO emailRequestDTO);
    }
}
