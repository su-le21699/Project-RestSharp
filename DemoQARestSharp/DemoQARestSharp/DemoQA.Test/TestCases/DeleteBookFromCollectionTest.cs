using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Reports;
using DemoQA.Service.Model.DataObject;
using DemoQA.Service.Services;
using DemoQA.Test.DataProvider;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;

namespace DemoQA.Test.TestCases
{
    [TestFixture("user_01", "book_01"), Category("DeleteBooks")]
    public class DeleteBookFromCollectionTest : BaseTest
    {
        private BookServices _bookServices;
        private UserServices _userServices;
        private string userDataKey;
        private string bookDataKey;
        UserDto userDto;
        BookDto bookDto;
        string token;

        public DeleteBookFromCollectionTest(string userKey, string bookKey)
        {
            _bookServices = new BookServices(ApiClient);
            _userServices = new UserServices(ApiClient);
            userDataKey = userKey;
            bookDataKey = bookKey;
            userDto = UserProvider.LoadUserDataByKey(userDataKey);
            bookDto = BookProvider.LoadBookDataByKey(bookDataKey);

        }

        [SetUp]
        public new void Setup()
        {
            ReportLog.Info("1. Generate token for account");
            _userServices.StoreUserToken(userDataKey, userDto);
            token = _userServices.GetUserToken(userDataKey);
        }

        [Test]
        public async Task DeleteBookFromCollectionSuccesfully()
        {
            ReportLog.Info("2. Add book into collection");
            var response = await _bookServices.AddBookAsync(userDto.UserId, bookDto.Isbn, token);

            ReportLog.Info("3. Delete book from user collection");
            var responseDelete = await _bookServices.DeleteBookAsync(userDto.UserId, bookDto.Isbn, token);
            Console.WriteLine(responseDelete.Content);

            ReportLog.Info("4.Verify delete book repsonse");
            responseDelete.StatusCode.Should().Be(System.Net.HttpStatusCode.NoContent);
        }
        [Test]
        public async Task DeleteBookFromCollectionWithoutAuthotized()
        {
            ReportLog.Info("2.Delete book from user collection");
            var response = await _bookServices.DeleteBookAsync(userDto.UserId, bookDto.Isbn, null);

            ReportLog.Info("3.Verify delete book response");
            response.StatusCode.Should().Be(System.Net.HttpStatusCode.Unauthorized);
            var result = (dynamic)JsonConvert.DeserializeObject(response.Content);
            ((int)result["code"]).Should().Be(1200);
            ((string)result["message"]).Should().Be("User not authorized!");
        }
        [Test]
        public async Task DeleteBookFromCollectionWithWrongUserId()
        {
            ReportLog.Info("2. Add book into user collection");
            await _bookServices.AddBookAsync(userDto.UserId, bookDto.Isbn, token);

            ReportLog.Info("3. Delete book from user's collection");
            var response = await _bookServices.DeleteBookAsync(userDto.UserId + "4567", bookDto.Isbn, token);

            ReportLog.Info("4. Verify add book response");
            response.StatusCode.Should().Be(System.Net.HttpStatusCode.Unauthorized);
            var result = (dynamic)JsonConvert.DeserializeObject(response.Content);
            Console.WriteLine(result);
            ((int)result["code"]).Should().Be(1207);
            ((string)result["message"]).Should().Be("User Id not correct!");

            await _bookServices.DeleteBookAsync(userDto.UserId, bookDto.Isbn, token);
        }
        [Test]
        public async Task DeleteBookFromCollectionWithWrongBookId()
        {
            ReportLog.Info("2. Add book into user collection");
            await _bookServices.AddBookAsync(userDto.UserId, bookDto.Isbn, token);

            ReportLog.Info("3. Delete book from user's collection");
            var response = await _bookServices.DeleteBookAsync(userDto.UserId, bookDto.Isbn + "44456", token);

            ReportLog.Info("3. Verify add book response");
            response.StatusCode.Should().Be(System.Net.HttpStatusCode.BadRequest);
            var result = (dynamic)JsonConvert.DeserializeObject(response.Content);
            Console.WriteLine(result);
            ((int)result["code"]).Should().Be(1206);
            ((string)result["message"]).Should().Be("ISBN supplied is not available in User's Collection!");

            await _bookServices.DeleteBookAsync(userDto.UserId, bookDto.Isbn, token);
        }
    }
}
