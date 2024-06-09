namespace BankingSagaPattern.EmailModule;

public class Email
{
    public string? SenderEmail { get; set; }
    public string? RecipientEmail { get; set; }
    public string? Subject { get; set; }
    public string? Body { get; set; }
    public string? AttachmentPath { get; set; }
}