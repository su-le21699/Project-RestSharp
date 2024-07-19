using Newtonsoft.Json;

namespace DemoQA.Service.Model.DataObject
{
    public class UserDto
    {
        [JsonProperty("userId")]
        public string UserId {get; set;}
        [JsonProperty("username")]
        public string Username {get; set;}
        [JsonProperty("password")]
        public string Password {get; set;}
    }
}