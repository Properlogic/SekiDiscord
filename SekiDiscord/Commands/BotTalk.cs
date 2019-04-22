using Cleverbot.Net;
using DSharpPlus.Entities;
using DSharpPlus.EventArgs;
using edu.stanford.nlp.parser.lexparser;
using edu.stanford.nlp.process;
using edu.stanford.nlp.trees;
using java.io;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml;

namespace SekiDiscord.Commands
{
    internal class BotTalk
    {
        private static string[] howMany = { "I dont know, maybe", "Probably", "More than", "Less than", "I think it was", "I don't know, so i'll give you a random number:", "", "It's" };
        private static string[] howIs = { "fine", "not fine", "lost", "being retarded again", "not feeling well", "being annoying as always", "probably hungry", "good", "all right", "upset", "bored" };

        private static string[] because = { "was lost", "is stupid", "asked me to", "was asked to", "has an inferiority complex", "is a terrible person",
                                    "felt like so", "wanted to", "liked it", "already had plans to do it", "wanted it that way"  };

        private static string[] when = { "maybe next week", "a few days ago", "last year", "yesterday", "tomorrow", "in a few hours",
                                "nobody knows", "next year", "it was yesterday", "I'm not sure", "next week" };

        private static string[] why = { "I dont know, maybe", "I don't know", "Yeah", "Nope.", "Yes.", "No.", "Probably", "Everything makes me believe so",
                                "Not sure, ask somebody else", "I don't know, im not wikipedia", "Sorry, but i don't know", "Because that was destined to be so" };

        private static string[] where = { "somewhere in a far away land" , "on the Youtube datacenter", "behind you", "in your house", "in Europe", "near Lygs", "that special place",
                                "in outer space","somewhere i belong", "On the shaddiest subreddit","on tumblr", "in space", "on your computer",
                                "beneath your bed!", "where you didnt expect", "near your house"};

        private static string[] whoDid = { "Probably", "Maybe it was", "I'm sure it was", "I don't think it was", "I suspect", "Someone told me it was", "I think it was" };
        private static string[] whoDo = { "Probably", "Maybe it is", "I'm sure it is", "I don't think it was", "I suspect", "Someone told me it is", "I think it is" };

        private static string[] what = { "Sorry, I have no idea","Can you ask somebody else? I really don't know","No idea, try Google", "I'm not good with questions",
                            "I'm under pressure, i can't answer you that","Stop bullying me!" };

        private static string[] whyY = { "Im not sure if", "Yeah,", "Yes,", "Correct!", "I think", "I believe that", "" };
        private static string[] whyN = { "Nope,", "No,", "I think that", "I believe that", "Negative!", "" };

        static private CleverbotSession cleverbotSession = null;
        static private Questions questions;

        public static async Task BotThink(MessageCreateEventArgs e, StringLibrary stringLibrary, string botName)
        {
            string message;

            if (string.IsNullOrWhiteSpace(Settings.Default.CleverbotAPI))
                return;

            string input = e.Message.Content;

            //Remove bot name from message input
            if (input.StartsWith(botName, StringComparison.OrdinalIgnoreCase))
            {
                input = input.Substring(input.IndexOf(',') + 1).Trim();
            }
            else if (input.TrimEnd().EndsWith(botName, StringComparison.OrdinalIgnoreCase))
            {
                input = input.Substring(0, input.LastIndexOf(botName, StringComparison.OrdinalIgnoreCase)).Trim();
            }

            await e.Channel.TriggerTypingAsync();

            try
            {
                if (cleverbotSession == null)
                    cleverbotSession = new CleverbotSession(Settings.Default.CleverbotAPI);

                CleverbotResponse answer = await cleverbotSession.GetResponseAsync(input);
                message = answer.Response;
            }
            catch
            {
                message = "Sorry, but i can't think right now";
                cleverbotSession = new CleverbotSession(Settings.Default.CleverbotAPI);
            }

            await e.Message.RespondAsync(message);
        }

