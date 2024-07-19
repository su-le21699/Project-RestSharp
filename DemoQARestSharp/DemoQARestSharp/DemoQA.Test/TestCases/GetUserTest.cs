using Core.Reports;
using DemoQA.Service.Model.DataObject;
using DemoQA.Service.Services;
using DemoQA.Test.DataProvider;
using FluentAssertions;
using Newtonsoft.Json;


namespace DemoQA.Test.TestCases
{
    [TestFixture("user_01"), Category("GetUser")]
    public class GetUserTest : BaseTest
    {
        private UserServices _userServices;
        private string userDataKey;
        UserDto userDto;
        string token;

        public GetUserTest(string userKey)
        {
            _userServices = new UserServices(ApiClient);
            userDataKey = userKey;
            userDto = UserProvider.LoadUserDataByKey(userDataKey);
        }
        [SetUp]
        public void SetUp()
        {
            ReportLog.Info("1. Generate token for account");
            _userServices.StoreUserToken(userDataKey, userDto);
            token = _userServices.GetUserToken(userDataKey);
        }
        [Test]
        public async Task GetUserSuccessfully()
        {
            ReportLog.Info("2. Get user by User ID");
            var response = await _userServices.GetUserAsync(userDto.UserId, token);

            ReportLog.Info("3. Verify get User response");
            response.StatusCode.Should().Be(System.Net.HttpStatusCode.OK);
            response.Data.UserId.Should().Be(userDto.UserId);
            response.Data.Username.Should().BeEquivalentTo(userDto.Username);
            response.Data.Books.Count().Should().BeGreaterThanOrEqualTo(0);
        }
        [Test]
        public async Task GetUserDetailWithoutAuthorized()
        {
            ReportLog.Info("1. Get user by User ID");
            var repsonse = await _userServices.GetUserAsync(userDto.UserId, null);

            ReportLog.Info("2. Verify get user response");
            repsonse.StatusCode.Should().Be(System.Net.HttpStatusCode.Unauthorized);
            var result = (dynamic)JsonConvert.DeserializeObject(repsonse.Content);
            ((int)result["code"]).Should().Be(1200);
            ((string)result["message"]).Should().Be("User not authorized!");
        }
        [Test]
        public async Task GetUserDetailWithWrongUserId()
        {
            ReportLog.Info("1. Get user by User ID");
            var repsonse = await _userServices.GetUserAsync(userDto.UserId + "123", token);

            ReportLog.Info("2. Verify get user response");
            repsonse.StatusCode.Should().Be(System.Net.HttpStatusCode.Unauthorized);
            var result = (dynamic)JsonConvert.DeserializeObject(repsonse.Content);
            ((int)result["code"]).Should().Be(1207);
            ((string)result["message"]).Should().Be("User not found!");

        }
    }
}