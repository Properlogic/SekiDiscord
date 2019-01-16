﻿using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using SekiDiscord.Commands;
using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SekiDiscord
{
    public class SekiCommands
    {
        private static StringLibrary stringLibrary;

        internal StringLibrary StringLibrary { get => stringLibrary; set => stringLibrary = value; }

        public static void SetStringLibrary(StringLibrary lib)
        {
            stringLibrary = lib;
        }

        [Command("quote")]
        [Description("Show or add quotes")]     // this will be displayed to tell users what this command does when they invoke help
        [Aliases("q")]                          // alternative names for the command
        public async Task Quote(CommandContext ctx)
        {
            Console.WriteLine(DateTime.Now.ToString("[HH:mm:ss] ") + "Quote Command");
            string arg;

            try
            {
                arg = ctx.Message.Content.Split(new char[] { ' ' }, 2)[1];
            }
            catch
            {
                arg = string.Empty;
            }

            if (string.Compare(arg.ToLower().Split(new char[] { ' ' }, 2)[0], "add") == 0)  //add
            {
                Quotes.AddQuote(ctx, stringLibrary);
            }
            else //lookup or random
            {
                string result = Quotes.PrintQuote(ctx, stringLibrary);
                await ctx.RespondAsync(result);
            }
        }

        [Command("qcount")]
        [Description("Show how many quotes are loaded")]     // this will be displayed to tell users what this command does when they invoke help
        public async Task QuoteCount(CommandContext ctx)
        {
            Console.WriteLine(DateTime.Now.ToString("[HH:mm:ss] ") + "Quote Count Command");

            string result = Quotes.QuoteCount(ctx, stringLibrary);
            await ctx.RespondAsync(result);
        }

        [Command("kill")]
        public async Task Kill(CommandContext ctx)
        {
            Console.WriteLine(DateTime.Now.ToString("[HH:mm:ss] ") + "Kill Command");
            KillUser.KillResult result = KillUser.Kill(ctx, StringLibrary);

            switch (result.IsAction)
            {
                case true:
                    await ctx.RespondAsync("*" + result.Result + "*");
                    break;

                case false:
                    await ctx.RespondAsync(result.Result);
                    break;
            }
        }

        [Command("rkill")]
        public async Task RKill(CommandContext ctx)
        {
            Console.WriteLine(DateTime.Now.ToString("[HH:mm:ss] ") + "RKill Command");
            KillUser.KillResult result = KillUser.KillRandom(ctx, StringLibrary);

            switch (result.IsAction)
            {
                case true:
                    await ctx.RespondAsync("*" + result.Result + "*");
                    break;

                case false:
                    await ctx.RespondAsync(result.Result);
                    break;
            }
        }

        [Command("addcmd")]
        [Description("Add a command to the custom commands list")]
        public async Task AddCustomCommand(CommandContext ctx)
        {
            Console.WriteLine(DateTime.Now.ToString("[HH:mm:ss] ") + "addcmd Command");
            string nick = ctx.User.Username;
            string[] splits;
            string message;
            splits = ctx.Message.Content.Split(new char[] { ' ' }, 3);

            if (CustomCommand.CommandExists(splits[0], StringLibrary.CustomCommands) == true)
            {
                message = "Command " + splits[0] + " already exists.";
                await ctx.RespondAsync(message);
            }

            StringLibrary.CustomCommands.Add(new CustomCommand(nick, splits[1], splits[2]));
            CustomCommand.SaveCustomCommands(StringLibrary.CustomCommands);
        }

        [Command("removecmd")]
        [Description("Remove a command to the custom commands list")]
        public async Task RemoveCustomCommand(CommandContext ctx)
        {
            Console.WriteLine(DateTime.Now.ToString("[HH:mm:ss] ") + "removecmd Command");

            string[] splits;

            splits = ctx.Message.Content.Split(new char[] { ' ' }, 3);

            if (Useful.MemberIsBotOperator(ctx.Member) || ctx.Member.IsOwner)
            {
                CustomCommand.RemoveCommandByName(splits[1], StringLibrary.CustomCommands);
                CustomCommand.SaveCustomCommands(StringLibrary.CustomCommands);
                string message = "Command " + splits[1] + " removed.";
                await ctx.RespondAsync(message);
            }
        }

        [Command("addping")]
        [Description("Add a word or phrase for which the user will always be pinged at")]
        public async Task AddPing(CommandContext ctx)
        {
            Console.WriteLine(DateTime.Now.ToString("[HH:mm:ss] ") + "addping Command");
            await Task.Run(() => PingUser.AddPing(ctx, StringLibrary));
        }

        [Command("removeping")]
        [Description("Remove a word or phrase for the users pings")]
        public async Task RemovePing(CommandContext ctx)
        {
            Console.WriteLine(DateTime.Now.ToString("[HH:mm:ss] ") + "removeping Command");
            await Task.Run(() => PingUser.RemovePing(ctx, StringLibrary));
        }

        [Command("copyping")]
        [Description("Copy another users pings")]
        public async Task CopyPing(CommandContext ctx)
        {
            Console.WriteLine(DateTime.Now.ToString("[HH:mm:ss] ") + "copyping Command");
            //disable for now
            //await Task.Run(() => PingUser.CopyPing(ctx, StringLibrary));
        }

        [Command("square")]
        [Description("square a word")]
        public async Task SquareText(CommandContext ctx)
        {
            int MAX_TEXT = 10;

            string text;
            string user = ctx.User.Username;

            try
            {
                text = ctx.Message.Content.Split(new char[] { ' ' }, 2)[1];
            }
            catch
            {
                text = string.Empty;
            }

            if (text.Length > MAX_TEXT)
            {
                string message = "_farts on " + user + "_";
                await ctx.RespondAsync(message);
                return;
            }
            else
            {
                StringBuilder builder = new StringBuilder();
                for (int i = 0; i <= text.Length - 1; i++)
                {
                    if (i == 0)
                    {
                        builder.Append("```");
                        foreach (char value in text.ToCharArray())
                        {
                            builder.Append(value + " ");
                        }
                        builder.Append("\n");
                    }
                    else if (i == text.Length - 1)
                    {
                        foreach (char value in text.ToCharArray().Reverse())
                        {
                            builder.Append(value + " ");
                        }
                        builder.Append("```");
                    }
                    else
                    {
                        builder.Append(text[i] + new string(' ', text.Length + (text.Length - 3)) + text[text.Length - 1 - i] + "\n");
                    }
                }
                string msg = builder.ToString();
                string message = msg.ToUpper();
                await ctx.RespondAsync(message);
            }
        }
    }
}