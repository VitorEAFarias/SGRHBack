using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace ApiSMT.Utilitários
{
    /// <summary>
    /// Classe que monitora quando um processo é executado ou pausado
    /// </summary>
    public class TimerHostedService : IHostedService
    {
        private readonly ILogger _logger;

        /// <summary>
        /// Construtor TimerHostedService
        /// </summary>
        /// <param name="logger"></param>
        public TimerHostedService(ILogger<TimerHostedService> logger)
        {
            _logger = logger;
        }

        /// <summary>
        /// Função que inicia o monitoramento processo
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public Task StartAsync(CancellationToken cancellationToken)
        {
            new Timer(ExecuteProcess, null, TimeSpan.Zero, TimeSpan.FromMinutes(5));
            return Task.CompletedTask;
        }

        /// <summary>
        /// Funçao que mostra o processo em execução
        /// </summary>
        /// <param name="state"></param>
        private void ExecuteProcess(object state)
        {
            _logger.LogInformation("### Proccess em execução ###");
            _logger.LogInformation($"{DateTime.Now}");
        }

        /// <summary>
        /// Funçao que mostra o processo que foi pausado
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("### Proccesso está pausado ###");
            _logger.LogInformation($"{DateTime.Now}");
            return Task.CompletedTask;
        }
    }
}
