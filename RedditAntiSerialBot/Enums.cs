using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RedditAntiSerialBot
{
    /// <summary>
    /// General class i like to make to keep enums in, so they are not scattered through the project.
    /// </summary>
    public class Enums
    {
        /// <summary>
        /// The type of keys supported by the bot
        /// </summary>
        public enum KeyType
        {
            Steam,
            WindowsStore,
            Windows,
            uPlay
        }

        /// <summary>
        /// The mothod used to convert a KeyType object to a human readable string.
        /// </summary>
        public static Dictionary<KeyType, String> KeyTypeToString = new Dictionary<KeyType, string>()
        {
            { KeyType.Steam, "Steam" },
            { KeyType.WindowsStore, "Windows Store" },
            { KeyType.Windows, "Windows OS" },
            { KeyType.uPlay, "Ubisoft Store (Uplay)" }
        };
    }
}
