using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.API;
using Core.ShareData;
using DemoQA.Service.Model.DataObject;
using DemoQA.Service.Model.Request;
using DemoQA.Service.Model.Response;
using FluentAssertions;
using Newtonsoft.Json;
using RestSharp;

namespace DemoQA.Service.Services
{
    public class UserServices
    {
        private readonly APIClient _client;

        public UserServices(APIClient client)
        {
            _client = client;
        }
        public async Task<RestResponse<GetUserResponseDto>> GetUserAsync(string userId, string token)
        {
            Console.WriteLine($"Getting user with ID: {userId} using token: {token}");
            return await _client.CreateRequest(String.Format(APIConstant.GetUserEndPoint, userId))
                .AddContentTypeHeader("application/json")
                .AddHeaderBearerToken(token)
                .ExecuteGetAsync<GetUserResponseDto>();
        }
        public RestResponse<GenerateTokenResponseDto> GenerateToken(GenerateTokenRequestDto requestBody)
        {
            Console.WriteLine("Generating token...");
            var response = _client.CreateRequest(String.Format(APIConstant.GenerateToken))
                .AddContentTypeHeader("application/json")
                .AddBody(requestBody)
                .ExecutePost<GenerateTokenResponseDto>();

            Console.WriteLine($"Token generation response: {response.Content}");
            return response;
        }
        public void StoreUserToken(string accountKey, UserDto userDto)
        {
            GenerateTokenRequestDto request = new()
            {
                Username = userDto.Username,
                Password = userDto.Password
            };
            if (DataStorage.GetData(accountKey) is null)
            {
                var response = GenerateToken(request);
                response.StatusCode.Should().Be(System.Net.HttpStatusCode.OK);
                var result = (dynamic)JsonConvert.DeserializeObject(response.Content);
                Console.WriteLine($"Storing token for accountKey: {accountKey}");
                DataStorage.SetData(accountKey, result["token"]);
            }
        }

        public string GetUserToken(string accountKey)
        {
            var token = DataStorage.GetData(accountKey);
            if (token is null)
            {
                throw new Exception("Token is not stored with account " + accountKey);
            }

            Console.WriteLine($"Retrieved token for accountKey: {accountKey} - Token: {token}");
            return token.ToString();
        }
    }
}