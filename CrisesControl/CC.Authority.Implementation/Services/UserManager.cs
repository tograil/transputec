using System.Net.Http.Formatting;
using System.Text;
using CC.Authority.Core.UserManagement;
using CC.Authority.Core.UserManagement.Models;
using CC.Authority.Implementation.Config;
using CC.Authority.Implementation.Helpers;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace CC.Authority.Implementation.Services
{
    public class UserManager : IUserManager
    {
        private const string GetUserPath = "/Api/User/ScimGetUser?UserId=";
        private const string CheckUserPath = "/Api/User/ScimGetUser";
        private const string AddUserPath = "/Api/User/ScimAddUser";
        private const string UpdateUserPath = "/Api/User/ScimUserUpdate";

        private readonly HttpClient _httpClient;

        private readonly Uri _baseUri;
        private readonly string _appPath;

        public UserManager(HttpClient httpClient, IOptions<CrisesControlServerConfig> options)
        {
            _httpClient = httpClient;
            var crisesControlServerConfig = options.Value;

            _baseUri = new Uri(crisesControlServerConfig.ApiEndpoint);
            _appPath = crisesControlServerConfig.AppPath;

            _httpClient.DefaultRequestHeaders.Add("X-ApiKey", crisesControlServerConfig.ApiSecret);
        }

        public async Task<UserResponse> AddUser(UserInput userInput)
        {
            var addUserUri = new Uri(_baseUri, $"{_appPath}{AddUserPath}");

            var serialized = JsonConvert.SerializeObject(userInput);

            var response = await _httpClient.PostAsync(addUserUri, new StringContent(serialized,
                Encoding.UTF8,
                "application/json"));

            var result = await response.Content.ReadAsStringAsync();

            var resultData = JsonConvert.DeserializeObject<UserResponse>(result);

            return resultData;
        }

        public async Task<(bool, UserResponse?)> UserExists(string email, string externalScimId)
        {
            var checkUserUri = new Uri(_baseUri, $"{_appPath}{CheckUserPath}?UserId=0&PrimaryEmail={email}&ExternalScimId={externalScimId}");

            var response = await _httpClient.GetAsync(checkUserUri);

            var result = await response.Content.ReadAsStringAsync();

            if (result == "-1")
                return (false, null);

            var resultStruct = JsonConvert.DeserializeObject<UserResponse>(result);

            return (true, resultStruct);
        }

        public async Task<UserResponse?> GetUser(string id)
        {
            var getUserUri = new Uri(_baseUri, $"{_appPath}{GetUserPath}{id}");

            var response = await _httpClient.GetAsync(getUserUri);

            var result = await response.Content.ReadAsStringAsync();

            if(result == "-1") {
                return null;
            } else {
                return JsonConvert.DeserializeObject<UserResponse>(result);
            }

        }

        public async Task<UserResponse> UpdateUser(UserInput userInput)
        {
            var addUserUri = new Uri(_baseUri, $"{_appPath}{UpdateUserPath}");

            var serialized = JsonConvert.SerializeObject(userInput);

            var response = await _httpClient.PostAsync(addUserUri, new StringContent(serialized,
                Encoding.UTF8,
                "application/json"));

            var result = await response.Content.ReadAsStringAsync();

            var resultData = JsonConvert.DeserializeObject<UserResponse>(result);

            return resultData;
        }
    }
}