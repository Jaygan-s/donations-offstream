using System;
using System.Threading.Tasks;

namespace StreamerClient
{
    class Program
    {
        static async Task Main(string[] args)
        {
            DonationCatcher catcher = new DonationCatcher();

            await catcher.Begin("YOUR KEY HERE");

            Console.ReadLine();
        }
    }
}
