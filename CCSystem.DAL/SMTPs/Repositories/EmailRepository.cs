﻿using CCSystem.DAL.Redis.Models;
using CCSystem.DAL.SMTPs.Models;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using CCSystem.DAL.Enums;

namespace CCSystem.DAL.SMTPs.Repositories
{
    public class EmailRepository
    {
        public EmailRepository()
        {

        }

        private Email GetEmailProperty()
        {
            IConfigurationBuilder builder = new ConfigurationBuilder()
                                  .SetBasePath(Directory.GetCurrentDirectory())
                                  .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);
            IConfigurationRoot configuration = builder.Build();
            return new Email()
            {
                Host = configuration.GetSection("Verification:Email:Host").Value,
                Port = int.Parse(configuration.GetSection("Verification:Email:Port").Value),
                SystemName = configuration.GetSection("Verification:Email:SystemName").Value,
                Sender = configuration.GetSection("Verification:Email:Sender").Value,
                Password = configuration.GetSection("Verification:Email:Password").Value,
            };
        }

        private string GetMessageToResetPassword(string systemName, string receiverEmail, string OTPCode)
        {
            string emailBody = "";
            string htmlParentDivStart = "<div style=\"font-family: Helvetica,Arial,sans-serif;min-width:1000px;overflow:auto;line-height:2\">";
            string htmlParentDivEnd = "</div>";
            string htmlMainDivStart = "<div style=\"margin:50px auto;width:70%;padding:20px 0\">";
            string htmlMainDivEnd = "</div>";
            string htmlSystemNameDivStart = "<div style=\"border-bottom:1px solid #eee\">";
            string htmlSystemNameDivEnd = "</div";
            string htmlSystemNameSpanStart = "<span style=\"font-size:1.4em;color: #00466a;text-decoration:none;font-weight:600\">";
            string htmlSystemNameSpanEnd = "</span>";
            string htmlHeaderBodyStart = "<p style=\"font-size:1.1em\">";
            string htmlHeaderBodyEnd = "</p>";
            string htmlBodyStart = "<p>";
            string htmlBodyEnd = "</p>";
            string htmlOTPCodeStart = "<h2 style=\"background: #00466a;margin: 0 auto;width: max-content;padding: 0 10px;color: #fff;border-radius: 4px;\">";
            string htmlOTPCodeEnd = "</h2>";
            string htmlFooterBodyStart = "<p style=\"font-size:0.9em;\">";
            string htmlBreakLine = "<br />";
            string htmlFooterBodyEnd = "</p>";

            emailBody += htmlParentDivStart;
            emailBody += htmlMainDivStart;
            emailBody += htmlSystemNameDivStart + htmlSystemNameSpanStart + systemName + htmlSystemNameSpanEnd + htmlSystemNameDivEnd + htmlBreakLine;
            emailBody += htmlHeaderBodyStart + $"Hi {receiverEmail}," + htmlHeaderBodyEnd;
            emailBody += htmlBodyStart + $"We've received a request to reset the password from {receiverEmail}. " +
                $"Use the following OTP to complete your reset password procedures. OTP is valid for 10 minutes." + htmlBodyEnd;
            emailBody += htmlOTPCodeStart + OTPCode + htmlOTPCodeEnd;
            emailBody += htmlFooterBodyStart + "Regards," + htmlBreakLine + systemName + htmlFooterBodyEnd;
            emailBody += htmlMainDivEnd;
            emailBody += htmlParentDivEnd;

            return emailBody;
        }