        public static async Task BotAnswer(MessageCreateEventArgs e, StringLibrary stringLibrary, string botName)
        {
            //TODO: This needs to go or be simplified
            string input = e.Message.Content;
            string user = ((DiscordMember)e.Message.Author).DisplayName;
            string message = null;
            string subjectNLP;
            Random r = new Random();
            List<DiscordMember> listU = Useful.getOnlineUsers(e.Channel.Guild);

            string arg;
            try
            {
                arg = input.Split(new char[] { ' ' }, 2)[1];
            }
            catch
            {
                arg = string.Empty;
            }

            //Let chat know that the bot is thinking...
            await e.Channel.TriggerTypingAsync();

            //Easter egg
            if (arg.ToLower().Trim().Contains("play despacito"))
            {
                message = "https://www.youtube.com/watch?v=kJQP7kiw5Fk";
                await e.Message.RespondAsync(message);
                return;
            }

            try
            {
                //Get the subject of the phrase using NLP
                if (questions == null)
                    questions = new Questions();

                subjectNLP = questions.GetSubject(arg);
            }
            catch (Exception ex)
            {
                System.Console.WriteLine(DateTime.Now.ToString("[HH:mm:ss] ") + "Error: " + ex.Message);
                subjectNLP = null;
            }

            // End with ?
            if (arg[arg.Length - 1] == '?')
            {
                arg = arg.Replace("?", string.Empty).TrimStart(new char[] { ' ' });
                string[] split = arg.Split(new char[] { ' ' });

                if (split.Length >= 1)
                {
                    if (string.Compare(split[0], "how", true) == 0)
                    {
                        if (split.Length >= 2)
                        {
                            if (string.Compare(split[1], "many", true) == 0)
                            {
                                if (string.Compare(arg, "how many killstrings do you have", true) == 0 || string.Compare(arg, "how many kills do you have", true) == 0)
                                    message = "I have " + stringLibrary.Kill.Count + " killstrings loaded in.";
                                else if (arg == "how many fucks do you give")
                                    message = "I always give 0 fucks.";
                                else
                                    message = (howMany[r.Next(howMany.Length)] + " " + r.Next(21)).Trim();
                            }
                            else if (split.Length >= 2 && string.Compare(split[1], "are", true) == 0)
                            {
                                if (string.Compare(arg, "how are you", true) == 0 || string.Compare(arg, "how are you doing", true) == 0)
                                    message = "I'm fine, thanks for asking. And you?";
                                else
                                    message = "I dont know yet, ask later";
                            }
                            else if (string.Compare(split[1], "is", true) == 0)
                            {
                                if (split.Length == 3)
                                    message = split[2] + " is " + howIs[r.Next(howIs.Length)];
                            }
                            else if (string.Compare(split[1], "did", true) == 0 && string.Compare(split[split.Length], "die", true) == 0)
                            {
                                KillUser.Kill(e, stringLibrary, Useful.GetBetween(arg, "did", "die"));
                                return;
                            }
                            else if (string.Compare(split[1], "old", true) == 0 && subjectNLP != null)
                            {
                                if (split.Length >= 4)
                                {
                                    string replaced = QuestionsRegex(subjectNLP);

                                    if (string.Compare(split[2], "is", true) == 0)
                                    {
                                        message = replaced + " is " + r.Next(41) + " years old";
                                    }
                                    else if (string.Compare(split[2], "are", true) == 0)
                                    {
                                        if (string.Compare(subjectNLP, "you", true) == 0)
                                            message = "I was compiled on " + GetCompilationDate.RetrieveLinkerTimestamp().ToString("R");
                                        else
                                            message = replaced + " are " + r.Next(41) + " years old";
                                    }
                                    else if (string.Compare(split[2], "am", true) == 0 || string.Compare(split[3], "i", true) == 0)
                                        message = "You are " + r.Next(41) + " years old";
                                }
                            }
                        }
                        else
                            message = user + ", no idea...";
                    }
                    else if (string.Compare(split[0], "how's", true) == 0 && subjectNLP != null)
                    {
                        string replaced = QuestionsRegex(subjectNLP);
                        message = replaced + " is " + howIs[r.Next(howIs.Length)];
                    }
                    else if (string.Compare(split[0], "why", true) == 0)
                    {
                        if (split.Length >= 2)
                            message = "Because " + listU[r.Next(listU.Count)].DisplayName + " " + because[r.Next(because.Length)];
                    }
                    else if (string.Compare(split[0], "is", true) == 0 && subjectNLP != null)
                    {
                        bool yes = false;

                        if (r.Next(0, 2) == 1)
                            yes = true;

                        if (split.Length >= 2)
                        {
                            string subject = subjectNLP;
                            string rest = "";

                            for (int i = 1; i < split.Length; i++)
                            {
                                rest += split[i] + " ";
                            }
                            rest = rest.TrimEnd(' ').Replace(subjectNLP, string.Empty);

                            string replaced = QuestionsRegex(rest);

                            if (yes)
                                message = (whyY[r.Next(whyY.Length)] + " " + subject.Replace("your", "my") + " is " + replaced).Trim();
                            else
                                message = (whyN[r.Next(whyN.Length)] + " " + subject.Replace("your", "my") + " isn't " + replaced).Trim();
                        }
                    }
                    else if (string.Compare(split[0], "was", true) == 0 && subjectNLP != null)
                    {
                        bool yes = false;

                        if (r.Next(0, 2) == 1)
                            yes = true;

                        if (split.Length >= 2)
                        {
                            string subject = subjectNLP;
                            string rest = "";

                            for (int i = 1; i < split.Length; i++)
                            {
                                rest += split[i] + " ";
                            }
                            rest = rest.TrimEnd(' ').Replace(subjectNLP, string.Empty);

                            string replaced = QuestionsRegex(rest);

                            if (yes)
                                message = (whyY[r.Next(whyY.Length)] + " " + subject.Replace("your", "my").Replace("Your", "my") + " was " + replaced).Trim();
                            else
                                message = (whyN[r.Next(whyN.Length)] + " " + subject.Replace("your", "my").Replace("Your", "my") + " wasn't " + replaced).Trim();
                        }
                    }
                    else if (string.Compare(split[0], "when", true) == 0)
                    {
                        message = when[r.Next(when.Length)];
                    }
                    else if (string.Compare(split[0], "are", true) == 0)
                    {
                        if (string.Compare(arg, "are you real", true) == 0)
                            message = "Yes, i am real";
                        else if (string.Compare(arg, "are you a real person", true) == 0 || string.Compare(arg, "are you a real human", true) == 0 || string.Compare(arg, "are you human", true) == 0)
                            message = "No, i'm a bot";
                        else
                        {
                            if (r.Next(100) < 15)
                                message = why[r.Next(why.Length)];
                            else if (subjectNLP != null)
                            {
                                bool yes = false;
                                if (r.Next(0, 2) == 1)
                                    yes = true;

                                string subject = subjectNLP;
                                string rest = "";

                                for (int i = 1; i < split.Length; i++)
                                {
                                    rest += split[i] + " ";
                                }
                                rest = rest.TrimEnd(' ').Replace(subjectNLP, string.Empty);

                                string replaced = QuestionsRegex(rest);

                                if (subject.Trim() == "you")
                                {
                                    if (yes)
                                        message = (whyY[r.Next(whyY.Length)] + " " + "I'm " + replaced).Trim();
                                    else
                                        message = (whyN[r.Next(whyN.Length)] + " " + "I'm not " + replaced).Trim();
                                }
                                else
                                {
                                    subject = QuestionsRegex(subject);

                                    if (yes)
                                        message = (whyY[r.Next(whyY.Length)] + " " + subject + " are " + replaced).Trim();
                                    else
                                        message = (whyN[r.Next(whyN.Length)] + " " + subject + " aren't " + replaced).Trim();
                                }
                            }
                        }
                    }
                    else if (string.Compare(split[0], "can", true) == 0)
                    {
                        if (string.Compare(arg, "can you give me a nick", true) == 0 || string.Compare(arg, "can you make me a nick", true) == 0 ||
                            string.Compare(arg, "can you generate a nick", true) == 0 || string.Compare(arg, "can you create a nick", true) == 0 ||
                            string.Compare(arg, "can you make me a new nick", true) == 0)
                        {
                            message = "Yes, here it is: " + NickGenerator.GenerateNick(stringLibrary.NickGenStrings, stringLibrary.NickGenStrings.Count, false, false, false, false);
                        }
                        else if (arg.ToLower().Contains("can you kill "))
                        {
                            KillUser.Kill(e, stringLibrary, Useful.GetBetween(arg.ToLower(), "can you kill ", ""));
                            return;
                        }
                        else
                            message = why[r.Next(why.Length)];
                    }
                    else if (string.Compare(split[0], "would", true) == 0)
                    {
                        if (string.Compare(arg, "would you make me a nick", true) == 0 || string.Compare(arg, "would you generate a nick", true) == 0 ||
                            string.Compare(arg, "would you create a nick", true) == 0 || string.Compare(arg, "would you make me a new nick", true) == 0)
                            message = "Yes, here it is: " + NickGenerator.GenerateNick(stringLibrary.NickGenStrings, stringLibrary.NickGenStrings.Count, false, false, false, false);
                        else
                            message = why[r.Next(why.Length)];
                    }
                    else if (string.Compare(split[0], "where", true) == 0)
                    {
                        message = where[r.Next(where.Length)];
                    }
                    else if (string.Compare(split[0], "who", true) == 0 || string.Compare(split[0], "who's", true) == 0)
                    {
                        if (string.Compare(arg, "who are you", true) == 0)
                            message = "I'm a bot!";
                        else if (split[1] == "do")
                            message = whoDid[r.Next(whoDo.Length)] + " " + listU[r.Next(listU.Count)].DisplayName;
                        else
                            message = whoDid[r.Next(whoDid.Length)] + " " + listU[r.Next(listU.Count)].DisplayName;
                    }
                    else if (string.Compare(split[0], "what", true) == 0 || string.Compare(split[0], "what's", true) == 0)
                    {
                        if (string.Compare(arg, "what are you", true) == 0)
                            message = "I'm a bot!";
                        else
                        {
                            message = what[r.Next(what.Length)];
                        }
                    }
                    else if (string.Compare(split[0], "if", true) == 0)
                    {
                        await BotThink(e, stringLibrary, botName);
                    }
                    else if (string.Compare(split[0], "am", true) == 0 && string.Compare(split[1], "i", true) == 0 && subjectNLP != null)
                    {
                        bool yes = false;

                        if (r.Next(0, 2) == 1)
                            yes = true;

                        string rest = "";

                        for (int i = 1; i < split.Length; i++)
                        {
                            rest += split[i] + " ";
                        }
                        rest = rest.TrimEnd(' ').Replace(subjectNLP, string.Empty);

                        string replaced = QuestionsRegex(rest);

                        if (yes)
                            message = (whyY[r.Next(whyY.Length)] + " you are " + replaced).Trim();
                        else
                            message = (whyN[r.Next(whyN.Length)] + " you aren't " + replaced).Trim();
                    }
                    else if (string.Compare(split[0], "do", true) == 0 && subjectNLP != null)
                    {
                        bool yes = false;
                        if (r.Next(0, 2) == 1)
                            yes = true;

                        if (split.Length >= 2)
                        {
                            string subject = subjectNLP;
                            string rest = "";

                            for (int i = 1; i < split.Length; i++)
                            {
                                rest += split[i] + " ";
                            }
                            rest = rest.TrimEnd(' ').Replace(subjectNLP, string.Empty);

                            string replaced = QuestionsRegex(rest);

                            if (string.Compare(split[1], "you", true) == 0)
                            {
                                if (yes)
                                    message = (whyY[r.Next(whyY.Length)] + " " + "I " + replaced).Trim();
                                else
                                    message = (whyN[r.Next(whyN.Length)] + " " + "I don't " + replaced).Trim();
                            }
                            else if (string.Compare(split[1], "i", true) == 0)
                            {
                                if (yes)
                                    message = (whyY[r.Next(whyY.Length)] + " " + "you do " + replaced).Trim();
                                else
                                    message = (whyN[r.Next(whyN.Length)] + " " + "you don't " + replaced).Trim();
                            }
                            else
                            {
                                if (yes)
                                    message = (whyY[r.Next(whyY.Length)] + " " + subject + " do " + replaced).Trim();
                                else
                                    message = (whyN[r.Next(whyN.Length)] + " " + subject + " doesn't " + replaced).Trim();
                            }
                        }
                    }
                    else if (string.Compare(split[0], "should", true) == 0 && subjectNLP != null)
                    {
                        bool yes = false;
                        if (r.Next(0, 2) == 1)
                            yes = true;

                        if (split.Length >= 2)
                        {
                            string subject = subjectNLP;
                            string rest = "";

                            for (int i = 1; i < split.Length; i++)
                            {
                                rest += split[i] + " ";
                            }
                            rest = rest.TrimEnd(' ').Replace(subjectNLP, string.Empty);

                            string replaced = QuestionsRegex(rest);

                            if (string.Compare(split[1], "you", true) == 0)
                            {
                                if (yes)
                                    message = (whyY[r.Next(whyY.Length)] + " " + "I should " + replaced).Trim();
                                else
                                    message = (whyN[r.Next(whyN.Length)] + " " + "I shouldn't " + replaced).Trim();
                            }
                            else if (string.Compare(split[1], "i", true) == 0)
                            {
                                if (yes)
                                    message = (whyY[r.Next(whyY.Length)] + " " + "you should " + replaced).Trim();
                                else
                                    message = (whyN[r.Next(whyN.Length)] + " " + "you shouldn't " + replaced).Trim();
                            }
                            else
                            {
                                if (yes)
                                    message = (whyY[r.Next(whyY.Length)] + " " + subject + " should " + replaced).Trim();
                                else
                                    message = (whyN[r.Next(whyN.Length)] + " " + subject + " shouldn't " + replaced).Trim();
                            }
                        }
                        else
                        {
                            if (yes)
                                message = (whyY[r.Next(whyY.Length)]).Trim();
                            else
                                message = (whyN[r.Next(whyN.Length)]).Trim();
                        }
                    }
                    else if (string.Compare(split[0], "did", true) == 0 && subjectNLP != null)
                    {
                        bool yes = false;
                        if (r.Next(0, 2) == 1)
                            yes = true;

                        if (split.Length >= 2)
                        {
                            string subject = subjectNLP;
                            string rest = "";

                            for (int i = 1; i < split.Length; i++)
                            {
                                rest += split[i] + " ";
                            }
                            rest = rest.TrimEnd(' ').Replace(subjectNLP, string.Empty);

                            string replaced = QuestionsRegex(rest);

                            if (string.Compare(split[1], "you", true) == 0)
                            {
                                if (yes)
                                    message = (whyY[r.Next(whyY.Length)] + " " + "I did " + replaced).Trim();
                                else
                                    message = (whyN[r.Next(whyN.Length)] + " " + "I didn't " + replaced).Trim();
                            }
                            else if (string.Compare(split[1], "i", true) == 0)
                            {
                                if (yes)
                                    message = (whyY[r.Next(whyY.Length)] + " " + "you did " + replaced).Trim();
                                else
                                    message = (whyN[r.Next(whyN.Length)] + " " + "you didn't " + replaced).Trim();
                            }
                            else
                            {
                                if (yes)
                                    message = (whyY[r.Next(whyY.Length)] + " " + subject + " did " + replaced).Trim();
                                else
                                    message = (whyN[r.Next(whyN.Length)] + " " + subject + " didn't " + replaced).Trim();
                            }
                        }
                    }
                    else if (string.Compare(split[0], "does", true) == 0 && subjectNLP != null)
                    {
                        bool yes = false;
                        if (r.Next(0, 2) == 1)
                            yes = true;

                        if (split.Length >= 2)
                        {
                            string subject = subjectNLP;
                            string rest = "";

                            for (int i = 1; i < split.Length; i++)
                            {
                                rest += split[i] + " ";
                            }
                            rest = rest.TrimEnd(' ').Replace(subjectNLP, string.Empty);

                            string replaced = QuestionsRegex(rest);

                            subject = QuestionsRegex(subject);

                            if (yes)
                                message = (whyY[r.Next(whyY.Length)] + " " + subject + " does " + replaced).Trim();
                            else
                                message = (whyN[r.Next(whyN.Length)] + " " + subject + " does not " + replaced).Trim();
                        }
                    }
                    else if (string.Compare(split[0], "will", true) == 0 && subjectNLP != null)
                    {
                        bool yes = false;

                        if (r.Next(0, 2) == 1)
                            yes = true;

                        if (split.Length >= 2)
                        {
                            string subject = subjectNLP;
                            string rest = "";

                            for (int i = 1; i < split.Length; i++)
                            {
                                rest += split[i] + " ";
                            }
                            rest = rest.TrimEnd(' ').Replace(subjectNLP, string.Empty);

                            string replaced = QuestionsRegex(rest);

                            subject = QuestionsRegex(subject);

                            if (yes)
                                message = (whyY[r.Next(whyY.Length)] + " " + subject + " will " + replaced).Trim();
                            else
                                message = (whyN[r.Next(whyN.Length)] + " " + subject + " won't " + replaced).Trim();
                        }
                    }
                }
                else
                {
                    await BotThink(e, stringLibrary, botName);
                }
            }
            else
            {
                await BotThink(e, stringLibrary, botName);
            }

            if (message != null && !string.IsNullOrWhiteSpace(message))
            {
                message = message.Replace("  ", " ");
                await e.Message.RespondAsync(message);
            }
        }

