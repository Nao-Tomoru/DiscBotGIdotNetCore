using Google.Apis.Auth.OAuth2;
using Google.Apis.Sheets.v4;
using Google.Apis.Sheets.v4.Data;
using Google.Apis.Services;
using Google.Apis.Util.Store;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using DSharpPlus.VoiceNext;
using DSharpPlus.Lavalink;

namespace DiscBotGIdotNetCore
{
    class Program
    {
        public static string[] Scopes = { SheetsService.Scope.Spreadsheets };
        public static string ApplicationName = "Google Sheets API .NET Quickstart";
        public static string SpreadsheetId = "1D1AqNBr5wkOo_1xYM-o4HUctoNvpuyo-94O4ywhgwHA";
        public static GoogleCredential credential;
        static VoiceNextExtension voice;



        static void Main(string[] args)
        {
            using (var stream = new FileStream("client_secret.json", FileMode.Open, FileAccess.Read)) {
                credential = GoogleCredential.FromStream(stream).CreateScoped(Scopes);
            }
            Bot bot = new Bot();
            bot.RunAsync().GetAwaiter().GetResult();

        }
    }
}
