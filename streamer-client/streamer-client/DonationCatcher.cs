using Websocket;
using Websocket.Client;
using Websocket.Client.Models;
using NSoup;
using NSoup.Helper;
using NSoup.Nodes;
using NSoup.Select;
using System;
using System.IO;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;


namespace StreamerClient
{
    class DonationCatcher
    {
        private WebsocketClient socket;
        public event EventHandler<YoutubeDonation> OnYoutubeDonation;


        public async Task Begin(string key)
        {
            /* Get version and token information */
            Document doc = HttpConnection.Connect($"https://twip.kr/widgets/alertbox/{key}").Get();
            Elements scripts = doc.GetElementsByTag("script");
            string script = scripts.ToString();

            string version = GetTwipVersion(script);
            string token = GetTwipToken(script);
            
            if(version == null)
            {
                Console.WriteLine("오류 : [버전 찾을 수 없음]"); // message says "version not found"
            }
            if (token == null)
            {
                Console.WriteLine("오류 : [토큰 찾을 수 없음]"); // message says "token not found"
            }

            /* Create URI for websocket connection */
            string uriString = $"wss://io.mytwip.net/socket.io/?" +
                $"alertbox_key={key}&" +
                $"version={version}&" +
                $"token={EncodeURIComponent(token)}&" +
                $"EIO=3&" +
                $"transport=websocket";
            Uri uri = new Uri(uriString);

            /* Initialize Websocket client */
            socket = new WebsocketClient(uri);
            socket.ReconnectTimeout = null;
            socket.DisconnectionHappened.Subscribe(OnDisconnected);
            socket.ReconnectionHappened.Subscribe(OnReconnecting);
            socket.MessageReceived.Subscribe(OnMessageReceived);

            /* Start */
            await socket.Start();
        }

        private void OnDisconnected(DisconnectionInfo info)
        {
            Console.WriteLine($"[Disconnected. Reason: {(info.Exception != null ? info.Exception.Message : "(None)")}]");
        }
        private void OnReconnecting(ReconnectionInfo info)
        {
            Console.WriteLine($"[Connecting... Type: {info.Type}]");
        }
        private void OnMessageReceived(ResponseMessage message)
        {
            Console.WriteLine($"[Received Message] \n\tType:{ message.MessageType}\n\tContent > {message.Text}");
            Parse(message.Text);
        }

        private void Parse(string str)
        {
            // TO-DO 1: add handler for these messages:
            // 1. media:playing  : info(video id, start time, duration...) of incoming video donation. 
            // 2. media:show     : to toggle on player
            // 3. media:started  : to hit the play button
            // 4. media:finished : to stop playing
            // TO-DO 2: encapsulate these up and put somewhere else since this is just a Parse function.

            // handler for media:playing
            if (str.Contains("media:playing"))
            {
                if (RegexMatchFromString(str, @"""type"":""(.{0,15})"",") == "youtube")
                {
                    // NOTE: needs better parsing
                    string id = RegexMatchFromString(str, @"""id"":""(.{1,15})"",");
                    string start = RegexMatchFromString(str, @"""start"":(\d*),");
                    string duration = RegexMatchFromString(str, @"""duration"":(\d*),");

                    // on parsing success, call Youtube donation event handler
                    if(id != null && start != null && duration != null)
                    {
                        YoutubeDonation donation = YoutubeDonation.CreateInstance(id, start, duration);
                        if(OnYoutubeDonation != null)
                        {
                            OnYoutubeDonation(this, donation);
                        }
                    }
                }
            }
        }

        private static string RegexMatchFromString(string input, string pattern)
        {
            Match m = Regex.Match(input, pattern);
            if (m.Success)
            {
                return m.Groups[1].Value;
            }
            return null;
        }

        private static string GetTwipVersion(string inputHTML)
        {
            string pattern = @"version: '(.*)'";
            return RegexMatchFromString(inputHTML, pattern);
        }

        private static string GetTwipToken(string inputHTML)
        {
            string pattern = @"window.__TOKEN__ = '(.*)'";
            return RegexMatchFromString(inputHTML, pattern);
        }

        private static string EncodeURIComponent(string s)
        {
            return System.Web.HttpUtility.UrlEncode(s, Encoding.UTF8);
        }
    }

}
