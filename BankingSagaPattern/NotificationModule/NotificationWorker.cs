namespace BankingSagaPattern.NotificationModule;

public class NotificationWorker(ILogger<NotificationWorker> logger, IConnection connection) : BackgroundService
{
    private IModel? _channel;

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _channel = connection.CreateModel();
        _channel.QueueDeclare(queue: "notificationQueue", durable: false, exclusive: false, autoDelete: false, arguments: null);
        _channel.QueueDeclare(queue: "emailQueue", durable: false, exclusive: false, autoDelete: false, arguments: null);

        var consumer = new EventingBasicConsumer(_channel);
        consumer.Received += (model, ea) =>
        {
            var body = ea.Body.ToArray();
            var message = Encoding.UTF8.GetString(body);
            var transferEvent = JsonSerializer.Deserialize<TransferEvent>(message);

            if (transferEvent?.Status == "COMPLETED")
            {
                QueueHelper.SendMessage("emailQueue", transferEvent,_channel);
                logger.LogInformation("[NotificationService]::Email Trigger Successfully!");
            }

            // Send notification
            logger.LogInformation($"Transfer {transferEvent?.TransferId} completed.");
           
        };
        _channel.BasicConsume(queue: "notificationQueue", autoAck: true, consumer: consumer);

        await Task.Delay(1000, stoppingToken);
    }


}
