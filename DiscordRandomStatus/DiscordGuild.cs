using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace DiscordCustomStatus
{
    class DiscordGuild
    {
        [JsonProperty(PropertyName = "name")]
        public string Name { get; internal set; }

        [JsonProperty(PropertyName = "icon")]
        public string Icon { get; internal set; }

        [JsonProperty(PropertyName = "id")]
        public string Id { get; internal set; }

        public string GetIconURL(string extension, int size)
        {
            return $"https://cdn.discordapp.com/icons/{Id}/{Icon}.{extension}?size={size}";
        }
    }
}
