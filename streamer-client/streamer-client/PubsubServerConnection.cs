using Websocket;
using Websocket.Client;
using Websocket.Client.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace StreamerClient
{
    class PubsubServerConnection
    {
        private WebsocketClient socket;

        public async Task Begin(string ipAddress)
        {
            /* Create URI for websocket connection */
            string uriString = $"ws://{ipAddress}/streamer"; // !! needs security checks !! for tests only
            Uri uri = new Uri(uriString);

            /* Initialize Websocket client */
            socket = new WebsocketClient(uri);
            socket.ReconnectTimeout = null;
            socket.ReconnectionHappened.Subscribe(OnReconnecting);
            socket.MessageReceived.Subscribe(OnMessageReceived);

            /* Start */
            await socket.Start();

            /* Say hi to server */
            //NewMessage("Hello, Mr. Server!");
        }

        public void NewMessage(string message)
        {
            Console.WriteLine($"[Sending New Message to Pubsub Server]" +
                $"\n\tContent > { message }");
            socket.Send(message);
        }

        private void OnReconnecting(ReconnectionInfo info)
        {
            Console.WriteLine($"[Connecting Pubsub Server... Type: {info.Type}]");
        }

        private void OnMessageReceived(ResponseMessage message)
        {
            Console.WriteLine($"[Received Message From Pubsub Server] " +
                $"\n\tType:{ message.MessageType }" +
                $"\n\tContent > {message.Text}");
        }
    }
}
