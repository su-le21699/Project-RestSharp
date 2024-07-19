﻿using Core.API;
using DemoQA.Service.Model.Request;
using DemoQA.Service.Model.Response;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DemoQA.Service.Services
{
    public class BookServices
    {
        private readonly APIClient _client;

        public BookServices(APIClient client)
        {
            _client = client;
        }
        public async Task<RestResponse<AddBookResponseDto>> AddBookAsync(string userId, string bookId, string token)
        {
            var requestBody = new AddBookRequestDto()
            {
                UserId = userId,
                CollectionOfIsbns = new List<BookRequestDto>
                {
                    new BookRequestDto { Isbn = bookId }
                }
            };
            return await _client.CreateRequest(String.Format(APIConstant.AddBookToCollectionEndPoint, userId, bookId))
                .AddContentTypeHeader("application/json")
                .AddHeader("accept", "application/json")
                .AddHeaderBearerToken(token)
                .AddBody(requestBody)
                .ExecutePostAsync<AddBookResponseDto>();
        }

        public async Task<RestResponse> DeleteBookAsync(string userId, string bookId, string token)
        {
            var requestBody = new DeleteBookResquestDto()
            {
                UserId = userId,
                Isbn = bookId
            };
            return await _client.CreateRequest(String.Format(APIConstant.DeleteBookFromCollectionEndPoint))
                .AddContentTypeHeader("application/json")
                .AddHeader("accept", "application/json")
                .AddHeaderBearerToken(token)
                .AddBody(requestBody)
                .ExecuteDeleteAsync();
        }
        public async Task<RestResponse<ReplaceBookResponseDto>> ReplaceBookAsync(string userId, string bookId, string replaceBookId, string token)
        {
            var requestBody = new ReplaceBookRequestDto()
            {
                UserId = userId,
                Isbn = replaceBookId
            };
            return await _client.CreateRequest(String.Format(APIConstant.ReplaceBookFromCollectionEndPoint, bookId))
                .AddContentTypeHeader("application/json")
                .AddHeader("accept", "application/json")
                .AddHeaderBearerToken(token)
                .AddBody(requestBody)
                .ExecutePutAsync<ReplaceBookResponseDto>();
        }
    }
}
