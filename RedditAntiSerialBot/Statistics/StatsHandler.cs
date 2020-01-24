using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RedditAntiSerialBot
{
    public static class StatsHandler
    {

        public static Stats LoadStats()
        {
            //Can't load the file if it doesn't exist ;)
            if (File.Exists("stats.json"))
            {
                //Read the file and return the Stats object with the values read from the file.
                return JsonConvert.DeserializeObject<Stats>(File.ReadAllText("stats.json"));
            }
            return new Stats();
        }

        public static void SaveStats(Stats stats)
        {
            //Write the JSON file with the current stats from the stats object.
            using (StreamWriter outputFile = new StreamWriter("stats.json"))
            {
                outputFile.WriteLine(JsonConvert.SerializeObject(stats));
            }
        }
    }
}
