namespace ApiSMT.Utilitários.email
{
    /// <summary>
    /// Objeto conexão com o objeto de conexao com servidor de email
    /// </summary>
    public class EmailSettingsDTO
    {
        /// <summary>
        /// usuario
        /// </summary>
        public string usuario { get; set; }

        /// <summary>
        /// display name
        /// </summary>
        public string DisplayName { get; set; }

        /// <summary>
        /// password
        /// </summary>
        public string Password { get; set; }

        /// <summary>
        /// host
        /// </summary>
        public string Host { get; set; }

        /// <summary>
        /// port
        /// </summary>
        public int Port { get; set; }
    }
}
