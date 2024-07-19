using Newtonsoft.Json;

namespace DemoQA.Service.Model.Request
{
    public class GenerateTokenRequestDto
    {
        [JsonProperty("userName")]
        public string Username { get; set; }

        [JsonProperty("password")]
        public string Password { get; set; }
    }
}