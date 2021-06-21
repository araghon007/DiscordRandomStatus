using CompactJson;

namespace DiscordRandomStatus
{
    class ClientProperties
    {
        [JsonProperty("os")]
        public string OS { get; internal set; }

        [JsonProperty("browser")]
        public string Browser { get; internal set; }

        [JsonProperty("release_channel")]
        public string ReleaseChannel { get; internal set; }

        [JsonProperty("client_version")]
        public string ClientVersion { get; internal set; }

        [JsonProperty("os_version")]
        public string OSversion { get; internal set; }

        [JsonProperty("os_arch")]
        public string OSarch { get; internal set; }

        [JsonProperty("client_build_number")]
        public int BuildNumber { get; internal set; }

        [JsonProperty("client_event_source")]
        public string EventSource { get; internal set; }

        // TODO: Add proper OS detection
        public ClientProperties()
        {
            OS = "Windows";
            Browser = "Discord Client";
            ReleaseChannel = "canary";
            ClientVersion = "1.0.36";
            OSversion = "10.0.19042";
            OSarch = "x64";
            BuildNumber = 88173;
        }
    }
}