        private string GetMessageToConfirm(string systemName, string receiverEmail)
        {
            string emailBody = "";
            string htmlParentDivStart = "<div style=\"font-family: Helvetica,Arial,sans-serif;min-width:1000px;overflow:auto;line-height:2\">";
            string htmlParentDivEnd = "</div>";
            string htmlMainDivStart = "<div style=\"margin:50px auto;width:70%;padding:20px 0\">";
            string htmlMainDivEnd = "</div>";
            string htmlSystemNameDivStart = "<div style=\"border-bottom:1px solid #eee\">";
            string htmlSystemNameDivEnd = "</div>";
            string htmlSystemNameSpanStart = "<span style=\"font-size:1.4em;color: #00466a;text-decoration:none;font-weight:600\">";
            string htmlSystemNameSpanEnd = "</span>";
            string htmlHeaderBodyStart = "<p style=\"font-size:1.1em\">";
            string htmlHeaderBodyEnd = "</p>";
            string htmlBodyStart = "<p>";
            string htmlBodyEnd = "</p>";
            string htmlFooterBodyStart = "<p style=\"font-size:0.9em;\">";
            string htmlBreakLine = "<br />";
            string htmlFooterBodyEnd = "</p>";

            emailBody += htmlParentDivStart;
            emailBody += htmlMainDivStart;
            emailBody += htmlSystemNameDivStart + htmlSystemNameSpanStart + systemName + htmlSystemNameSpanEnd + htmlSystemNameDivEnd + htmlBreakLine;
            emailBody += htmlHeaderBodyStart + $"Hi {receiverEmail}," + htmlHeaderBodyEnd;
            emailBody += htmlBodyStart + "Your booking has been marked as completed by the housekeeper. Please confirm the completion of your booking by clicking the confirmation link provided." + htmlBodyEnd;
            emailBody += htmlFooterBodyStart + "Regards," + htmlBreakLine + systemName + htmlFooterBodyEnd;
            emailBody += htmlMainDivEnd;
            emailBody += htmlParentDivEnd;

            return emailBody;
        }


        public EmailVerification SendEmailToResetPassword(string receiverEmail)
        {
            try
            {
                Email email = GetEmailProperty();
                MailMessage mailMessage = new MailMessage();
                SmtpClient smtp = new SmtpClient();
                mailMessage.From = new MailAddress(email.Sender);
                mailMessage.To.Add(new MailAddress(receiverEmail));
                mailMessage.Subject = "Reset your password";
                mailMessage.IsBodyHtml = true;
                string otpCode = GenerateOTPCode();
                mailMessage.Body = GetMessageToResetPassword(email.SystemName, receiverEmail, otpCode);
                smtp.Port = email.Port;
                smtp.Host = email.Host;
                smtp.EnableSsl = true;
                smtp.UseDefaultCredentials = false;
                smtp.Credentials = new NetworkCredential(email.Sender, email.Password);
                smtp.DeliveryMethod = SmtpDeliveryMethod.Network;
                smtp.Send(mailMessage);
                EmailVerification emailVerification = new EmailVerification()
                {
                    Email = receiverEmail,
                    OTPCode = otpCode,
                    CreatedDate = DateTime.Now,
                    IsVerified = Convert.ToBoolean((int)EmailVerificationEnum.Status.NOT_VERIFIRED)
                };
                return emailVerification;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task SendEmailToCustomerConfirm(string receiverEmail)
        {
            try
            {
                Email email = GetEmailProperty();
                MailMessage mailMessage = new MailMessage();
                SmtpClient smtp = new SmtpClient();

                mailMessage.From = new MailAddress(email.Sender);
                mailMessage.To.Add(new MailAddress(receiverEmail));
                mailMessage.Subject = "Booking Completion Confirmation";
                mailMessage.IsBodyHtml = true;

                // Sử dụng hàm GetMessageToConfirm thay vì GetMessageToResetPassword
                mailMessage.Body = GetMessageToConfirm(email.SystemName, receiverEmail);

                smtp.Port = email.Port;
                smtp.Host = email.Host;
                smtp.EnableSsl = true;
                smtp.UseDefaultCredentials = false;
                smtp.Credentials = new NetworkCredential(email.Sender, email.Password);
                smtp.DeliveryMethod = SmtpDeliveryMethod.Network;

                await smtp.SendMailAsync(mailMessage);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }


        private string GenerateOTPCode()
        {
            Random random = new Random();
            string otp = string.Empty;
            for (int i = 0; i < 6; i++)
            {
                int tempval = random.Next(0, 10);
                otp += tempval;
            }
            return otp;
        }
    }
}
