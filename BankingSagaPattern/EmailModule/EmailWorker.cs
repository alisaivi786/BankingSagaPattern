namespace BankingSagaPattern.EmailModule;

public class EmailWorker(ILogger<EmailWorker> logger, IEmailSender emailSender, IConnection connection) : BackgroundService
{
    private IModel? _channel;

    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _channel = connection.CreateModel();
        _channel.QueueDeclare(queue: "emailQueue", durable: false, exclusive: false, autoDelete: false, arguments: null);

        var consumer = new EventingBasicConsumer(_channel);
        consumer.Received += (model, ea) =>
        {
            var body = ea.Body.ToArray();
            var message = Encoding.UTF8.GetString(body);
            var transferEvent = JsonSerializer.Deserialize<TransferEvent>(message);

            SendEmail(transferEvent);
        };

        _channel.BasicConsume(queue: "emailQueue", autoAck: true, consumer: consumer);

        return Task.CompletedTask;
    }

    private void SendEmail(TransferEvent transferEvent)
    {
        string message = @$"Dear User,

This is to notify you that your payment has been transferred successfully!
Account From: {transferEvent.FromAccount}
Account To: {transferEvent.ToAccount}
Transfer ID: {transferEvent.TransferId}
Amount: {transferEvent.Amount}

Thank you for using our service.

Regards,
Your Bank";

        var email = new Email
        {
            SenderEmail = "alisaivi786@gmail.com",
            RecipientEmail = "alisaivi786@gmail.com",
            Subject = "Transfer Confirmation",
            Body = message
        };

        emailSender.SendEmail(email, null);
    }
}
