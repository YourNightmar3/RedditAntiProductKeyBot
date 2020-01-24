using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RedditAntiSerialBot
{
    public static class General
    {
        /// <summary>
        /// A log method that for now only writes everything to the console.
        /// I thought it would be nice to write everything to a log file as well but never got to it.
        /// </summary>
        /// <param name="logline"></param>
        /// <param name="WriteToLogs"></param>
        public static void Log(String logline, bool WriteToLogs = true)
        {
            Console.WriteLine(DateTime.Now + " - " + logline);
        }

        /// <summary>
        /// Small method to put comma seperators in big numbers. I believe i got this from the internet somewhere.
        /// </summary>
        /// <param name="number"></param>
        /// <returns></returns>
        public static string FormatNumber(ulong number)
        {
            return String.Format("{0:#,##0}", number);
        }
    }
}
