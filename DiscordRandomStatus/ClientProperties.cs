using Newtonsoft.Json;

namespace DiscordCustomStatus
{
    class ClientProperties
    {
        [JsonProperty(PropertyName = "os")]
        public string OS { get; internal set; }

        [JsonProperty(PropertyName = "browser")]
        public string Browser { get; internal set; }

        [JsonProperty(PropertyName = "release_channel")]
        public string ReleaseChannel { get; internal set; }

        [JsonProperty(PropertyName = "client_version")]
        public string ClientVersion { get; internal set; }

        [JsonProperty(PropertyName = "os_version")]
        public string OSversion { get; internal set; }

        [JsonProperty(PropertyName = "os_arch")]
        public string OSarch { get; internal set; }

        [JsonProperty(PropertyName = "client_build_number")]
        public int BuildNumber { get; internal set; }

        [JsonProperty(PropertyName = "client_event_source")]
        public string EventSource { get; internal set; }

        public ClientProperties()
        {
            OS = "Windows";
            Browser = "Discord Client";
            ReleaseChannel = "staging";
            ClientVersion = "0.0.690";
            OSversion = "10.0.18362";
            OSarch = "x64";
            BuildNumber = 44690;
        }
    }
}
