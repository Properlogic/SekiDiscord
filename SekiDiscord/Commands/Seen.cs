﻿using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;

namespace SekiDiscord.Commands
{
    internal class Seen
    {
        public static Dictionary<string, DateTime> SeenTime { get; set; }

        static Seen()
        {
            SeenTime = ReadSeen();
        }

        public static void MarkUserSeen(string userName)
        {
            if (SeenTime.ContainsKey(userName))
            {
                SeenTime[userName] = DateTime.UtcNow;
            }
            else
            {
                SeenTime.Add(userName, DateTime.UtcNow);
            }
        }

        private static DateTime GetUserSeenUTC(string nick)
        {
            string user = nick.ToLower(CultureInfo.CreateSpecificCulture("en-GB"));
            if (SeenTime.ContainsKey(user))
            {
                return SeenTime[user];
            }
            else
                throw new UserNotSeenException("The user " + user + " has not been seen yet, or an error has occured");
        }

        public static string CheckSeen(string args)
        {
            DateTime seenTime;
            DateTime now = DateTime.UtcNow;
            TimeSpan diff;

            try
            {
                seenTime = GetUserSeenUTC(args);

                diff = now.Subtract(seenTime);
                string timeDiff = string.Empty;

                if (diff.Days >= 1)
                    if (diff.Days == 1)
                        timeDiff += diff.Days + " day, ";
                    else
                        timeDiff += diff.Days + " days, ";

                if (diff.Hours >= 1)
                    if (diff.Hours == 1)
                        timeDiff += diff.Hours + " hour ago";
                    else
                        timeDiff += diff.Hours + " hours ago";
                else
                    if (diff.Minutes == 1)
                    timeDiff += diff.Minutes + " minute ago";
                else
                    timeDiff += diff.Minutes + " minutes ago";

                return "The user " + args + " was last seen " + timeDiff;
            }
            catch (UserNotSeenException ex)
            {
                return ex.Message;
            }
        }

        public class UserNotSeenException : Exception
        {
            public UserNotSeenException()
            {
            }

            public UserNotSeenException(string user) : base(user)
            {
            }

            public UserNotSeenException(string message, Exception innerException) : base(message, innerException)
            {
            }
        }

        public static Dictionary<string, DateTime> ReadSeen()
        {
            Dictionary<string, DateTime> seen = new Dictionary<string, DateTime>();

            if (File.Exists("TextFiles/seen.json"))
            {
                try
                {
                    using (StreamReader r = new StreamReader("TextFiles/seen.json"))
                    {
                        string json = r.ReadToEnd();
                        seen = JsonConvert.DeserializeObject<Dictionary<string, DateTime>>(json);
                    }
                }
                catch (JsonException)
                {
                }
            }

            return seen;
        }

        public static void SaveSeen(Dictionary<string, DateTime> seen)
        {
            try
            {
                using (StreamWriter w = File.CreateText("TextFiles/seen.json"))
                {
                    JsonSerializer serializer = new JsonSerializer();
                    serializer.Serialize(w, seen);
                }
            }
            catch (JsonException)
            {
            }
        }
    }
}