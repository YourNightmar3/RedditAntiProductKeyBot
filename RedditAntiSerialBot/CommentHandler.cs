using RedditSharp.Things;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using static RedditAntiSerialBot.General;

namespace RedditAntiSerialBot
{
    public static class CommentHandler
    {
        /// <summary>
        /// Method to "scan" a comment for product keys.
        /// </summary>
        /// <param name="comment">The comment to "scan".</param>
        /// <returns></returns>
        public static Dictionary<String, List<Enums.KeyType>> getProductKeysFromComment(Comment comment)
        {
            //The object to return in the format <string, List<KeyType>>
            Dictionary<String, List<Enums.KeyType>> ProductKeys = new Dictionary<string, List<Enums.KeyType>>();

            //Regex patterns to match the different product keys. I have spent a lot of time in getting these the way they are.
            //There is probably a way to do it easier/smaller but this seems to work decently well. I tested them with https://regexr.com/3b60q
            Dictionary<List<Enums.KeyType>, String> RegexPatterns = new Dictionary<List<Enums.KeyType>, string>()
            {
                {new List<Enums.KeyType>{ Enums.KeyType.Steam }, @"(?<![\w-!@#$%^&*()_+\-=\[\]{};':\\|,.<>\/?])([A-Z0-9]{4,6}-[A-Z0-9]{4,6}-[A-Z0-9]{4,6})(?![\w-!@#$%^&*()_+\-=\[\]{};':\\|<>\/?])"},
                {new List<Enums.KeyType>{ Enums.KeyType.WindowsStore, Enums.KeyType.Windows }, @"(?<![\w-!@#$%^&*()_+\-=\[\]{};':\\|,.<>\/?])([A-Z0-9]{5}-[A-Z0-9]{5}-[A-Z0-9]{5}-[A-Z0-9]{5}-[A-Z0-9]{5})(?![\w-!@#$%^&*()_+\-=\[\]{};':\\|<>\/?])"},
                {new List<Enums.KeyType>{ Enums.KeyType.uPlay }, @"(?<![\w-!@#$%^&*()_+\-=\[\]{};':\\|,.<>\/?])([A-Z]{4}-[A-Z]{4}-[A-Z]{4}-[A-Z]{4})(?![\w-!@#$%^&*()_+\-=\[\]{};':\\|<>\/?])" },
                {new List<Enums.KeyType>{ Enums.KeyType.uPlay }, @"(?<![\w-!@#$%^&*()_+\-=\[\]{};':\\|,.<>\/?])([A-Z]{3}-[A-Z]{4}-[A-Z]{4}-[A-Z]{4}-[A-Z]{4})(?![\w-!@#$%^&*()_+\-=\[\]{};':\\|<>\/?])" }
            };

            //Check the current comment against each regex pattern.
            foreach (KeyValuePair<List<Enums.KeyType>, String> RegexPattern in RegexPatterns)
            {
                Regex rgx = new Regex(RegexPattern.Value);
                foreach (Match match in rgx.Matches(comment.Body))
                {
                    //Check if the product key contains a letter. If not, it's probably a phone number ;)
                    if (match.Value.Any(x => char.IsLetter(x)))
                    {
                        //Check if this product key wasn't already listed. The user may have posted the same key twice.
                        if (!ProductKeys.ContainsKey(match.Value.Trim()))
                        {
                            //Remember the product key with the type so we can tell the user we found it and matched it.
                            ProductKeys.Add(match.Value.Trim(), RegexPattern.Key);
                        }
                    }
                }
            }

            return ProductKeys;
        }

