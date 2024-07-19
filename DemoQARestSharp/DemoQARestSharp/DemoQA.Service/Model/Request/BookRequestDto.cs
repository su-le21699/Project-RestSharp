using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace DemoQA.Service.Model.Request
{
    public class BookRequestDto
    {
        [JsonProperty("isbn")]
        public string Isbn { get; set; }
    }
}