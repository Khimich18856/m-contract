using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Web;

namespace MContract.AppCode
{
	public class MailHelper
	{
        //================= настройка данных потчового ящика ======================== //
        public static void SendMail(string sendTo, string subject, string body)
        {
            SmtpClient smtpServer = new SmtpClient("94.103.92.235", 25)
            {
                Credentials = new NetworkCredential("info@m-contract.ru", "AF3z9J79hGCi")
            };
            smtpServer.Send(GetMessage(sendTo, subject, body, "info@m-contract.ru"));
        }

        // ==================== метод отправки почты ===================================//
        private static MailMessage GetMessage(string sendTo, string subject, string body, string fromemailuser)
        {
            MailMessage Message = new MailMessage
            {
                From = new MailAddress(fromemailuser)
            };
            Message.To.Add(new MailAddress(sendTo));
            Message.Subject = subject;
            Message.IsBodyHtml = true;
            Message.Body = body;
            return Message;
        }
    }
}