        /// <summary>
        /// Method to call that will find product keys, then post a reply if product keys were found.
        /// </summary>
        /// <param name="comment"></param>
        /// <param name="BotLaunchUTC"></param>
        /// <param name="ModListString"></param>
        /// <param name="BotStats"></param>
        /// <param name="currentsub"></param>
        public static void ProcessComment(Comment comment, DateTime BotLaunchUTC, List<string> ModListString, Stats BotStats, String currentsub)
        {
            //We don't want to comment on our own comments and also not on the comments of subreddit moderators because we assume they know what they are doing.
            if (comment.AuthorName != "AntiSerialCodeBot" && !ModListString.Contains(comment.AuthorName))
            {
                //Make sure it doesn't reply to comments before the bot was launched. This shouldn't happen but i like having this check in here anyway.
                if (comment.CreatedUTC.DateTime > BotLaunchUTC)
                {
                    //Keep some stats.
                    BotStats.TotalCommentsRead++;

                    //Get all product keys from the comment.
                    Dictionary<String, List<Enums.KeyType>> ProductKeys = CommentHandler.getProductKeysFromComment(comment);

                    //STATISTICS ARE GREAT
                    BotStats.ProductKeysFound += (ulong)ProductKeys.Count;

                    if (ProductKeys.Count != 0)
                    {
                        if (Configuration.AllowBotToPostComments)
                        {
                            try
                            {
                                BotStats.TotalRepliesPosted++;
                                //Post a reply on the comment that contains the product key(s)
                                PostReply(comment, BotStats, ProductKeys, currentsub);
                            }
                            catch (Exception e)
                            {
                                //Set the cursor at the bottom of the console past all other lines we already printed this session
                                Console.SetCursorPosition(0, (int)(BotStats.TotalRepliesPosted - BotStats.RepliesPostedAtBotLaunch) + (int)BotStats.RepliesFailed + BotStats.StatisticLines + (int)BotStats.OwnCommentsDeleted); //We do +4 because there is 4 lines of statistics at the top of the console.
                                //Log the error message
                                Log(e.Message + " - " + comment.Shortlink);
                                //S T A T I S T I C S
                                BotStats.RepliesFailed++;
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Method to post a reply to a comment on reddit
        /// </summary>
        /// <param name="comment">The comment to post a reply on</param>
        /// <param name="BotStats">Stats object to keep statistics</param>
        /// <param name="ProductKeys">The product keys that were found in the comment object passed through</param>
        /// <param name="subtomonitor">The current subreddit</param>
        public static void PostReply(Comment comment, Stats BotStats, Dictionary<String, List<Enums.KeyType>> ProductKeys, string subtomonitor)
        {

            //Note: This method always needs to be called within a Try catch statement

            /*
             * Honestly this is so garbage formatted but i don't know of a way to improve it. The output on Reddit looks great.
             * Examples:
             * https://prnt.sc/qsb7xy (https://www.reddit.com/r/pcgaming/comments/emp4k0/rock_of_ages_3_closed_alpha_details_invitations/fdr71qu/?context=3)
             * https://www.reddit.com/r/testingground4bots/comments/ehrr9o/testing_my_first_reddit_bot_in_the_comments/fcv1f43/?context=3
             */

            String KeyTable = GenerateTableOutput(ProductKeys);
            comment.Reply("Hey there u/" + comment.AuthorName + "!"
                                + "\r\n\r\nIt seems like you have posted a product key of some sorts."
                                + "\r\nBots like me can scrape Reddit and automatically activate such keys. This is why I recommend not to share serial codes without inserting a change that is easy to solve for a human, but hard to interpret for a bot.\r\n\r\nThis is usually done by putting a question mark in the product key and, for example, writing ? = 4+3. Or better yet: if your product key is meant for one specific redditor, you can PM it to them!"
                                + "\r\n\r\nFound product keys:\r\n\r\n"
                                + KeyTable
                                + "\r\n\r\n^This ^bot ^is ^in ^it's ^testing ^phase ^- ^Currently ^monitoring ^" + Configuration.SubsToMonitorList.Count() + " ^subreddits ^- ^Scanned ^" + FormatNumber(BotStats.TotalCommentsRead - BotStats.CommentsReadAtBotLaunch) + " ^commments ^this ^session, ^" + FormatNumber(BotStats.TotalCommentsRead) + " ^comments ^total ^- ^I ^helped ^protect ^" + BotStats.ProductKeysFound + " ^product ^keys ^- ^Created ^by [^(u/YourNightmar31)](https://www.reddit.com/user/YourNightmar31) ^- ^Please ^upvote ^if ^right ^and ^downvote ^if ^wrong.");
            
            //Set the cursor at the bottom past all previous lines we printed
            Console.SetCursorPosition(0, (int)(BotStats.TotalRepliesPosted - BotStats.RepliesPostedAtBotLaunch) + (int)BotStats.RepliesFailed + BotStats.StatisticLines + (int)BotStats.OwnCommentsDeleted); //We do +statisticlines because there is 4 lines of statistics at the top of the console.
           
            //Output that we successfully posted a comment. WOOO!
            Log("[" + subtomonitor + "] New Comment posted by " + comment.AuthorName + " - Posted Reply!");
        }

        /// <summary>
        /// Method used to generate the table to put in Reddit comments.
        /// </summary>
        /// <param name="ProductKeys">Dictionary with all found product keys</param>
        /// <returns></returns>
        static string GenerateTableOutput(Dictionary<String, List<Enums.KeyType>> ProductKeys)
        {
            //This might be even worse than the previous method. This took a lot of trial and error.

            //Making all found serial codes into a single string
            String output = "|Product Key|Pattern matches\r\n|:-|:-|";
            foreach (KeyValuePair<String, List<Enums.KeyType>> ProductKey in ProductKeys)
            {
                output += "\r\n|" + CensorKey(ProductKey.Key) + "|";
                bool first = true;
                foreach (Enums.KeyType ProductKeyType in ProductKey.Value)
                {
                    //Ugh i kinda hate this but it's to make it prettier
                    if (first)
                    {
                        first = false;
                        output += Enums.KeyTypeToString[ProductKeyType];
                    }
                    else
                    {
                        output += ", " + Enums.KeyTypeToString[ProductKeyType];
                    }
                }
            }
            output.Trim();
            return output;
        }

        /// <summary>
        /// Method used to censor serial keys like ABC-XXX-XXX
        /// </summary>
        /// <param name="ProductKey">The product key to censor</param>
        /// <returns></returns>
        private static string CensorKey(string ProductKey)
        {
            //Split the product key by dashes
            String[] keySplit = ProductKey.Split('-');

            /*
             * This loop is used just to censor part of the product key. For example:
             * 
             * Input: BL57P-CFQ59-CYMG4
             * Output: BL57P-XXXXX-XXXXX
             * 
             * Input: FJGHD-DKFGE-EHDKF-DJWKE-JDKFJ
             * Output: FJGHD-XXXXX-XXXXX-XXXXX-XXXXX
             * 
             * I wanted to do this to prove to people that it can actually find and read the product keys in comments.
             */

            String censoredKey = "";
            for (int i = 0; i < keySplit.Count(); i++)
            {
                //We only show the first segment of the code
                if (i == 0)
                {
                    censoredKey += keySplit[i];
                }
                else
                {
                    //And censor the rest
                    censoredKey = censoredKey + "-";
                    foreach (Char character in keySplit[i])
                    {
                        censoredKey += "X";
                    }
                }

            }
            return censoredKey;
        }
    }
}
