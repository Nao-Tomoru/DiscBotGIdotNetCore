using DiscBotGIdotNetCore.Handlers.Dialogue;
using DiscBotGIdotNetCore.Handlers.Dialogue.Steps;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using DSharpPlus.Interactivity;
using DSharpPlus.VoiceNext;
using Google;
using Google.Apis.Services;
using Google.Apis.Sheets.v4;
using Google.Apis.Sheets.v4.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace DiscBotGIdotNetCore.Commands
{
    public class PingPongCMD : BaseCommandModule                  //Commands class
    {


        [Command("ping")]                                                        //how to trigger it in the DISCORD
        [Description("Возвращает Pong")]                                                       //Description
        [RequireRoles(RoleCheckMode.Any, "MODERATOR", "captain", "boo")]          //Which roles reqiere
        public async Task Ping(CommandContext ctx)                                                      //Command Function
        {
            await ctx
                .Channel.SendMessageAsync("Pong")                                                       //Bot send message
                .ConfigureAwait(false);
        }

        [Command("Nolan")]                                                        //how to trigger it in the DISCORD
        [Description("Считает Ноланов")]                                                       //Description
                 //Which roles reqiere
        public async Task Nolan(CommandContext ctx)                                                      //Command Function
        {

            int nolans = 0;
            int counter = 0;
            bool rightServer = false;
            using (StreamReader NolanReader = File.OpenText("Nolan.txt"))
            {
                string nextLine;

                while ((nextLine = await NolanReader.ReadLineAsync()) != null && !rightServer)
                {
                    var arr = nextLine.Split(':');
                    if (ctx.Guild.Name.Equals(arr[0])) { rightServer = true; nolans = Int32.Parse(arr[1]); }
                    else { counter++; }
                }
            }
            await ctx.Channel.SendMessageAsync("Nolans called: " + ++nolans).ConfigureAwait(false);
            List<string> vs = new List<string>();
            if (rightServer)
            {
                using (StreamReader NolanReader = File.OpenText("Nolan.txt"))
                {
                    string nextLine;
                    while ((nextLine = await NolanReader.ReadLineAsync()) != null)
                    {
                        vs.Add(nextLine);
                    }
                }
                vs[counter] = ctx.Guild.Name + ":" + nolans;
                using (StreamWriter NolanWriter = File.CreateText("Nolan.txt"))
                {
                    foreach (var s in vs)
                    {
                        await NolanWriter.WriteLineAsync(s);
                    }

                }
            }
            else
            {
                using (StreamWriter NolanWriter = File.AppendText("Nolan.txt"))
                {
                    await NolanWriter.WriteLineAsync(ctx.Guild.Name + ":" + nolans);
                }
            }

        }
        [Command("запроспрощенияу")]
        [Description("Запросить прощения у Маши")]
        public async Task ForgivePLS(CommandContext ctx, DiscordUser u)
        {

            var servEms = ctx.Guild.GetEmojisAsync().ConfigureAwait(false).GetAwaiter().GetResult();
            var ss = servEms.GetEnumerator();
            do
            {
                ss.MoveNext();
            }
            while (ss.Current.Name != "BibleThump");
            await ctx.Channel.SendMessageAsync(u.Mention + ", прости пожалуйста " + ss.Current);
        }
        [Command("add")]
        [Description("Складывает числа")]
        public async Task Add(CommandContext ctx,
            [Description("Первое число")] int a,                                        //description of args
            [Description("Второе число")] int b)
        {
            await ctx
                .Channel.SendMessageAsync(Convert.ToString(a + b))
                .ConfigureAwait(false);
        }
        [Command("playback")]
        public async Task Playback(CommandContext ctx, [RemainingText] string link)
        {

            bool flag = false;

            var voice = ctx.Client.UseVoiceNext();


            var channels = await ctx.Guild.GetChannelsAsync().ConfigureAwait(false);
            var echannels = channels.GetEnumerator();
            DiscordChannel channel = null;
            string name = "no name";
            do
            {
                echannels.MoveNext();
                if (echannels.Current == null) break;
                if (echannels.Current.Type == DSharpPlus.ChannelType.Voice)
                {
                    var k = echannels.Current.Users.GetEnumerator();
                    do
                    {
                        k.MoveNext();
                        if (k.Current == null) break;
                        if (k.Current.Id == ctx.User.Id)
                        {
                            flag = true;
                            name = echannels.Current.Name;
                            channel = echannels.Current;
                        }
                    } while (k.Current != null && flag != true);
                };
            } while (echannels != null && flag != true);
            await ctx.RespondAsync(name);
            if (channel != null)
                await voice.ConnectAsync(channel).ConfigureAwait(false);
            voice.GetConnection(ctx.Guild);

        }
        [Command("AddRoles")]
        public async Task AddRole(CommandContext ctx)
        {
            ulong roleId = 750066580628701185;
            var members = ctx.Guild.Members;
            var role = ctx.Guild.GetRole(roleId);
            // string check = role.Name;
            foreach (var member in members)
            {
                if (member.Value.Roles.Contains(role) == false)
                    await member.Value.GrantRoleAsync(role).ConfigureAwait(false);

            }


        }
        [Command("response")]
        public async Task Response(CommandContext ctx)
        {
            try
            {
                var sheet = "test sheet";
                var interactivity = ctx.Client.GetInteractivity();                                      //interactivity
                var message = await interactivity
                    .WaitForMessageAsync(x => x.Channel == ctx.Channel)                    //would wait for a message and, if it meets the reqirements
                    .ConfigureAwait(false);
                await ctx.Channel.SendMessageAsync(message.Result.Content)                   //    send a response //here is text
                    .ConfigureAwait(false);
                byte[] stingByte = Encoding.Unicode.GetBytes(message.Result.Content);

                var service = new SheetsService(new BaseClientService.Initializer()
                {
                    HttpClientInitializer = Program.credential,
                    ApplicationName = Program.ApplicationName,
                });
                var range = $"{sheet}!A:A";
                var valueRange = new ValueRange();
                var objectList = new List<object>() { $"{message.Result.Content}" };
                valueRange.Values = new List<IList<object>> { objectList };

                var appendRequset = service.Spreadsheets.Values.Append(valueRange, Program.SpreadsheetId, range);
                appendRequset.ValueInputOption = SpreadsheetsResource.ValuesResource.AppendRequest.ValueInputOptionEnum.USERENTERED;
                var appendResponde = appendRequset.Execute();
                /*   
                              using (FileStream stream = new FileStream("D:\\suggestions.txt", FileMode.Append, FileAccess.Write, FileShare.None, 4096, true))
                              using (StreamWriter sw = new StreamWriter(stream))
                               {
                                    await sw.WriteLineAsync(message.Result.Content);
                               }     
                        */
            }
            catch (Google.GoogleApiException e)
            {
                Console.WriteLine(e.Message);
            }

        }

        [Command("responserct")]
        [RequireRoles(RoleCheckMode.Any, "MODERATOR", "captain", "boo")]
        public async Task ResponseRCT(CommandContext ctx)
        {
            var interactivity = ctx.Client.GetInteractivity();
            var message = await interactivity
                .WaitForReactionAsync(x => x.Channel == ctx.Channel)              //same but for reaction
                .ConfigureAwait(false);
            await ctx.Channel.SendMessageAsync(message.Result.Emoji)                   //same but emoji
                .ConfigureAwait(false);
        }
        //[Command("changesheet")]
        //[RequireRoles(RoleCheckMode.Any, "MODERATOR", "captain", "boo")]
        //public async Task ChangeSheet(CommandContext ctx)
        //{
        //    using (var streamReader = File.OpenText("spreadsheet.txt"))
        //    {
        //        var lines;
        //        await lines = streamReader.ReadToEndAsync().ConfigureAwait(false).Split("\r\n".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
        //        foreach (var line in lines) { }
        //    }
        //    // Process line
        //}

        [Command("dialogue")]
        public async Task Dialogue(CommandContext ctx)
        {
            var inputStep = new StringStep("Enter something interesting", null);
            string input = string.Empty;


            inputStep.OnValidResult += (result) => input = result;

            var userChannel = await ctx.Member.CreateDmChannelAsync().ConfigureAwait(false);
            var inputDialogueHandler = new DialogueHandler(
                ctx.Client,
                userChannel,
                ctx.User,
                inputStep
                );
            bool succeeded = await inputDialogueHandler.ProcessDialogue().ConfigureAwait(false);

            if (!succeeded)
            {
                return;
            }

            await ctx.Channel.SendMessageAsync(input).ConfigureAwait(false);
        }

        [Command("getallmess")]
        public async Task GetAllMess(CommandContext ctx)
        {
            try
            {
                var channels = await ctx.Guild.GetChannelsAsync().ConfigureAwait(false);
                var echannels = channels.GetEnumerator();
                DiscordChannel channel = null;
                string name = "no name";
                bool flag = false;
                do
                {
                    echannels.MoveNext();
                    if (echannels.Current == null) break;
                    if (echannels.Current.Type == DSharpPlus.ChannelType.Text)
                    {
                        if (echannels.Current.Name == "general")
                        {
                            name = "found " + echannels.Current.Name;
                            channel = echannels.Current;
                        }
                    };
                } while (echannels != null || flag != true);

                var service = new SheetsService(new BaseClientService.Initializer()
                {
                    HttpClientInitializer = Program.credential,
                    ApplicationName = Program.ApplicationName,
                });
                var list = await channel.GetMessagesAsync().ConfigureAwait(false);
                var sheet = "test sheet";
                var range = $"{sheet}!A:A";
                var valueRange = new ValueRange();
                int i = 0;
                foreach (var point in list)
                {
                    var objectList = new List<object>() { $"{point.Content}" };
                    valueRange.Values = new List<IList<object>> { objectList };
                    i++;
                    if (i == 60)
                    {
                        i = 0;
                        await Task.Delay(60000);
                    }
                    var appendRequset = service.Spreadsheets.Values.Append(valueRange, Program.SpreadsheetId, range);
                    appendRequset.ValueInputOption = SpreadsheetsResource.ValuesResource.AppendRequest.ValueInputOptionEnum.USERENTERED;
                    var appendResponde = appendRequset.Execute();
                }

                await ctx.RespondAsync(name);
            }
            catch (GoogleApiException e)
            {
                Console.WriteLine(e.Message);
            }


        }
    }
}
