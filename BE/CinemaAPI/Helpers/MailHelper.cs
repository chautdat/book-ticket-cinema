using System;
using System.Diagnostics;     // Cho Debug.WriteLine
using System.Net;            // Cho NetworkCredential
using System.Net.Mail;       // Cho SmtpClient và MailMessage

namespace cinema
{
    public class MailHelper
    {
        public bool Send(string from, string to, string subject, string content)
        {
            try
            {
                var host = Environment.GetEnvironmentVariable("SMTP_HOST");
                var portValue = Environment.GetEnvironmentVariable("SMTP_PORT");
                var username = Environment.GetEnvironmentVariable("SMTP_USERNAME");
                var password = Environment.GetEnvironmentVariable("SMTP_PASSWORD");
                var fromAddress = Environment.GetEnvironmentVariable("SMTP_FROM");
                var enableSslValue = Environment.GetEnvironmentVariable("SMTP_ENABLE_SSL");

                if (
                    string.IsNullOrWhiteSpace(host) ||
                    string.IsNullOrWhiteSpace(portValue) ||
                    string.IsNullOrWhiteSpace(username) ||
                    string.IsNullOrWhiteSpace(password))
                {
                    Console.WriteLine("[MailHelper] SMTP configuration is missing. Email sending is disabled.");
                    return false;
                }

                if (!int.TryParse(portValue, out var port))
                {
                    port = 587;
                }

                var enableSsl = !string.Equals(enableSslValue, "false", StringComparison.OrdinalIgnoreCase);

                var smtpClient = new SmtpClient
                {
                    Host = host,
                    Port = port,
                    EnableSsl = enableSsl,
                    Credentials = new NetworkCredential(username, password)
                };

                var senderAddress = string.IsNullOrWhiteSpace(from)
                    ? fromAddress
                    : from;

                if (string.IsNullOrWhiteSpace(senderAddress))
                {
                    senderAddress = username;
                }

                var mailMessage = new MailMessage(senderAddress, to)
                {
                    Subject = subject,
                    Body = content,
                    IsBodyHtml = true
                };

                smtpClient.Send(mailMessage);
                Console.WriteLine($"[MailHelper] Email sent successfully to {to}");
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[MailHelper] ERROR sending email to {to}: {ex.Message}");
                Console.WriteLine($"[MailHelper] Stack trace: {ex.StackTrace}");
                Debug.WriteLine($"Error sending email: {ex.Message}");
                return false;
            }
        }
    }

    public class Email
    {
        public string From { get; set; }    // Địa chỉ người gửi
        public string To { get; set; }      // Địa chỉ người nhận
        public string Subject { get; set; } // Chủ đề email
        public string Content { get; set; } // Nội dung email (HTML/text)
    }
}
