namespace BankingSagaPattern.AccountModule;
public class AccountWorker(ILogger<AccountWorker> logger, IConnection connection) : BackgroundService
{
    private IModel? _channel;


    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _channel = connection.CreateModel();
        _channel.QueueDeclare(queue: "transferQueue", durable: false, exclusive: false, autoDelete: false, arguments: null);

        var consumer = new EventingBasicConsumer(_channel);
        consumer.Received += (model, ea) =>
        {
            var body = ea.Body.ToArray();
            var message = Encoding.UTF8.GetString(body);
            var transferEvent = JsonSerializer.Deserialize<TransferEvent>(message);

            if (transferEvent.Status == "INITIATED")
            {
                // Simulate withdrawal
                logger.LogInformation("[AccountService]::Event Status is Changed with WITHDRAWN Successfully!");
                transferEvent.Status = "WITHDRAWN";
                QueueHelper.SendMessage("transferQueue", transferEvent,_channel);
            }
            else if (transferEvent.Status == "WITHDRAWN")
            {
                // Simulate deposit
                logger.LogInformation("[AccountService]::Event Status is Changed with DEPOSITED Successfully!");
                transferEvent.Status = "DEPOSITED";
                QueueHelper.SendMessage("transferQueue", transferEvent,_channel);
            }
        };
        _channel.BasicConsume(queue: "transferQueue", autoAck: true, consumer: consumer);
        
        return Task.CompletedTask;

    }
}
