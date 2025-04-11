
using CommunityToolkit.WinUI.Notifications;
using dm.PulseShift.Application.Interfaces;
using Microsoft.Extensions.Options;
using System.Xml;
using Windows.Data.Xml.Dom;
using Windows.UI.Notifications;

namespace dm.PulseShift.NotifierService;

public class NotificationWorker : BackgroundService
{
    private readonly ILogger<NotificationWorker> _logger;
    private readonly WorkerSettings _settings;
    private readonly IServiceProvider _serviceProvider;

    private TimeSpan _targetDuration;
    private TimeSpan _checkInterval;
    private bool _notifiedToday = false;
    private DateTime _lastCheckDate = DateTime.MinValue;

    public NotificationWorker(
        ILogger<NotificationWorker> logger,
        IOptions<WorkerSettings> settings, // Injete as configurações fortemente tipadas
        IServiceProvider serviceProvider)
    {
        _logger = logger;
        _settings = settings.Value;
        _serviceProvider = serviceProvider;

        // Parse das configurações (faça tratamento de erro aqui!)
        if (!TimeSpan.TryParse(_settings.TargetWorkDuration, out _targetDuration))
        {
            _logger.LogWarning("Formato inválido para TargetWorkDuration '{Duration}'. Usando padrão 8 horas.", _settings.TargetWorkDuration);
            _targetDuration = TimeSpan.FromHours(8);
        }
        _checkInterval = TimeSpan.FromMinutes(_settings.CheckIntervalMinutes);
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("PulseShift Notifier Service iniciado às: {time}", DateTimeOffset.Now);

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                DateTime currentDate = DateTime.Today;

                // Resetar flag de notificação se o dia mudou
                if (currentDate != _lastCheckDate)
                {
                    _logger.LogInformation("Novo dia detectado. Resetando flag de notificação.");
                    _notifiedToday = false;
                    _lastCheckDate = currentDate;
                }

                // Só executa a lógica se ainda não notificou hoje
                if (!_notifiedToday)
                {
                    // --- Criar um escopo de DI para resolver serviços Scoped/Transient ---
                    using var scope = _serviceProvider.CreateScope();
                    var workerAppService = scope.ServiceProvider.GetRequiredService<IWorkerAppService>(); // Resolva o serviço AQUI

                    _logger.LogInformation("Verificando duração da jornada às: {time}", DateTimeOffset.Now);

                    // Obter duração do serviço da aplicação
                    TimeSpan currentDuration = await workerAppService.GetTodaysDurationAsync(); // Chame seu método aqui

                    _logger.LogInformation("Duração atual da jornada: {duration}", currentDuration);

                    // Comparar com o alvo
                    if (currentDuration >= _targetDuration)
                    {
                        _logger.LogInformation("Duração da jornada atingiu o alvo. Enviando notificação.");
                        SendNotification(); // Chama o método para enviar a notificação
                        _notifiedToday = true; // Marca que já notificou hoje
                    }
                    // --- Escopo de DI é descartado aqui ---
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro durante a execução do worker às: {time}", DateTimeOffset.Now);
                // Considere adicionar uma lógica de backoff aqui para não tentar imediatamente após um erro
            }

            try
            {
                // Esperar pelo intervalo definido antes da próxima verificação
                _logger.LogDebug("Aguardando {interval} para próxima verificação.", _checkInterval);
                await Task.Delay(_checkInterval, stoppingToken);
            }
            catch (OperationCanceledException)
            {
                // Ocorre quando o serviço está parando, é normal.
                break;
            }
        }

        _logger.LogInformation("PulseShift Notifier Service parando às: {time}", DateTimeOffset.Now);
    }

    private void SendNotification()
    {
        try
        {
            // Usando CommunityToolkit.WinUI.Notifications
            var toastContent = new ToastContentBuilder()
            .AddText("Pulse Shift - Hora do Ponto!")
            .AddText($"Sua jornada atingiu ou passou de {_targetDuration:hh\\:mm}. Hora de marcar a saída!")
            // .AddAppLogoOverride(new Uri("file:///C:/path/to/your/logo.png")) // Logo requer AUMID configurado corretamente!
            // Adicione outros elementos se desejar
            .GetXml(); 
            
            var xmlDoc = new Windows.Data.Xml.Dom.XmlDocument();
            xmlDoc.LoadXml(toastContent.GetXml()); // Obtém a string XML

            // 3. Criar a Notificação Toast
            var toastNotification = new ToastNotification(xmlDoc);

            try
            {
                // Tenta criar o notificador padrão para a aplicação atual
                ToastNotifier toastNotifier = ToastNotificationManager.CreateToastNotifier();
                toastNotifier.Show(toastNotification);
                _logger.LogInformation("Notificação enviada (tentativa sem AUMID).");
            }
            catch (Exception exSimple)
            {
                _logger.LogError(exSimple, "Falha ao enviar notificação (sem AUMID). Um AUMID pode ser necessário. Veja as notas sobre registro de AUMID.");
                // **SOLUÇÃO ROBUSTA (REQUER REGISTRO PRÉVIO):**
                // Se você registrou um AUMID para sua aplicação (veja notas abaixo), use-o:
                /*
                string aumid = "SeuVendor.PulseShiftNotifier"; // Use o AUMID que você registrou!
                try
                {
                    ToastNotifier toastNotifierWithAumid = ToastNotificationManager.CreateToastNotifier(aumid);
                    toastNotifierWithAumid.Show(toastNotification);
                    _logger.LogInformation("Notificação enviada com AUMID '{Aumid}'.", aumid);
                }
                catch (Exception exAumid)
                {
                    // Se falhar mesmo com AUMID, pode ser problema no registro ou permissões.
                     _logger.LogError(exAumid, "Falha ao enviar notificação mesmo com AUMID '{Aumid}'. Verifique o registro do AUMID e as permissões.", aumid);
                }
                */
            }

            _logger.LogInformation("Notificação enviada com sucesso.");
        }
        catch (Exception ex)
        {
            // Pode falhar se o app não estiver registrado corretamente ou por permissões
            _logger.LogError(ex, "Falha ao enviar notificação Toast.");
        }
    }
}