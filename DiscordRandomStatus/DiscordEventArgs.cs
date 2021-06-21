using System;

namespace DiscordRandomStatus
{

    public delegate void DiscordEventHandler<T>(T e);
    public abstract class DiscordEventArgs : EventArgs
    {
        /// <summary>
        /// Gets the client that triggered the event.
        /// </summary>
        public DiscordClient Client { get; internal set; }

        protected DiscordEventArgs(DiscordClient client)
        {
            this.Client = client;
        }
    }
    public sealed class ReadyEventArgs : DiscordEventArgs
    {
        internal ReadyEventArgs(DiscordClient client) : base(client) { }
    }
    public sealed class LoginErrorEventArgs : DiscordEventArgs
    {
        public string Error;
        internal LoginErrorEventArgs(DiscordClient client, string error) : base(client) { Error = error; }
    }
}
