using Microsoft.Extensions.Options;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using ControleEPI.BLL;
using ControleEPI.DTO.E_Mail;
using System.Text;

namespace ApiSMT.Utilitários
{
    /// <summary>
    /// Classe de envio de e-mail
    /// </summary>
    public class MailService : IMailService
    {
        private readonly EmailSettingsDTO _mailSettings;

        /// <summary>
        /// Construtor MailService
        /// </summary>
        /// <param name="emailSettings"></param>
        public MailService(IOptions<EmailSettingsDTO> emailSettings)
        {
            _mailSettings = emailSettings.Value;
        }

        /// <summary>
        /// Função para enviar e-mail
        /// </summary>
        /// <param name="mailRequest"></param>
        /// <returns></returns>
        public async Task SendEmailAsync(EmailRequestDTO mailRequest)
        {
            MailMessage message = new MailMessage();
            SmtpClient smtp = new SmtpClient();
            EncryptDecrypt crypt = new EncryptDecrypt();

            var hash = crypt.Decrypt(_mailSettings.Password);

            var sb = new StringBuilder();
            sb.AppendFormat(@"
                            <html>
                                <head>
                                    <meta http-equiv='Content-Type' content='text/html; charset=UTF-8'/>
                                    <meta name='viewport' content='width=device-width, initial-scale=1.0'/>
                                    <meta http-equiv='X-UA-Compatible' content='IE=edge' >");
            sb.AppendFormat(@"
             
                                    <style>
                                        body {{
			                                margin: 0;
			                                padding: 0;
			                            }}
                                        .texto {{
				                            font-family: 'montserrat';
				                            font-align: 'justify';
			                            }}			
			                            .middle {{
			                                position: absolute;			  
			                            }}
			                            .btn {{
			                                color: #7c8095;
			                                position: relative;
			                                display: block;
			                                font-size: 14px;
			                                font-family: 'montserrat';
			                                text-decoration: none;
			                                margin: 30px 0;
			                                border: 2px solid #001737;
			                                padding: 14px 60px;
			                                text-transform: uppercase;
			                                overflow: hidden;
			                                transition: 1s all ease;
			                            }}
			                            .btn::before {{
			                                background: #001737;
			                                content: '';
                                            position: absolute;
                                            top: 50%;
                                            left: 50%;
                                            transform: translate(-50%,-50%);
                                            z-index: -1;
                                            transition: all 0.6s ease;
                                        }}
			                            .btn1::before {{
			                                width: 0%;
			                                height: 100%;
			                            }}
			                            .btn1:hover::before {{
			                                width: 100%;
			                            }}		
			                            .styled-table {{
                                            border-collapse: collapse;
                                            margin: 25px 0;
                                            font-size: 0.9em;
                                            font-family: sans-serif;
                                            min-width: 400px;
                                            box-shadow: 0 0 20px rgba(0, 0, 0, 0.15);
                                        }}
			                            .styled-table thead th {{
                                            background-color: #001737;
				                            color: #7c8095;
				                            text-align: left;
                                        }}
			                            .styled-table td,
			                            .styled-table th {{
                                            padding: 12px 15px;
                                        }}			
			                            .styled-table tbody td {{
                                            border-bottom: 1px solid #dddddd;
			                            }}
			                            .styled-table tbody td:nth-of-type(even) {{
                                            background-color: #f3f3f3;
			                            }}
			                            .styled-table tbody td:last-of-type {{
                                            border-bottom: 2px solid #001737;
			                            }}
                                    </style>    
                                </head>
                                <body style='padding: 15px;'>
                                    <p><b>NUMERO DO PEDIDO:</b> {0}</p>
                                    <p><b>NOME DO COLABORADOR:</b> {1}</p>
                                    <p><b>DEPARTAMENTO:</b> {2}</p>
                                    <div style='margin-bottom: 10px'>
			                            <table class='styled-table' border='2' align='center' width='100%'>
				                            <thead>
					                            <tr>
						                            <th>Nome</th>
						                            <th>Tamanho</th>
						                            <th>Quantidade</th>
                                                    <th>Status</th>
					                            </tr>", mailRequest.ConteudoColaborador.idPedido, mailRequest.ConteudoColaborador.nomeColaborador, 
                                                mailRequest.ConteudoColaborador.departamento
            );
            foreach (var item in mailRequest.Conteudo)
            {                   
                sb.AppendFormat(@"<tr>
                                    <td>{0}</td>
                                    <td>{1}</td>
                                    <td>{2}</td>
                                    <td>{3}</td>
                                 </tr>", item.nome, item.tamanho, item.quantidade, item.status
                );
            }
            sb.Append(@"
				                            </thead>				                        
			                            </table>
                                    </div>
                                    <div class='middle'>
			                            <a href='https://intranet.reisoffice.com.br/smt/' class='btn btn1'>Acessar Sistema</a>
		                            </div>
                                </body>
                            </html>"
            );

            message.From = new MailAddress(mailRequest.EmailDe, _mailSettings.DisplayName);
            message.To.Add(new MailAddress(mailRequest.EmailPara));
            message.Subject = mailRequest.Assunto;
            message.IsBodyHtml = true;
            message.Body = sb.ToString();

            smtp.Port = _mailSettings.Port;
            smtp.Host = _mailSettings.Host;
            smtp.EnableSsl = true;
            smtp.UseDefaultCredentials = false;
            smtp.Credentials = new NetworkCredential(_mailSettings.usuario, hash);
            smtp.DeliveryMethod = SmtpDeliveryMethod.Network;

            //await smtp.SendMailAsync(message);
        }
    }
}