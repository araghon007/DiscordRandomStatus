using Newtonsoft.Json;

namespace DiscordCustomStatus
{
    class DiscordEmoji
    {
        [JsonProperty(PropertyName = "name")]
        public string Name { get; internal set; }
    }
}
