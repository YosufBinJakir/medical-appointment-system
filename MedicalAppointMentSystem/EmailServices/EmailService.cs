namespace MedicalAppointMentSystem.EmailServices
{
    using MedicalAppointMentSystem.DTOs.EmailDtos;
    using Microsoft.Extensions.Options;
    using System.Net;
    using System.Net.Mail;
    using System.Net.Mime;

    public class EmailService : IEmailService
    {
        private readonly EmailSettings _emailSettings;
        private readonly ILogger<EmailService> _logger;

        public EmailService(IOptions<EmailSettings> emailSettings, ILogger<EmailService> logger)
        {
            _emailSettings = emailSettings.Value;
            _logger = logger;
        }

        public async Task SendEmailAsync(string toEmail, string subject, string body,
            byte[] attachment = null, string attachmentName = null)
        {
            try
            {
                using (var message = new MailMessage())
                {
                    message.From = new MailAddress(_emailSettings.SenderEmail, _emailSettings.SenderName);
                    message.To.Add(new MailAddress(toEmail));
                    message.Subject = subject;
                    message.Body = body;
                    message.IsBodyHtml = true;

                    if (attachment != null && !string.IsNullOrEmpty(attachmentName))
                    {
                        if (attachment != null && !string.IsNullOrEmpty(attachmentName))
                        {
                            var stream = new MemoryStream(attachment); // just create the stream
                            var contentType = new ContentType(MediaTypeNames.Application.Pdf);
                            var emailAttachment = new Attachment(stream, contentType)
                            {
                                Name = attachmentName
                            };
                            emailAttachment.ContentDisposition.FileName = attachmentName;
                            message.Attachments.Add(emailAttachment);
                        }

                    }

                    using (var client = new SmtpClient(_emailSettings.SmtpServer, _emailSettings.SmtpPort))
                    {
                        client.Credentials = new NetworkCredential(_emailSettings.UserName, _emailSettings.Password);
                        client.EnableSsl = _emailSettings.EnableSsl;

                     
                        client.Timeout = 30000;

                        await client.SendMailAsync(message);
                    }
                }

                _logger.LogInformation($"Email sent successfully to {toEmail}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Failed to send email to {toEmail}");
                throw new Exception("Failed to send email. Please try again later.", ex);
            }
        }
    }
}
