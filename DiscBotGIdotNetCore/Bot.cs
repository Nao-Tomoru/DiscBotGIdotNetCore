using DiscBotGIdotNetCore.Commands;
using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.EventArgs;
using DSharpPlus.Interactivity;
using DSharpPlus.Interactivity.Extensions;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace DiscBotGIdotNetCore
{

    public class Bot
    {

                                                                
        public DiscordClient Client { get; private set; }                                      //proclaiming Client 
        public CommandsNextExtension Commands { get; private set; }                      //Command Support
        public InteractivityConfiguration Interactivity { get; private set; }                  //Interactivity Support
        public async Task RunAsync()                                                        //Initialization of bot
        {

            bool feelings = true;

            string json = string.Empty;                                                       //initialization of JSON string

            using (var fs = File.OpenRead("config.json"))                                     //File Stream config
            using (var sr = new StreamReader(fs, new UTF8Encoding(false)))               //Stream Reader for config 
                json = await sr.ReadToEndAsync().ConfigureAwait(false);                    //Async read of config to JSON string

            var configJson = JsonConvert.DeserializeObject<ConfigJSON>(json);            //Deserealization of config to ConfigJSON class

            var config = new DiscordConfiguration                                          //Base config of bot
            {
                Token = configJson.Token,                                                  //Token Initialization
                TokenType = TokenType.Bot,                                                //Type of token
                AutoReconnect = true,                                                      //Will attempt to recconnect if true
                MinimumLogLevel = LogLevel.Debug,                                                  //Type of logging
                                               //Using of Internal logging base
            };

            Client = new DiscordClient(config);                                             //Initialization of Client with config
            Client.Ready += OnClientReady;                                                 //Delegate OnClientReady function to do

            Client.UseInteractivity(new InteractivityConfiguration                                              //Interactivity Settings
            {
                Timeout = TimeSpan.FromMinutes(2)
            });


            CommandsNextConfiguration cmdsConfig = new CommandsNextConfiguration               //Commands settings
            {
                StringPrefixes = new string[] { configJson.Prefix },                                    //Command Prefix
                EnableDms = false,                                         //Will Bot write into DMs
                EnableMentionPrefix = true,                                                          //@<BotName> will work
                CaseSensitive = false,                                      //Case Sensetivity
                DmHelp = false                                                         // will bot send help into DMs
            };
            Commands = Client.UseCommandsNext(cmdsConfig);                //appling commands settings

            Commands.RegisterCommands<PingPongCMD>();                                //Register all  commands in this class
            Commands.RegisterCommands<RoleCMD>();
            await Client.ConnectAsync();                                         //Connecting bot

            await Task.Delay(-1);                                                //To stay alive after all tasks done
        }
        private Task OnClientReady(DiscordClient client,ReadyEventArgs e)                                //Stuff which bot will do on when it's ready
        {
            Console.WriteLine("done ClR");
            return Task.CompletedTask;
        }
    }
}
