using System;
using System.Collections.Generic;
using System.Text;

namespace StreamerClient
{
    class YoutubeDonation
    {
        private string id;
        private string start;
        private int duration;

        public static YoutubeDonation CreateInstance(string newID, string newStart, string newDuration)
        {
            YoutubeDonation newObject = new YoutubeDonation();
            newObject.id = newID;
            newObject.start = newStart;
            newObject.duration = Convert.ToInt32(newDuration);
            return newObject;
        }

        public string MakeLink()
        {
            string result = $"https://youtu.be/{id}?t={start}";
            return result;
        }
    }
}
