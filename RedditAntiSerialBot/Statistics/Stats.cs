using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RedditAntiSerialBot
{
    public class Stats
    {
        //Statistics which are saved to a JSON file
        public ulong TotalCommentsRead = 0;
        public ulong TotalRepliesPosted = 0;
        public ulong ProductKeysFound = 0;


        //The following are statistics only for this session. They do not need to be saved into the JSON file.
        [JsonIgnore]
        public ulong RepliesPostedAtBotLaunch = 0;
        [JsonIgnore]
        public ulong CommentsReadAtBotLaunch = 0;
        [JsonIgnore]
        public ulong RepliesFailed = 0;
        [JsonIgnore]
        public ulong OwnCommentsChecked = 0;
        [JsonIgnore]
        public ulong OwnCommentsDeleted = 0;
        [JsonIgnore]
        public DateTime BotLaunchUTC;
        [JsonIgnore]
        public DateTime BotLaunch;
        [JsonIgnore]
        public int StatisticLines = 0;
    }
}
