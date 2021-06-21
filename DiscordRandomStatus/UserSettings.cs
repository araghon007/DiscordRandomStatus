using CompactJson;

namespace DiscordRandomStatus
{
    public class UserSettings
    {
        [JsonEmitNullValue]
        [JsonProperty("custom_status")]
        public CustomStatus CustomStatus { get; set; }

        public UserSettings(CustomStatus status)
        {
            CustomStatus = status;
        }
    }

    public class CustomStatus
    {
        [JsonProperty("text")]
        public string Text { get; set; }

        [JsonProperty("emoji_id")]
        public string EmojiId { get; set; }

        [JsonProperty("emoji_name")]
        public string EmojiName { get; set; }

        [JsonIgnoreMember]
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

        [JsonIgnoreMember]
        public bool Animated;

        [JsonIgnoreMember]
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
