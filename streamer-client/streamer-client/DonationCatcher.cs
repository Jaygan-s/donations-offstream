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
                Console.WriteLine("오류 : [버전 찾을 수 없음]");
            }
            if (token == null)
            {
                Console.WriteLine("오류 : [토큰 찾을 수 없음]");
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

        private static void OnDisconnected(DisconnectionInfo info)
        {
            Console.WriteLine($"[Disconnected. Reason: {(info.Exception != null ? info.Exception.Message : "(None)")}]");
        }
        private static void OnReconnecting(ReconnectionInfo info)
        {
            Console.WriteLine($"[Re-connecting... Type: {info.Type}]");
        }
        private static void OnMessageReceived(ResponseMessage message)
        {
            Console.WriteLine($"[Received Message] \n\tType:{ message.MessageType}\n\tContent > {message.Text}");
            Parse(message.Text);
        }

        private static void Parse(string str)
        {
            if(str.Contains("media:playing"))
            {
                if (RegexMatchFromString(str, @"""type"":""(.{0,15})"",") == "youtube")
                {
                    // NOTE: needs better parsing
                    string id = RegexMatchFromString(str, @"""id"":""(.{1,15})"",");
                    string start = RegexMatchFromString(str, @"""start"":(\d*),");
                    string duration = RegexMatchFromString(str, @"""duration"":(\d*),");

                    // on parsing success
                    if(id != null && start != null && duration != null)
                    {
                        YoutubeDonation donation = YoutubeDonation.CreateInstance(id, start, duration);
                        string link = donation.MakeLink();
                        bool a = true;
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
