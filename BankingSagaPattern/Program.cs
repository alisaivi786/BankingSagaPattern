var builder = Host.CreateApplicationBuilder(args);
builder.Services.AddSingleton<IEmailSender, SmtpEmailSender>();
builder.Services.AddSingleton(provider =>
{
    var factory = new ConnectionFactory()
    {
        HostName = "localhost"
    };
    var connection = factory.CreateConnection();
    //var transferService = new TransferService(connection);

    //// Example transfer initiation
    //transferService.InitiateTransfer("IBAN123456789", "IBAN987654321", 1000);

    return connection;
});
builder.Services.AddHostedService<NotificationWorker>();
builder.Services.AddHostedService<AccountWorker>();
builder.Services.AddHostedService<TransferWorker>();
builder.Services.AddHostedService<EmailWorker>();


var host = builder.Build();
host.Run();
