namespace OnlineShopping.Core.Interfaces;

public interface IEmailService
{
    Task SendEmailAsync(string to, string subject, string body, bool isHtml = true);
    Task SendEmailWithAttachmentAsync(string to, string subject, string body, byte[] attachment, string attachmentName, bool isHtml = true);
    Task SendMonthlyTransactionReportAsync(Guid customerId, DateTime month);
    Task SendBulkMonthlyReportsAsync(DateTime month);
}