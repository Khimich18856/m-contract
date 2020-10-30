using System;
using System.Net;
using System.Net.Mail;
using System.Web;
using System.Web.UI;

namespace MContract.AppCode
{
    public class MailHelper
    {
        //================= настройка данных потчового ящика ======================== //
        public static void SendMail(string sendTo, string subject, string body)
        {
            SmtpClient smtpServer = new SmtpClient("94.103.92.235", 25)
            {
                Port = 25,
                Credentials = new NetworkCredential("info@m-contract.ru", "AF3z9J79hGCi"),
                Timeout = 20000
            };
            smtpServer.EnableSsl = false;
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

        // ==================== метод отправки почты с вложением файла ===================================//
        public static void MailSendAttachment(string strFrom, string strTo, string strToCC, string strSubject, string strBody, string dirName, string fileName)
        {
            if (ValidHelper.EmailRus(strTo))
            try
            {
                MailMessage Message = new MailMessage(strFrom, strTo); // 

                if (ValidHelper.EmailRus(strToCC))
                    Message.CC.Add(strToCC);
                Message.Body = strBody;
                Message.Subject = strSubject;
                Message.IsBodyHtml = true;
                if (fileName.Length > 0)
                {
                    Attachment att = new Attachment(HttpContext.Current.Server.MapPath("~/" + dirName + "/") + fileName, "application/pdf");
                    Message.Attachments.Add(att);
                }

                SmtpClient smtpServer = new SmtpClient("94.103.92.235", 25)
                {
                    Port = 25,
                    Credentials = new NetworkCredential("info@m-contract.ru", "AF3z9J79hGCi"),
                    Timeout = 20000
                };
                smtpServer.EnableSsl = false;

                smtpServer.Send(Message);
                Message.Dispose();
                Console.WriteLine("Отлично.");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message.ToString());
            }
        }
    }
}