using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using DSharpPlus.Interactivity;
using DSharpPlus.Interactivity.Extensions;
using System;
using System.Threading.Tasks;

namespace DiscBotGIdotNetCore.Commands
{
    public class RoleCMD : BaseCommandModule
    {
        [Command("join")]
        public async Task Join(CommandContext ctx, string s1)    //Giving roles
        {

            var role = ctx.Guild.GetRole(729339121507500102);        //role ID
            DiscordEmbedBuilder.EmbedThumbnail thumbnailWorkAround = new DiscordEmbedBuilder.EmbedThumbnail();                //rework
            thumbnailWorkAround.Url = ctx.Client.CurrentUser.AvatarUrl;                                                               //  ThumbnailUrl
            var joinEmbed = new Discord​Embed​Builder                                 //Building Embeded Thumbnail
            {
                Title = "Would u like to join?",
                ImageUrl = ctx.User.AvatarUrl,
                Thumbnail = thumbnailWorkAround,
                Timestamp = DateTime.Now,                                                                       //No ThumbnailUrl
                Description = s1,
                Color = DiscordColor.Green
            };
            var joinMessage = await ctx.Channel.SendMessageAsync(embed: joinEmbed).ConfigureAwait(false);      //Sending join message



            var thumbsUpEmoji = DiscordEmoji.FromName(ctx.Client, ":+1:");                      //thumbUp
            var thumbsDownEmoji = DiscordEmoji.FromName(ctx.Client, ":-1:");                              //thumbDown

            await joinMessage.CreateReactionAsync(thumbsUpEmoji).ConfigureAwait(false);
            await joinMessage.CreateReactionAsync(thumbsDownEmoji).ConfigureAwait(false);

            var interactivity = ctx.Client.GetInteractivity();

            var reactionResult = await interactivity.WaitForReactionAsync(              //cheaking
                x => x.Message == joinMessage &&
                x.User.Id == ctx.User.Id &&
               (x.Emoji == thumbsUpEmoji ||
                x.Emoji == thumbsDownEmoji))
                .ConfigureAwait(false);

            if (reactionResult.Result.Emoji == thumbsUpEmoji)                                                             //more cheaking
            {
                await ctx.Member.GrantRoleAsync(role).ConfigureAwait(false);
            }
            else if (reactionResult.Result.Emoji == thumbsDownEmoji)
            {
                await ctx.Member.RevokeRoleAsync(role).ConfigureAwait(false);
            }
            else
            {
                await ctx.Channel.SendMessageAsync("wrong emoji").ConfigureAwait(false);
            }

            await joinMessage.DeleteAsync().ConfigureAwait(false);                     //delete join message
            
        }
    }
}
