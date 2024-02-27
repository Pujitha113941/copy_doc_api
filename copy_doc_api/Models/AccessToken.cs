using Newtonsoft.Json;

namespace copy_doc_api.Models
{
    public class AccessToken
    {
        [JsonProperty("access_token")]
        public string accessToken { get; set; }
        [JsonProperty("id")]
        public string Id { get; set; }
    }
}
