namespace BankingSagaPattern.EmailModule.Interface;

public interface IEmailSender
{
    void SendEmail(Email email, string? Password);
}
