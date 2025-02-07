using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace AuthService.Services
{
    public class PhoneOTPService
    {
        private readonly string _smsApiUrl = "https://sms.capcom.me/api/3rdparty/v1/message";
        private readonly string _apiUser = "RK0O0O";
        private readonly string _apiPassword = "droa_6fmy6o7fi";
        private readonly HttpClient _httpClient;

        public PhoneOTPService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        
        public async Task<bool> SendPhoneOTP(string phoneNumber, string otp)
        {
            var payload = new
            {
                message = $"Your OTP code is: {otp}",
                phoneNumbers = new[] { phoneNumber }
            };

            var jsonContent = JsonConvert.SerializeObject(payload);

            var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

            var request = new HttpRequestMessage(HttpMethod.Post, _smsApiUrl)
            {
                Content = content
            };

            var authValue = Convert.ToBase64String(Encoding.ASCII.GetBytes($"{_apiUser}:{_apiPassword}"));
            request.Headers.Add("Authorization", $"Basic {authValue}");

            var response = await _httpClient.SendAsync(request);

            if (response.IsSuccessStatusCode)
            {
                
                var responseBody = await response.Content.ReadAsStringAsync();
                var responseJson = JsonConvert.DeserializeObject<dynamic>(responseBody);

                 

                return true; 
            }
            return false; 
        }
    }
}