        private static string QuestionsRegex(string rest)
        {
            var someVariable1 = "you";
            var someVariable2 = "me";
            var someVariable3 = "you are";
            var someVariable4 = "my";
            var someVariable5 = "your";
            var someVariable6 = "myself";
            var someVariable7 = "yourself";

            var replacements = new Dictionary<string, string>()
            {
                    {"me",someVariable1},
                    {"you",someVariable2},
                    {"i am", someVariable3},
                    {"i'm", someVariable3},
                    {"your", someVariable4},
                    {"my", someVariable5},
                    {"yourself", someVariable6},
                    {"myself", someVariable7},
                    {"i", someVariable1}
            };

            try
            {
                var regex = new Regex("(?i)(\\b" + string.Join("\\b|\\b", replacements.Keys) + "\\b)");
                var replaced = regex.Replace(rest, m => replacements[m.Value]);
                return replaced;
            }
            catch
            {
                return rest;
            }
        }

        public class Questions
        {
            private LexicalizedParser lp;

            public Questions()
            {
                // Loading english PCFG parser from file
                lp = LexicalizedParser.loadModel("models\\englishPCFG.ser.gz");
            }

            public string GetSubject(string question)
            {
                string subjectNPL = string.Empty;

                var tokenizerFactory = PTBTokenizer.factory(new CoreLabelTokenFactory(), "");
                var sent2Reader = new StringReader(question);
                var rawWords2 = tokenizerFactory.getTokenizer(sent2Reader).tokenize();
                sent2Reader.close();
                var tree = lp.apply(rawWords2);

                var tp = new TreePrint("xmlTree");

                PrintWriter p = new PrintWriter("parse.xml", "UTF-8");
                tp.printTree(tree, p);

                p.close();

                BufferedReader br = new BufferedReader(new FileReader("parse.xml"));
                string xmlS;

                try
                {
                    StringBuilder sb = new StringBuilder();
                    string line = br.readLine();

                    while (line != null)
                    {
                        sb.AppendLine(line);
                        line = br.readLine();
                    }
                    xmlS = sb.ToString();
                }
                finally
                {
                    br.close();
                    System.IO.File.Delete("parse.xml");
                }

                XmlDocument xml = new XmlDocument();
                xml.LoadXml(xmlS);

                XmlNodeList xnList = xml.SelectNodes("//*");

                XmlNode npTree = null;

                foreach (XmlNode xn in xnList)
                {
                    XmlAttributeCollection ac = xn.Attributes;

                    for (int i = 0; i < ac.Count; i++)
                    {
                        if (ac["value"].InnerText == "NP")
                        {
                            if (npTree == null)
                                npTree = xn;
                            break;
                        }
                    }
                    if (npTree != null) break;
                }

                if (npTree != null)
                {
                    XmlNodeList words = npTree.SelectNodes(".//*");

                    foreach (XmlNode xn in words)
                    {
                        if (xn.Name == "leaf")
                        {
                            XmlAttributeCollection ac = xn.Attributes;

                            for (int i = 0; i < ac.Count; i++)
                            {
                                if (xn.ParentNode.Attributes["value"].InnerText == "," || ac["value"].InnerText == "_" || ac["value"].InnerText == "'" || ac["value"].InnerText == "'s")
                                    subjectNPL = subjectNPL.Trim();

                                subjectNPL += ac["value"].InnerText + " ";
                            }
                        }
                    }
                }
                return subjectNPL.Trim();
            }
        }
    }
}