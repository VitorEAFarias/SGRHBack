using System.Threading.Tasks;

namespace Utilitarios.Utilitários.email
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
    }
}
