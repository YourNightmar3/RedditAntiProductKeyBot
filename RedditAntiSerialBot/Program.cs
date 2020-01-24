using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using RedditSharp;
using RedditSharp.Things;

using static RedditAntiSerialBot.General;
using static RedditAntiSerialBot.Configuration;

namespace RedditAntiSerialBot
{

    /*
     * This Reddit bot is originally made by YourNightmar3.
     * 
     * My Reddit profile: https://www.reddit.com/user/YourNightmar31
     * My GitHub profile: https://github.com/YourNightmar3
     * 
     * This bot was made with the intend to spread knowledge on the posting of product keys (or serial codes) on Reddit.
     * It is generally not a good idea to do this because there are bots that can find these codes automatically and steal them.
     * 
     * Again, SPREAD KNOWLEDGE was the intend of this bot. I realize that once a product key has been found, it has probably also been found by other bots.
     * However, the point is to teach the user who originally posted it not to do it again. The point is not to tell them to remove it from the current post, because it's too late already.
     * 
     * Im specifying this because some people didn't understand that. Looking at you, r/steam mods.
     * 
     * I had always wanted to make a Reddit bot, i just couldnt figure out what it should do. When i got this idea i thought it was fantastic.
     * I spent quite some time on developing this code. The first ever test bot comment was on 01/01/2020. (https://www.reddit.com/r/testingground4bots/comments/ehrr9o/testing_my_first_reddit_bot_in_the_comments/fcl3z10/?context=3)
     * 
     * I was happy to try to help spread awareness. I was quite proud of my code, but some mods just had to bash everything to shit.
     * As of 24/01/2020 i took the bot offline. I cba to deal with this crap.
     * 
     * I added as much comments as i thought was nessecary to try amd help people know what's going on. I hope you can learn something from my code.
     * 
     * PERMISSIONS:
     * - You are allowed to host your own anti product key bot as you please using this code. (Please read the bottom line of this comment section)
     * - You are allowed to use this code as an example to create your own reddit bot.
     * - You are NOT ALLOWED to reupload this code and claim it was you who wrote it.
     * 
     * IF YOU ARE GOING TO HOST YOUR OWN ANTI PRODUCT KEY REDDIT BOT: I do request you to REMOVE my reddit tag from CommentHandler.PostReply. I would rather not get in trouble with any more subreddit mods.
     */

    class Program
    {
        static Stats BotStats;

        /// <summary>
        /// Method used to establish a connection with Reddit and log in.
        /// </summary>
        /// <returns>A Reddit object</returns>
        static Reddit ConnectToReddit()
        {
            var webAgent = new BotWebAgent("USERNAME", "PASSWORD", "CLIENTID", "SECRET", "https://www.nowebsite.com/");
            //This will check if the access token is about to expire before each request and automatically request a new one for you
            //"false" means that it will NOT load the logged in user profile so reddit.User will be null
            var reddit = new Reddit(webAgent, true);

            //Make sure we are not exceeding Reddit API Limits.
            if (SubsToMonitorList.Count > 99)
            {
                throw new Exception("Bots can only monitor up to 100 subreddits at the same time.");
            }

            Log("Successfully connected to Reddit!");
            return reddit;
        }

        /// <summary>
        /// Method used to start the monitoring of every subreddit specified in Configuration.SubsToMonitorList
        /// </summary>
        /// <param name="RedditAPI"></param>
        /// <param name="StatisticLines"></param>
        /// <returns></returns>
        static List<Thread> StartMonitoring(Reddit RedditAPI, int StatisticLines)
        {
            List<Thread> MonitorThreads = new List<Thread>();
            foreach (string sub in Configuration.SubsToMonitorList)
            {
                var MonitorThread = new Thread(() => MonitorSubreddit(RedditAPI, sub, StatisticLines));
                MonitorThreads.Add(MonitorThread);
                MonitorThread.Start();
            }
            return MonitorThreads;
        }

        static void Main(string[] args)
        {
            Console.Title = "Reddit Anti Product Key Bot - Made by YourNightmar3";

            Console.ForegroundColor = ConsoleColor.Yellow;

            Log("Loading stats...");
            BotStats = StatsHandler.LoadStats();

            Log("Connecting to Reddit...");

            BotStats.BotLaunchUTC = DateTime.UtcNow;
            BotStats.BotLaunch = DateTime.Now;

            var RedditAPI = ConnectToReddit();

            Log("Launching Bot...");

            //Handle some statistics work
            BotStats.RepliesPostedAtBotLaunch = BotStats.TotalRepliesPosted;
            BotStats.CommentsReadAtBotLaunch = BotStats.TotalCommentsRead;

            //The amount of statistics we show on the first few lines in the console:
            BotStats.StatisticLines = 7;

            //Now start monitoring Reddit.
            List<Thread> MonitorThreads = StartMonitoring(RedditAPI, BotStats.StatisticLines);

            Log("Starting monitoring view in 10 seconds...");

            //Wait 10 seconds
            Thread.Sleep(10000);

            //Clear the console to show the statistics overview.
            Console.Clear();
            ShowConsoleStatistics(RedditAPI, BotStats.StatisticLines);

            //The following only happens when the user presses ESCAPE

            //Stop monitoring
            StopMonitoring(MonitorThreads, BotStats.StatisticLines);


            Log("Saving configuration file...");
            StatsHandler.SaveStats(BotStats);
            Log("Configuration file saved. Press enter to shut down.");
            Console.ReadLine();
            Environment.Exit(0);
        }

