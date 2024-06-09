using System.Threading.Channels;

namespace BankingSagaPattern.QueueGeneric;

internal class QueueHelper
{
    public static void SendMessage(string queueName, TransferEvent transferEvent, IModel _channel)
    {
        var body = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(transferEvent));
        _channel.BasicPublish(exchange: "", routingKey: queueName, basicProperties: null, body: body);
    }

    public static Task StopAsync(CancellationToken cancellationToken, IModel _channel, IConnection _connection)
    {
        _channel?.Close();
        _connection?.Close();
        return Task.CompletedTask;
    }
}
