namespace BankingSagaPattern.Common;

public class TransferEvent
{
    public string TransferId { get; set; } = Guid.NewGuid().ToString();
    public string? FromAccount { get; set; }
    public string? ToAccount { get; set; }
    public double? Amount { get; set; } = 0;
    /// <summary>
    ///  INITIATED, WITHDRAWN, DEPOSITED, FAILED
    /// </summary>
    public string Status { get; set; } = "FAILED";
}