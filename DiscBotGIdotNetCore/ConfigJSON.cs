using Newtonsoft.Json;

namespace DiscBotGIdotNetCore
{
    class ConfigJSON                                                       //class parsing JSON
    {
        [JsonProperty("token")]
        public string Token { get; private set; }
        [JsonProperty("prefix")]
        public string Prefix { get; private set; }
    }
}
