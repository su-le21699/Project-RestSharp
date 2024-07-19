using Core.Reports;
using DemoQA.Service.Model.DataObject;
using DemoQA.Service.Services;
using DemoQA.Test.DataProvider;
using FluentAssertions;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DemoQA.Test.TestCases
{
    [TestFixture("user_01", "book_01"), Category("AddBooks")]
    public class AddBookToCollectionTest : BaseTest
    {
        private BookServices _bookServices;
        private UserServices _userServices;
        private string userDataKey;
        private string bookDataKey;
        UserDto userDto;
        BookDto bookDto;
        string token;

        public AddBookToCollectionTest(string userKey, string bookKey)
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
        public async Task AddBookToCollectionSuccesfully()
        {

            ReportLog.Info("2. Add book into collection");
            var response = await _bookServices.AddBookAsync(userDto.UserId, bookDto.Isbn, token);
            Console.WriteLine(response.Content);

            ReportLog.Info("3.Verify add book repsonse");
            response.StatusCode.Should().Be(System.Net.HttpStatusCode.Created);
            response.Data.Books.FirstOrDefault().Isbn.Should().Be(bookDto.Isbn);
            Console.WriteLine(response.StatusCode);
            Console.WriteLine(response.Content);

            ReportLog.Info("3. Delete added book from user's collection");
            await _bookServices.DeleteBookAsync(userDto.UserId, bookDto.Isbn, token);
        }
        [Test]
        public async Task AddBookToCollectionWithoutAuthotized()
        {
            ReportLog.Info("2. Add book to user's collection");
            var response = await _bookServices.AddBookAsync(userDto.UserId, bookDto.Isbn, null);

            ReportLog.Info("3. Verify add book response");
            response.StatusCode.Should().Be(System.Net.HttpStatusCode.Unauthorized);
            var result = (dynamic)JsonConvert.DeserializeObject(response.Content);
            ((int)result["code"]).Should().Be(1200);
            ((string)result["message"]).Should().Be("User not authorized!");
        }
        [Test]
        public async Task AddBookToCollectionWithWrongUserId()
        {
            ReportLog.Info("2. Add book to user's collection");
            var response = await _bookServices.AddBookAsync(userDto.UserId + "123", bookDto.Isbn, token);

            ReportLog.Info("3. Verify add book response");
            response.StatusCode.Should().Be(System.Net.HttpStatusCode.Unauthorized);
            var result = (dynamic)JsonConvert.DeserializeObject(response.Content);
            Console.WriteLine(result);
            ((int)result["code"]).Should().Be(1207);
            ((string)result["message"]).Should().Be("User Id not correct!");
        }
        [Test]
        public async Task AddBookToCollectionWithWrongBookId()
        {
            ReportLog.Info("2. Add book to user's collection");
            var response = await _bookServices.AddBookAsync(userDto.UserId, bookDto.Isbn + "444", token);

            ReportLog.Info("3. Verify add book response");
            response.StatusCode.Should().Be(System.Net.HttpStatusCode.BadRequest);
            var result = (dynamic)JsonConvert.DeserializeObject(response.Content);
            Console.WriteLine(result);
            ((int)result["code"]).Should().Be(1205);
            ((string)result["message"]).Should().Be("ISBN supplied is not available in Books Collection!");
        }
        [Test]
        public async Task AddBookToCollectionWithAddedBook()
        {
            ReportLog.Info("2. Add book to user's collection");
            var response = await _bookServices.AddBookAsync(userDto.UserId, bookDto.Isbn, token);

            ReportLog.Info("3. Verify add book response");
            response.StatusCode.Should().Be(System.Net.HttpStatusCode.BadRequest);
            var result = (dynamic)JsonConvert.DeserializeObject(response.Content);
            Console.WriteLine(result);
            ((int)result["code"]).Should().Be(1210);
            ((string)result["message"]).Should().Be("ISBN already present in the User's Collection!");
        }
    }
}

