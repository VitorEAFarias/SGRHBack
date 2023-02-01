namespace ApiSMT.Utilitários.JWT
{
    /// <summary>
    /// Interface repositorio de token
    /// </summary>
    public interface ITokenService
    {
        /// <summary>
        /// Cria token
        /// </summary>
        /// <param name="key"></param>
        /// <param name="issuer"></param>
        /// <param name="usuario"></param>
        /// <returns></returns>
        string BuildToken(string key, string issuer, string usuario);

        /// <summary>
        /// Verifica validade do token
        /// </summary>
        /// <param name="key"></param>
        /// <param name="issuer"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        string IsTokenValid(string key, string issuer, string token);
    }
}
