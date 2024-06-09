namespace BankingSagaPattern.TransferModule;

public class TransferWorker(ILogger<TransferWorker> logger, IConnection connection) : BackgroundService
{
    private IModel? _channel;

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _channel = connection.CreateModel();
        _channel.QueueDeclare(queue: "transferQueue", durable: false, exclusive: false, autoDelete: false, arguments: null);

        var consumer = new EventingBasicConsumer(_channel);
        consumer.Received += (model, ea) =>
        {
            var body = ea.Body.ToArray();
            var message = Encoding.UTF8.GetString(body);
            var transferEvent = JsonSerializer.Deserialize<TransferEvent>(message);

            if (transferEvent?.Status == "WITHDRAWN")
            {
                logger.LogInformation($"[[TransferService]]::[[{DateTime.Now.TimeOfDay}]]Event Status is Changed with WITHDRAWN Successfully!");
                QueueHelper.SendMessage("transferQueue", transferEvent,_channel);
            }
            else if (transferEvent?.Status == "DEPOSITED")
            {
                logger.LogInformation("[TransferService]::Event Status is Changed with COMPLETED Successfully!");
                transferEvent.Status = "COMPLETED";
                QueueHelper.SendMessage("notificationQueue", transferEvent,_channel);
            }
        };
        _channel.BasicConsume(queue: "transferQueue", autoAck: true, consumer: consumer);

        await Task.Delay(1000, stoppingToken);
    }

}
