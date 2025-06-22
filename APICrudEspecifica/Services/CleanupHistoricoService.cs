using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading;
using System.Threading.Tasks;
using APICrudEspecifica.Data;
using Microsoft.EntityFrameworkCore;

public class CleanupHistoricoService : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;

    public CleanupHistoricoService(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            // Executa a limpeza assim que iniciar
            await LimparHistoricosAntigosAsync(stoppingToken);

            // Espera 24 horas para a próxima execução
            await Task.Delay(TimeSpan.FromDays(1), stoppingToken);
        }
    }

    private async Task LimparHistoricosAntigosAsync(CancellationToken stoppingToken)
    {
        using (var scope = _serviceProvider.CreateScope())
        {
            var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();

            var limite = DateTime.Now.AddDays(-7);

            var historicosAntigos = await dbContext.HistoricosPrecos
                .Where(h => h.DataCalculo < limite)
                .ToListAsync(stoppingToken);

            if (historicosAntigos.Any())
            {
                dbContext.HistoricosPrecos.RemoveRange(historicosAntigos);
                await dbContext.SaveChangesAsync(stoppingToken);
            }
        }
    }

}
