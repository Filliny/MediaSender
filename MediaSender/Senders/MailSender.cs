using System;
using System.Collections.Generic;
using System.Windows;
using MailKit.Net.Smtp;
using MediaSender.Properties;
using MimeKit;

namespace MediaSender.Senders
{
	class MailSender
    {
        private Settings Settings = Properties.Settings.Default;

		public async void  MailSend(List<string> attachments)
		{

			var message = new MimeMessage();
			message.From.Add(new MailboxAddress("MediaSender", Settings.Email));
			message.To.Add(new MailboxAddress("User", Settings.EmailToSend));
			message.Subject = "Message from media sender";
            ;

			var builder = new BodyBuilder();
			builder.TextBody = Settings.DefaultMessage;

			// We may also want to attach a calendar event for Monica's party...

			foreach (string attachment in attachments) {
				builder.Attachments.Add(attachment);
			}
			// Now we just need to set the message body and we're done
			message.Body = builder.ToMessageBody();

			using (var client = new SmtpClient())
			{
				// For demo-purposes, accept all SSL certificates (in case the server supports STARTTLS)
				client.ServerCertificateValidationCallback = (s, c, h, e) => true;

				client.Connect(Settings.SmtpServer, Settings.SmtpPort,Settings.SSL);

				// Note: only needed if the SMTP server requires authentication
                try
                {
                    string passw = Settings.Password;
                    //new NetworkCredential("", Settings.Default.Password).Password;

					client.Authenticate(Settings.Default.Email,  passw);
					await client.SendAsync(message);
					MessageBox.Show(passw);
				}
                catch (Exception e)
                {
                    MessageBox.Show(e.Message+ "\n" + e.HelpLink);
					
                }
                finally
                {
                    client.Disconnect(true);
				}
				
			}
		}



	}
}
