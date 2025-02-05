using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace AuthService.Services
{
    public class PhoneOTPService
    {
        private readonly string _smsApiUrl = "https://sms.capcom.me/api/3rdparty/v1/message";  // SMS API endpoint
        private readonly string _apiUser = "RK0O0O";  // Your API username
        private readonly string _apiPassword = "droa_6fmy6o7fi";  // Your API password
        private readonly HttpClient _httpClient;

        public PhoneOTPService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        // Sends OTP via SMS API
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

            // Add Basic Auth header for authorization
            var authValue = Convert.ToBase64String(Encoding.ASCII.GetBytes($"{_apiUser}:{_apiPassword}"));
            request.Headers.Add("Authorization", $"Basic {authValue}");

            // Send the request
            var response = await _httpClient.SendAsync(request);

            if (response.IsSuccessStatusCode)
            {
                return true; // OTP sent successfully
            }

            return false; // Failed to send OTP
        }
    }
}
