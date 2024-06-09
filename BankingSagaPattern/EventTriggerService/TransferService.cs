namespace BankingSagaPattern.EventTriggerService;
public class TransferService(IConnection connection)
{
    public void InitiateTransfer(string fromAccount, string toAccount, double amount)
    {
        using var channel = connection.CreateModel();
        channel.QueueDeclare(queue: "transferQueue", durable: false, exclusive: false, autoDelete: false, arguments: null);

        var transferEvent = new TransferEvent
        {
            FromAccount = fromAccount,
            ToAccount = toAccount,
            Amount = amount,
            Status = "INITIATED"
        };

        var message = JsonSerializer.Serialize(transferEvent);
        var body = Encoding.UTF8.GetBytes(message);

        channel.BasicPublish(exchange: "", routingKey: "transferQueue", basicProperties: null, body: body);
    }
}