using System.Collections.Generic;
using System.Text;
using Vestimenta.DTO.DinkPDF;

namespace ApiSMT.Utilitários.PDF
{
    /// <summary>
    /// Template html relatorio vestimentas a serem entregues
    /// </summary>
    public class RelatórioPDF
    {
        /// <summary>
        /// Função para montar o html com as informações enviadas
        /// </summary>
        /// <param name="dadosPDF"></param>
        /// <returns></returns>
        public static string GetHTMLString(List<VestRelatorioVestimentasDTO> dadosPDF)
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
                                        font-size: 16px;
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
                                        font-size: 14px;
                                    }}

                                    table {{
                                        width: 100%;
                                    }}

                                    tr:nth-child(even) {{
                                        background-color: #f2f2f2;
                                    }}
                                </style>
                            </head>
                            <body>
                                <div class='row'><img src='https://www.reisoffice.com.br/images/logo-reisoffice-novo.png'/></div>
                                <div class='header'><h1>Relatório de entrega de uniforme</h1></div>
                                <div style='overflow-x: auto;'>
                                    <table align='center'>
                                        <tr>
                                            <th>Número do Pedido</th>
                                            <th>Data do Pedido</th>
                                            <th>Colaborador</th>
                                            <th>Departamento</th>
                                            <th>Vestimenta</th>
                                            <th>Tamanho</th>
                                            <th>Quantidade</th>
                                        </tr>");
            foreach (var item in dadosPDF)
            {
                sb.AppendFormat(@"<tr>
                                    <td>{0}</td>
                                    <td>{1}</td>
                                    <td>{2}</td>
                                    <td>{3}</td>
                                    <td>{4}</td>
                                    <td>{5}</td>
                                    <td>{6}</td>
                                  </tr>", item.numeroPedido, item.dataPedido.ToString("dd/MM/yyyy HH:mm"), item.colaborador, item.departamento, item.vestimenta.ToUpper(), 
                                  item.tamanho, item.quantidade);
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
