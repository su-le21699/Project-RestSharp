using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DemoQA.Service.Model.DataObject;
using Newtonsoft.Json;

namespace DemoQA.Service.Model.Response
{
    public class ReplaceBookResponseDto
    {
        [JsonProperty("userId")]
        public string UserId { get; set; }

        [JsonProperty("username")]
        public string Username { get; set; }

        [JsonProperty("books")]
        public List<BookDto> Books { get; set; }
    }
}