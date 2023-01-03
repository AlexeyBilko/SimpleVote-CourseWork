
using LeadSub.Models.Email;
using System.Net;
using System.Net.Mail;

namespace LeadSub.Models
{
    public class EmailManager
    {
        public static Task SendText(EmailConfiguration config, EmailMessage message)
        {
            return Task.Run(() =>
            {
                MailAddress from = new MailAddress(config.From);
                MailAddress to = new MailAddress(message.To);
                using (MailMessage m = new MailMessage(from, to))
                {
                    using (SmtpClient smtp = new SmtpClient(config.SmtpServer, config.Port))
                    {
                        m.Subject = message.Subject;
                        m.IsBodyHtml = true;
                        string html = "<div class=\"c-email\" style=\"font-family: \'Arial\';font-style: normal; font-size: 18px; color: black;\">" +
                                        "<div class=\"c-email__header\">" +
                                            "<h1 class=\"c-email__header__title\">Your Verification Code</h1>" +
                                        "</div>" +
                                        "<div class=\"c-email__content\">" +
                                            "<p class=\"c-email__content__text text-title\">Enter this verification code in field:</p>" +
                                            "<div class=\"c-email__code\">" +
                                                $"<h2 style=\"font-size: 24px !important;\" class=\"c-email__code__text\">{message.Content}</h2>" +
                                            "</div>" +
                                            "<p class=\"c-email__content__text text-italic opacity-30 text-title mb-0\">Thanks for the registration!</p>" +
                                        "</div>" +
                                        "<div class=\"c-email__footer\">" +
                                        "</div>" +
                                        "<div style=\"font-size: 24px !important; font-weight: 900;\">Simple<span style=\"color: #3362DB !important;\">Vote</span></div>" +
                                      "</div>";
                        m.Body = $"{html}";
                        smtp.Credentials = new NetworkCredential(config.UserName, config.Password);
                        smtp.EnableSsl = true;
                        //try
                        //{
                            smtp.Send(m);
                        //}
                        //catch { }
                    }
                }
             
            });
        }

        public static Task ContactUsMessage(EmailConfiguration config, EmailMessage message)
        {
            return Task.Run(() =>
            {
                MailAddress from = new MailAddress("leadsub.manager@gmail.com");
                MailAddress to = new MailAddress("leadsub.manager@gmail.com");
                using (MailMessage m = new MailMessage(from, to))
                {
                    using (SmtpClient smtp = new SmtpClient(config.SmtpServer, config.Port))
                    {
                        m.Subject = message.Subject;
                        m.IsBodyHtml = true;
                        string html = $"Contact Us from {message.Subject}<br></br><br></br>Message: \"{message.Content}\"";
                        m.Body = $"{html}";
                        smtp.Credentials = new NetworkCredential(config.UserName, config.Password);
                        smtp.EnableSsl = true;
                        //try
                        //{
                        smtp.Send(m);
                        //}
                        //catch { }
                    }
                }

            });
        }

        public static Task ReportPage(EmailConfiguration config, EmailMessage message)
        {
            return Task.Run(() =>
            {
                MailAddress from = new MailAddress("leadsub.manager@gmail.com");
                MailAddress to = new MailAddress("leadsub.manager@gmail.com");
                using (MailMessage m = new MailMessage(from, to))
                {
                    using (SmtpClient smtp = new SmtpClient(config.SmtpServer, config.Port))
                    {
                        m.Subject = message.Subject;
                        m.IsBodyHtml = true;
                        string html = $"Subpage with Id: {message.Subject} reported. Title: {message.To}<br></br><br></br>Owner Email: \"{message.Content}\"";
                        m.Body = $"{html}";
                        smtp.Credentials = new NetworkCredential(config.UserName, config.Password);
                        smtp.EnableSsl = true;
                        //try
                        //{
                        smtp.Send(m);
                        //}
                        //catch { }
                    }
                }

            });
        }
    }
}
