using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace DemoQA.Service.Model.Request
{
    public class ReplaceBookRequestDto
    {
        [JsonProperty("userId")]
        public string UserId { get; set; }

        [JsonProperty("isbn")]
        public string Isbn { get; set; }
    }
}