using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Microsoft.AspNetCore.SignalR.Client;
using Xamarin.Forms;

namespace SignalR_Client.ViewModel
{
    class Chat_ViewModel : _BaseViewModel
    {
        private HubConnection hubConnection;
        private string ip = "";
        private string port = "";
        private string spoofedUser = "Mr. Bojangels";

        private ObservableCollection<string> _Messages;
        public ObservableCollection<string> PropertyMessages
        {
            set { SetProperty(ref _Messages, value); }
            get { return _Messages; }
        }

        private string _MessageToSend;
        public string PropertyMessageToSend
        {
            set { SetProperty(ref _MessageToSend, value); }
            get { return _MessageToSend; }
        }

        public Chat_ViewModel()
        {
            //Excluded for security this is your localhost or hosted website
            ip = ConnectionData.localConnection;
            port = ConnectionData.localPort;

            hubConnection = new HubConnectionBuilder()
                .WithUrl($"http://{ip}:{port}/chatHub")
                .Build();

            Task.Run(async () => await Connect());

            hubConnection.On<string, string>("ReceiveMessage", (user, message) =>
            {
                var newMessage = $"{user} says {message}";
                PropertyMessages.Add(newMessage);
            });
        }

        private async Task Connect()
        {
            try
            {
                await hubConnection.StartAsync();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        private ICommand _SendMessage;
        public ICommand SendMessageCommand
        {
            get
            {
                return _SendMessage ?? (_SendMessage = new Command(async (contact) => await SendHubMessage()));
            }
        }

        private async Task SendHubMessage()
        {
            try
            {
                await hubConnection.InvokeAsync("SendMessage", spoofedUser, PropertyMessageToSend);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

       

    }
}
