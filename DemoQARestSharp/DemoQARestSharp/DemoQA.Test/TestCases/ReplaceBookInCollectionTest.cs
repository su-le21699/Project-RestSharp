using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Extensions;
using Core.Reports;
using DemoQA.Service.Model.DataObject;
using DemoQA.Service.Services;
using DemoQA.Test.DataProvider;
using FluentAssertions;
using Newtonsoft.Json;

namespace DemoQA.Test.TestCases
{

    [TestFixture("user_01", "book_01", "book_02"), Category("ReplaceBook")]
    public class ReplaceBookTest : BaseTest
    {
        private BookServices _bookServices;
        private UserServices _userServices;
        private string userDatakey;
        private string bookDataKey;
        private string replaceBookDataKey;
        UserDto userDto;
        BookDto book, replaceBook;
        string token;

        public ReplaceBookTest(string userKey, string bookKey1, string bookKey2)
        {
            _bookServices = new BookServices(ApiClient);
            _userServices = new UserServices(ApiClient);
            userDatakey = userKey;
            bookDataKey = bookKey1;
            replaceBookDataKey = bookKey2;
            userDto = UserProvider.LoadUserDataByKey(userDatakey);
            book = BookProvider.LoadBookDataByKey(bookDataKey);
            replaceBook = BookProvider.LoadBookDataByKey(replaceBookDataKey);
        }
        [SetUp]
        public new void Setup()
        {
            ReportLog.Info("1. Generate token for account");
            _userServices.StoreUserToken(userDatakey, userDto);
            token = _userServices.GetUserToken(userDatakey);
        }
        [Test]
        public async Task ReplaceBookSuccessfully()
        {
            ReportLog.Info("2. Add book into collection");
            await _bookServices.AddBookAsync(userDto.UserId, book.Isbn, token);

            ReportLog.Info($"3. Replace {book.Title} by {replaceBook.Title}");
            var response = await _bookServices.ReplaceBookAsync(userDto.UserId, book.Isbn, replaceBook.Isbn, token);

            ReportLog.Info("4. Verify replace book response");
            response.StatusCode.Should().Be(System.Net.HttpStatusCode.OK);
            response.Data.UserId.Should().Be(userDto.UserId);
            response.Data.Username.Should().BeEquivalentTo(userDto.Username);
            response.Data.Books.Should().ContainSingle(book => book.Isbn == replaceBook.Isbn);

            ReportLog.Info("5. Verify json schema of the response body");
            response.VerifySchema(SchemaConstant.ReplaceBookSchema);

            ReportLog.Info("6. Delete replaced book in collection");
            await _bookServices.DeleteBookAsync(userDto.UserId, replaceBook.Isbn, token);
        }
        [Test]
        public async Task RepleaceBookInCollectionWithhoutAuthorized()
        {
            ReportLog.Info("2. Add book into collection");
            await _bookServices.AddBookAsync(userDto.UserId, book.Isbn, token);

            ReportLog.Info($"3. Replace {book.Title} by {replaceBook.Title}");
            var response = await _bookServices.ReplaceBookAsync(userDto.UserId, book.Isbn, replaceBook.Isbn, token);

            ReportLog.Info("4. Verify replace books response");
            response.StatusCode.Should().Be(System.Net.HttpStatusCode.Unauthorized);
            var result = (dynamic)JsonConvert.DeserializeObject(response.Content);
            ((int)result["code"]).Should().Be(1200);
            ((string)result["message"]).Should().Be("User not authorized!");

            ReportLog.Info("5. Delete replaced book in collection");
            await _bookServices.DeleteBookAsync(userDto.UserId, replaceBook.Isbn, null);
        }
        [Test]
        public async Task RepleaceBookInCollectionWithWrongUserID()
        {
            ReportLog.Info("2. Add book into collection");
            await _bookServices.AddBookAsync(userDto.UserId, book.Isbn, token);

            ReportLog.Info($"3. Replace {book.Title} by {replaceBook.Title}");
            var response = await _bookServices.ReplaceBookAsync(userDto.UserId + "2344", book.Isbn, replaceBook.Isbn, token);

            ReportLog.Info("4. Verify replace books response");
            response.StatusCode.Should().Be(System.Net.HttpStatusCode.Unauthorized);
            var result = (dynamic)JsonConvert.DeserializeObject(response.Content);
            ((int)result["code"]).Should().Be(1207);
            ((string)result["message"]).Should().Be("User Id not correct!");

            ReportLog.Info("5. Delete replaced book in collection");
            await _bookServices.DeleteBookAsync(userDto.UserId, replaceBook.Isbn, token);
        }
        [Test]
        public async Task RepleaceBookInCollectionWithWrongBookID()
        {
            ReportLog.Info("2. Add book into collection");
            await _bookServices.AddBookAsync(userDto.UserId, book.Isbn, token);

            ReportLog.Info($"3. Replace {book.Title} by {replaceBook.Title}");
            var response = await _bookServices.ReplaceBookAsync(userDto.UserId, book.Isbn + "4424", replaceBook.Isbn, token);

            ReportLog.Info("4. Verify replace books response");
            response.StatusCode.Should().Be(System.Net.HttpStatusCode.BadRequest);
            var result = (dynamic)JsonConvert.DeserializeObject(response.Content);
            ((int)result["code"]).Should().Be(1206);
            ((string)result["message"]).Should().Be("ISBN supplied is not available in User's Collection!");

            ReportLog.Info("5. Delete replaced book in collection");
            await _bookServices.DeleteBookAsync(userDto.UserId, replaceBook.Isbn, token);

        }
        [Test]
        public async Task RepleaceBookInCollectionWithBookId()
        {
            ReportLog.Info("2. Add book into collection");
            await _bookServices.AddBookAsync(userDto.UserId, book.Isbn, token);

            ReportLog.Info($"3. Replace {book.Title} by {replaceBook.Title}");
            var response = await _bookServices.ReplaceBookAsync(userDto.UserId, book.Isbn, book.Isbn, token);

            ReportLog.Info("4. Verify replace books response");
            response.StatusCode.Should().Be(System.Net.HttpStatusCode.BadRequest);
            var result = (dynamic)JsonConvert.DeserializeObject(response.Content);
            ((int)result["code"]).Should().Be(1206);
            ((string)result["message"]).Should().Be("ISBN supplied is not available in User's Collection!");

            ReportLog.Info("5. Delete replaced book in collection");
            await _bookServices.DeleteBookAsync(userDto.UserId, book.Isbn, token);
        }
        [Test]
        [TestCase("9781449325862")]
        public async Task RepleaceBookInCollectionWithAddedBookId(string isbn)
        {
            ReportLog.Info("2. Add book into collection");
            await _bookServices.AddBookAsync(userDto.UserId, book.Isbn, token);

            ReportLog.Info($"3. Replace {book.Title} by {replaceBook.Title}");
            var response = await _bookServices.ReplaceBookAsync(userDto.UserId, book.Isbn, isbn, token);

            ReportLog.Info("4. Verify replace books response");
            response.StatusCode.Should().Be(System.Net.HttpStatusCode.BadRequest);
            var result = (dynamic)JsonConvert.DeserializeObject(response.Content);
            ((int)result["code"]).Should().Be(1206);
            ((string)result["message"]).Should().Be("ISBN supplied is not available in User's Collection!");
            Console.WriteLine(result);

            ReportLog.Info("5. Delete replaced book in collection");
            await _bookServices.DeleteBookAsync(userDto.UserId, book.Isbn, token);
        }
    }
}

