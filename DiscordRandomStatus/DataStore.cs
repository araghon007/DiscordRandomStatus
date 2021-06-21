using System;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using CompactJson;

namespace DiscordRandomStatus
{
    static class DataStore
    {
        static string saveLocation = $@"{Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData)}\\araghon007\\DiscordRandomCustomStatus\\";

        static DataStore()
        {
            if (!Directory.Exists(saveLocation)) Directory.CreateDirectory(saveLocation);
        }

        public static StatusArray[] Load()
        {
            if (!File.Exists($"{saveLocation}saved-statuses.json"))
            {
                return null;
            }
            return Serializer.Parse<SaveData>(File.ReadAllText($"{saveLocation}saved-statuses.json")).Arrays;
        }

        public static void Save(StatusArray[] arrays)
        {
            File.WriteAllText($"{saveLocation}saved-statuses.json", Serializer.ToString(new SaveData(arrays), true));
        }

        public static string LoadToken()
        {
            if (!File.Exists($"{saveLocation}secret"))
            {
                return null;
            }
            var bytes = File.ReadAllBytes($"{saveLocation}secret");
            var entropy = bytes.Take(20).ToArray();
            var ciphertext = bytes.Skip(20).ToArray();
            return Encoding.UTF8.GetString(ProtectedData.Unprotect(ciphertext, entropy, DataProtectionScope.CurrentUser));
        }

        public static void SaveToken(string token)
        {
            // Data to protect. Convert a string to a byte[] using Encoding.UTF8.GetBytes().
            byte[] plaintext = Encoding.UTF8.GetBytes(token);

            // Generate additional entropy (will be used as the Initialization vector)
            byte[] entropy = new byte[20];
            using (RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider())
            {
                rng.GetBytes(entropy);
            }

            byte[] ciphertext = ProtectedData.Protect(plaintext, entropy, DataProtectionScope.CurrentUser);
            byte[] result = new byte[entropy.Length + ciphertext.Length];
            entropy.CopyTo(result, 0);
            ciphertext.CopyTo(result, 20);
            File.WriteAllBytes($"{saveLocation}secret", result);
        }
    }

    class SaveData
    {
        [JsonProperty("arrays")]
        public StatusArray[] Arrays;

        public SaveData(){}

        public SaveData(StatusArray[] arrays)
        {
            Arrays = arrays;
        }
    }

    class StatusArray
    {
        [JsonProperty("chance_ratio")]
        public int ChanceRatio { get; set; }

        [JsonProperty("statuses")]
        public TransitCustomStatus[] Statuses;

        public StatusArray(){}

        public StatusArray(TransitCustomStatus[] statuses)
        {
            Statuses = statuses;
        }
    }
    
    class TransitCustomStatus
    {
        [JsonProperty("text")]
        public string Text { get; set; }

        [JsonProperty("emoji_id")]
        public string EmojiId { get; set; }

        [JsonProperty("emoji_name")]
        public string EmojiName { get; set; }

        [JsonProperty("animated")]
        public bool Animated;

        [JsonProperty("time")]
        public uint Time;

        public TransitCustomStatus() { }

        public TransitCustomStatus(CustomStatus status)
        {
            Text = status.Text;
            EmojiId = status.EmojiId;
            EmojiName = status.EmojiName;
            Animated = status.Animated;
            Time = status.Time;
        }

        public CustomStatus GetStatus()
        {
            return new CustomStatus(Text, EmojiId, EmojiName, Animated, Time);
        }
    }
}
