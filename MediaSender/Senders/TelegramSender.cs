using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using MediaSender.Properties;
using TeleSharp.TL;
using TLSharp.Core;
using TLSharp.Core.Utils;

namespace MediaSender.Senders
{
    class TelegramSender
    {
        private string _phone;

        private string _user;
        private string ResponceCode { get; set; }

        private string Phone
        {
            get => _phone;
            set => _phone = value.StartsWith("+") ? value.Substring(1, value.Length - 1) : value;
        }

        private string apiHash { get; set; } = Settings.Default.TelegramApiHash;
        private int ApiId { get; set; } = Settings.Default.TelegramApiId;
        public string DefUser
        {
            get => _user;
            set => _user = value.StartsWith("@") ? value.Substring(1, value.Length - 1) : value;
        }

        public TelegramSender()
        {
            Phone = Settings.Default.TelegramAccount;
            DefUser = Settings.Default.TelegramToSend;
        }

        public async void TelegramSend(List<string> attachments)
        {
            var store = new FileSessionStore();
            var client = new TelegramClient(ApiId, apiHash, store);
            await client.ConnectAsync();

            var session = client.Session;


            if (Settings.Default.SessionId != client.Session.Id)
            {
                await ReauthorizeClient(client);
            }

            var ContactsAll = await client.GetContactsAsync();


            var found = await client.SearchUserAsync(DefUser);
            var user = found.Users
                .Where(x => x.GetType() == typeof(TLUser))
                .Cast<TLUser>()
                .FirstOrDefault(x => x.Username == DefUser);

            if (user != null)
            {
                await client.SendMessageAsync(new TLInputPeerUser() { UserId = user.Id, AccessHash = user.AccessHash.Value }, "Hello!");

                if (attachments.Count != 0)
                {
                    foreach (string file in attachments)
                    {
                        var mediaFile = await client.UploadFile(file, new StreamReader(file));
                        await client.SendUploadedPhoto(new TLInputPeerUser() { UserId = user.Id, AccessHash = user.AccessHash.Value }, mediaFile, file);
                    }
                }
            }
            else
            {
                MessageBox.Show($"Telegram user {Settings.Default.TelegramToSend} not found!");
            }




        }

        private async Task ReauthorizeClient(TelegramClient client)
        {
            var hash = await client.SendCodeRequestAsync(Phone);

            Response responseWindow = new Response();
            responseWindow.ShowDialog();
            ResponceCode = responseWindow.CodeTextBox.Text;

            var user = await client.MakeAuthAsync(Phone, hash, ResponceCode);
            Settings.Default.SessionId = client.Session.Id;
            Settings.Default.Save();
        }



    }
}