        /// <summary>
        /// Method used to show bot statistics in the console.
        /// </summary>
        /// <param name="RedditAPI"></param>
        /// <param name="StatisticLines"></param>
        static void ShowConsoleStatistics(Reddit RedditAPI, int StatisticLines)
        {
            int loopcount = 0;

            //This is just a loop that refreshes the console every 2 seconds to display the current statistics.
            //The loop will stop when the ESCAPE key is pressed (It only checks every 2 seconds so you have to hold it down a bit)
            while (!(Console.KeyAvailable && Console.ReadKey(true).Key == ConsoleKey.Escape))
            {
                Console.SetCursorPosition(0, 0);
                ulong RepliesPostedThisSession = BotStats.TotalRepliesPosted - BotStats.RepliesPostedAtBotLaunch;
                ulong CommentsReadThisSession = BotStats.TotalCommentsRead - BotStats.CommentsReadAtBotLaunch;
                Console.WriteLine("Press ESC to stop");
                Console.WriteLine("Power on time: " + BotStats.BotLaunch.ToString());
                Console.WriteLine("Amount of subreddits monitored: " + SubsToMonitorList.Count());
                Console.WriteLine($"Posts monitored: {FormatNumber(CommentsReadThisSession)} ({FormatNumber(BotStats.TotalCommentsRead)})");
                Console.WriteLine($"Replies posted: {FormatNumber(RepliesPostedThisSession)} ({FormatNumber(BotStats.TotalRepliesPosted)})");
                Console.WriteLine($"Checked own comments: {BotStats.OwnCommentsChecked} ({BotStats.OwnCommentsDeleted} deleted)");
                Console.WriteLine("Errors: " + FormatNumber(BotStats.RepliesFailed));
                Thread.Sleep(2000);
                loopcount++;
                if (loopcount == 1800) //60 minutes
                {
                    var CheckDownVotesThread = new Thread(() => CheckOwnDownvotes(RedditAPI, StatisticLines));
                    CheckDownVotesThread.Start();
                    loopcount = 0;
                }

            }
        }

        /// <summary>
        /// Method used to abort all open threads that are monitoring subreddits.
        /// </summary>
        /// <param name="MonitorThreads"></param>
        /// <param name="StatisticLines"></param>
        static void StopMonitoring(List<Thread> MonitorThreads, int StatisticLines)
        {
            //Set the cursor position at the bottom of the screen, past all other printed lines.
            Console.SetCursorPosition(0, ((int)(BotStats.TotalRepliesPosted - BotStats.RepliesPostedAtBotLaunch) + (int)BotStats.RepliesFailed + StatisticLines + (int)BotStats.OwnCommentsDeleted) + 2);
            Log("Stopping all threads...");
            foreach (Thread MonitorThread in MonitorThreads)
            {
                MonitorThread.Abort();
            }
            Log("All threads stopped.");
        }

        /// <summary>
        /// Method used to actually monitor all comments from a subreddit.
        /// </summary>
        /// <param name="reddit"></param>
        /// <param name="subtomonitor"></param>
        /// <param name="StatisticLines"></param>
        static void MonitorSubreddit(Reddit reddit, String subtomonitor, int StatisticLines)
        {
            var subreddit = reddit.GetSubreddit(subtomonitor);

            Console.ForegroundColor = ConsoleColor.DarkYellow;
            Log("[" + subtomonitor + "] Gathering moderator names...");
            Console.ForegroundColor = ConsoleColor.Yellow;
            //I don't want the bot to reply to the moderators of subreddits because.. well i assume they know what they are doing.
            List<string> ModListString = new List<string>();
            foreach(ModeratorUser Mod in reddit.GetSubreddit(subtomonitor).Moderators) //So we get a list of moderator usernames
            {
                ModListString.Add(Mod.Name);
            }

            Console.ForegroundColor = ConsoleColor.Yellow;
            Log("[" + subtomonitor + "] Now monitoring.");

            //Start a comment stream to get all comments from the currently monitored subreddit as they are posted.
            foreach (var comment in subreddit.CommentStream)
            {
                //On every comment, call the ProcessComment() function to look through the comment body for product keys.
                CommentHandler.ProcessComment(comment, BotStats.BotLaunchUTC, ModListString, BotStats, subtomonitor);
            }
        }

        /// <summary>
        /// Method used to delete bot comments once they have 5 downvotes (-5 votes total) because when that happens i assume the bot has detected something wrong.
        /// NOTE: In theory this method should work but i have never been able to test it. It's UNTESTED.
        /// </summary>
        /// <param name="reddit"></param>
        /// <param name="StatisticLines"></param>
        static void CheckOwnDownvotes(Reddit reddit, int StatisticLines)
        {
            //The amount of downvotes needed to delete a comment.
            int DeleteVote = -5;
            //Check every comment posted so far
            foreach (Comment comment in reddit.User.Comments)
            {
                //Check if the upvotes are equal or less than the ones needed to delete the comment.
                if (comment.Upvotes <= DeleteVote)
                {
                    //Set the cursor position past all previously printed lines
                    Console.SetCursorPosition(0, (int)(BotStats.TotalRepliesPosted - BotStats.RepliesPostedAtBotLaunch) + (int)BotStats.RepliesFailed + StatisticLines + (int)BotStats.OwnCommentsDeleted);
                    Log("Deleted comment with " + -DeleteVote + " or more downvotes: " + comment.Shortlink);
                    //Delete the comment.
                    comment.Del();
                    //Keep track of it.
                    BotStats.OwnCommentsDeleted++;
                }
            }
            BotStats.OwnCommentsChecked++;
        }
    }
}
