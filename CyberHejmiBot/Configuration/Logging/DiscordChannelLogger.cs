using Discord;
using Discord.Rest;
using Discord.WebSocket;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CyberHejmiBot.Configuration.Logging
{
    public class DiscordChannelLogger : ILogger
    {
        private readonly string _name;
        private readonly DiscordSocketClient _client;
        private const ulong CHANNEL_ID = 1012391792014008353;

        public DiscordChannelLogger(string name, DiscordSocketClient client)
        {
            _name = name;
            _client = client;
        }

        public IDisposable? BeginScope<TState>(TState state) where TState : notnull => default!;

        public bool IsEnabled(LogLevel logLevel)
        {
            // Only log warnings and above, or explicitly allowed levels.
            // DebugLogger logged everything if called, but typically we care about warnings/errors for this channel.
            return logLevel >= LogLevel.Warning;
        }

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception, Func<TState, Exception?, string> formatter)
        {
            if (!IsEnabled(logLevel))
                return;

            if (_client.LoginState != LoginState.LoggedIn)
                return;

            // Fire and forget logging to avoid blocking
            _ = LogAsync(logLevel, eventId, state, exception, formatter);
        }

        private async Task LogAsync<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception, Func<TState, Exception?, string> formatter)
        {
            try
            {
                var channel = await _client.GetChannelAsync(CHANNEL_ID) as IMessageChannel ?? await _client.Rest.GetChannelAsync(CHANNEL_ID) as IMessageChannel;
                
                if (channel == null)
                    return;

                var message = formatter(state, exception);
                var embed = new EmbedBuilder()
                    .WithTitle(logLevel.ToString())
                    .WithDescription(message)
                    .WithColor(LogColors.ContainsKey(logLevel) ? LogColors[logLevel] : Color.Default)
                    .WithTimestamp(DateTimeOffset.UtcNow)
                    .WithFooter(_name);

                if (exception != null)
                {
                    embed.AddField("Exception", exception.Message.Substring(0, Math.Min(exception.Message.Length, 1024)));
                    if (exception.StackTrace != null)
                    {
                        // Stack trace can be long, maybe just log a snippet or skip in embed
                        // DebugLogger logged inner exception.
                        if (exception.InnerException != null)
                        {
                             embed.AddField("Inner Exception", exception.InnerException.Message.Substring(0, Math.Min(exception.InnerException.Message.Length, 1024)));
                        }
                    }
                }

                await channel.SendMessageAsync(embed: embed.Build());
            }
            catch
            {
                // Prevent infinite loops if logging fails
            }
        }

        private static readonly Dictionary<LogLevel, Color> LogColors = new Dictionary<LogLevel, Color>
        {
            { LogLevel.Trace, Color.Blue },
            { LogLevel.Debug, Color.DarkPurple },
            { LogLevel.Information, Color.Blue },
            { LogLevel.Warning, Color.Orange },
            { LogLevel.Error, Color.Red },
            { LogLevel.Critical, Color.DarkRed },
        };
    }

    public class DiscordChannelLoggerProvider : ILoggerProvider
    {
        private readonly DiscordSocketClient _client;

        public DiscordChannelLoggerProvider(DiscordSocketClient client)
        {
            _client = client;
        }

        public ILogger CreateLogger(string categoryName)
        {
            return new DiscordChannelLogger(categoryName, _client);
        }

        public void Dispose()
        {
        }
    }
}
