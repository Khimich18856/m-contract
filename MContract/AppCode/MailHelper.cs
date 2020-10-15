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
		//public static void MailSend(string sendTo, string subject, string body)
		//{
		//	SmtpClient smtpServer = new SmtpClient("smtp-15.1gb.ru", 25);
		//	smtpServer.Credentials = new NetworkCredential("u510928", "e8ef59bzgh");
		//	smtpServer.Send(GetMessage(sendTo, subject, body, "supertrader5555@gmail.com"));

		//	//SmtpClient smtpServer = new SmtpClient("localhost", 25);
		//	//smtpServer.Send(GetMessage(sendTo, subject, body, "test@test.ru"));
		//}

		private static MailMessage GetMessage(string sendTo, string subject, string body, string fromemailuser)
		{
			MailMessage Message = new MailMessage();
			Message.From = new MailAddress(fromemailuser);
			Message.To.Add(new MailAddress(sendTo));
			Message.Subject = subject;
			Message.IsBodyHtml = true;
			Message.Body = body;
			return Message;
		}

		public static void SendE_Mail(string mailTo, string subject, string body)
		{
			SmtpClient client = new SmtpClient();
			client.DeliveryMethod = SmtpDeliveryMethod.Network;
			client.EnableSsl = true;
			client.Host = "smtp.gmail.com";
			client.Port = 587;

			NetworkCredential credentials =
			  new NetworkCredential("supertrader5555@gmail.com", "8Millions");
			client.UseDefaultCredentials = false;
			client.Credentials = credentials;

			MailMessage msg = new MailMessage();
			msg.From = new MailAddress("supertrader5555@gmail.com");
			msg.To.Add(new MailAddress(mailTo));
			//msg.CC.Add("");

			msg.Subject = subject;
			msg.IsBodyHtml = true;
			msg.Body = body;

			client.Send(msg);
		}
	}
}