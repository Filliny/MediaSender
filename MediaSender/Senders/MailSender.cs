using System;
using System.Collections.Generic;
using System.Windows;
using MailKit.Net.Smtp;
using MediaSender.Properties;
using MimeKit;

namespace MediaSender.Senders
{
	class MailSender {

		

		public async void  MailSend(List<string> attachments)
		{

			var message = new MimeMessage();
			message.From.Add(new MailboxAddress("Oleh", "olegf.droid@gmail.com"));
			message.To.Add(new MailboxAddress("Oleg", "oleg@timtyler.org.ua"));
			message.Subject = "How you doin'?";

			var builder = new BodyBuilder();
			builder.TextBody = @"Hey Alice,

						What are you up to this weekend? Monica is throwing one of her parties on
						Saturday and I was hoping you could make it.

						Will you be my +1?

						-- Joey
						";

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

				client.Connect("smtp.gmail.com", 465, true);

				// Note: only needed if the SMTP server requires authentication
                try
                {
                    string passw = Settings.Default.Password;
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
