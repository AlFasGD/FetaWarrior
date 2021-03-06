﻿using Discord;
using Discord.Net;
using Discord.Rest;
using System;
using System.Net;
using System.Threading.Tasks;

namespace FetaWarrior.DiscordFunctionality
{
    public class PersistentMessage
    {
        public RestUserMessage CurrentMessage { get; private set; }

        public PersistentMessage(RestUserMessage currentMessage)
        {
            CurrentMessage = currentMessage;
        }

        public async Task SetContentAsync(string content)
        {
            await ModifyAsync(m => m.Content = content);
        }

        public async Task ModifyAsync(Action<MessageProperties> modifier)
        {
            while (true)
            {
                try
                {
                    await CurrentMessage.ModifyAsync(modifier);
                    return;
                }
                catch (HttpException e) when (e.HttpCode == HttpStatusCode.NotFound)
                {
                    CurrentMessage = await CurrentMessage.Channel.SendMessageAsync(CurrentMessage.Content) as RestUserMessage;
                    await ModifyAsync(modifier);
                }
                catch { }

                await Task.Delay(200);
            }
        }

        public async Task DeleteAsync()
        {
            while (true)
            {
                try
                {
                    await CurrentMessage.DeleteAsync();
                    return;
                }
                catch (HttpException e) when (e.HttpCode == HttpStatusCode.NotFound)
                {
                    return;
                }
                catch { }

                await Task.Delay(200);
            }
        }
    }
}
