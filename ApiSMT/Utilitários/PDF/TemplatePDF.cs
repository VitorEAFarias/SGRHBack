using System.Text;
using Vestimenta.DTO.DinkPDF;

namespace ApiSMT.Utilitários.PDF
{
    /// <summary>
    /// CLasse template html
    /// </summary>
    public class TemplatePDF
    {
        /// <summary>
        /// Função para montar o html com as informações enviadas
        /// </summary>
        /// <param name="dadosPDF"></param>
        /// <returns></returns>
        public static string GetHTMLString(VestDadosPDFDTO dadosPDF)
        {
            var sb = new StringBuilder();
            sb.AppendFormat(@"
                        <html>
                            <head>
                                <meta http-equiv='Content-Type' content='text/html; charset=UTF-8'/>
                                <meta name='viewport' content='width=device-width, initial-scale=1.0'/>
                                <meta http-equiv='X-UA-Compatible' content='IE=edge' >");
            sb.AppendFormat(@" 
                                <style>
                                    .header {{
                                        text-align: center;
                                        color: gray;
                                        padding-bottom: 35px;
                                    }}

                                    th {{
                                        height: 70px;
                                        font-size: 21px;
                                        background-color: #4B0082;
                                        color: white;
                                    }}

                                    td, th {{
                                        padding: 15px;
                                        text-align: center;
                                        border-bottom: 1px solid #ddd;
                                    }}

                                    td {{
                                        height: 50px;
                                        font-size: 18px;
                                    }}

                                    table {{
                                        width: 100%;
                                    }}

                                    tr:nth-child(even) {{
                                        background-color: #f2f2f2;
                                    }}

                                    p {{
                                        color: black;
                                        text-align: left;
                                        font-size: 20px;
                                    }}
                                </style>
                            </head>
                            <body>
                                <div class='row'><img src='https://www.reisoffice.com.br/images/logo-reisoffice-novo.png'/></div>
                                <div class='header'><h1>Histórico de Vestimentas</h1></div>
                                <div style='overflow-x: auto;'>
                                    <table align='center'>
                                        <tr>
                                            <th>Nome</th>
                                            <th>Tamanho</th>
                                            <th>Quantidade</th>
                                            <th>Data de Recebimento</th>
                                            <th>Data de Devolução</th>
                                            <th>Item Usado</th>
                                            <th>Item recebido por: </th>
                                        </tr>");
            foreach (var item in dadosPDF.vestimentas)
            {
                sb.AppendFormat(@"<tr>
                                    <td>{0}</td>
                                    <td>{1}</td>
                                    <td>{2}</td>
                                    <td>{3}</td>
                                    <td>{4}</td>
                                    <td>{5}</td>
                                    <td>{6}</td>
                                  </tr>", item.nomeVestimenta, item.tamanho, item.quantidade, item.dataVinculo.ToString("dd/MM/yyyy HH:mm"), 
                                  item.dataDesvinculo.ToString("dd/MM/yyyy HH:mm") == "01/01/0001 00:00"? "-" : item.dataDesvinculo.ToString("dd/MM/yyyy HH:mm"), 
                                  item.usado, dadosPDF.nome);
            }
            sb.Append(@"
                                    </table>
                                </div>
                            </body>
                        </html>");
            return sb.ToString();
        }
    }
}
