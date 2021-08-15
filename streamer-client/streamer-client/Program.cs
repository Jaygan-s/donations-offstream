using System;
using System.Threading.Tasks;

namespace StreamerClient
{
    static class Program
    {
        static DonationCatcher catcher;
        static PubsubServerConnection pubsub;

        static int Main(string[] args)
        {
            /* Initialize */
            InitializeClientInstance();
            catcher.OnYoutubeDonation += new EventHandler<YoutubeDonation>(YoutubeDonationHandler);
            
            /* Run Clients */
            BeginClients("YOUR_ALERTBOX_KEY_HERE", "localhost:3000"); // your pubsub server address at second param

            /* Pauses console */
            Console.ReadLine();
            return 0;
        }

        static void InitializeClientInstance()
        {
            catcher = new DonationCatcher();
            pubsub = new PubsubServerConnection();
        }

        static async void BeginClients(string alertboxKey, string pubsubBrokerAddress)
        {
            await catcher.Begin(alertboxKey);
            await pubsub.Begin(pubsubBrokerAddress);
        }

        static void YoutubeDonationHandler(object sender, YoutubeDonation donationInfo)
        {
            // this is just a simple payload for testing.
            //pubsub.NewMessage($"OnYoutubeDonation[link:{donationInfo.MakeLink()}]");
            pubsub.NewMessage(donationInfo.GetID());
        }
    }
}
