using System.Security.Cryptography;
using System.Text;

namespace ApiSMT.Utilitários
{
    /// <summary>
    /// Classe que gera criptografia MD5
    /// </summary>
    public class GerarMD5
    {
        /// <summary>
        /// Função que converte o valor digitado para criptografia MD5
        /// </summary>
        /// <param name="valor"></param>
        /// <returns></returns>
        public string GeraMD5(string valor)
        {
            //O mesmo do PHP
            MD5 md5Hasher = MD5.Create();

            byte[] valorCriptografado = md5Hasher.ComputeHash(Encoding.Default.GetBytes(valor));

            StringBuilder strBuilder = new StringBuilder();

            for (int i = 0; i < valorCriptografado.Length; i++)
            {
                strBuilder.Append(valorCriptografado[i].ToString("x2"));
            }

            return strBuilder.ToString();

        }
    }
}
