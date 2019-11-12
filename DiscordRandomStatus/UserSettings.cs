using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiscordCustomStatus
{
    public class UserSettings
    {
        [JsonProperty(PropertyName = "custom_status")]
        public CustomStatus CustomStatus { get; set; }

        public UserSettings(CustomStatus status)
        {
            CustomStatus = status;
        }
    }

    public class CustomStatus
    {
        [JsonProperty(PropertyName = "text", NullValueHandling = NullValueHandling.Ignore)]
        public string Text { get; set; }

        [JsonProperty(PropertyName = "emoji_id", NullValueHandling = NullValueHandling.Ignore)]
        public string EmojiId { get; set; }

        [JsonProperty(PropertyName = "emoji_name", NullValueHandling = NullValueHandling.Ignore)]
        public string EmojiName { get; set; }

        [JsonIgnore]
        public string EmojiFull {
            get{
                if (!string.IsNullOrEmpty(EmojiId))
                {
                    if (Animated)
                    {
                        return $"<a:{EmojiName}:{EmojiId}>";
                    }
                    return $"<:{EmojiName}:{EmojiId}>";
                }
                return EmojiName;
            }
        }

        [JsonIgnore]
        public bool Animated;

        [JsonIgnore]
        public uint Time;

        public CustomStatus(string text = null, string emojiId = null, string emojiName = null, bool animated = false, uint time = 10)
        {
            Text = text;
            EmojiId = emojiId;
            EmojiName = emojiName;
            Animated = animated;
            Time = time;
        }
    }
}
