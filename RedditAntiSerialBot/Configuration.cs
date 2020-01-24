using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RedditAntiSerialBot
{
    public class Configuration
    {
        //Quick settings
        public static bool AllowBotToPostComments = true;

        /*
         * The more subs you monitor. the more the more laggy it gets. I found that 12 is a good number. 
         * 
         * Note that i never asked the moderators for permission to monitor their subreddit. This might be something i should have done but
         * do you really think i have time to message 30+ people and then wait for a reply? Honestly be happy that i am trying to help people on your subreddit.
         * 
         * The AntiSerialCodeBot account got banned from r/steam and r/gaming because they
         * apparently do not appreciate it when you put time and effort into teaching people to not post product keys.
         * 
         * r/PS4 seemed to appreciate it.
         */
        public static List<string> SubsToMonitorList = new List<string>()
        {
            "/r/testingground4bots",
            "/r/pcmasterrace",
            "/r/pcgaming",
            "/r/gaming",
            "/r/Games",
            "/r/buildaPC",
            "/r/PS4",
            "/r/XboxOne",
            "/r/TrueGaming",
            "/r/windows",
            "/r/Windows10",
            "/r/g2a"
        };
    }
}
