public class SePayWebhookRequest
{
    public string id { get; set; }
    public string gateway { get; set; }
    public string transactionDate { get; set; }
    public string accountNumber { get; set; }
    public string subAccountCode { get; set; }
    public string content { get; set; } // Ví dụ: "Nguyen Van A chuyen tien DH123"
    public string transferType { get; set; } // "in" hoặc "out"
    public decimal transferAmount { get; set; }
    public decimal accumulated { get; set; }
    public string referenceCode { get; set; }
    public string description { get; set; }
}