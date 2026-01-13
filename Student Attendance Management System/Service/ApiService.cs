

using System.Net.Http.Headers;

namespace Student_Attendance_Management_System.Service
{
    public static class ApiService
    {
        static HttpClient _httpClient = new()
        {
            BaseAddress = new Uri("http://localhost:3000")
        };

        public static async Task<HttpResponseMessage> SendAsync(
            HttpMethod method,
            string endpoint,
            object body = null,
            bool authorized = false
            )
        {
            var request = new HttpRequestMessage(method, endpoint);
            if (authorized)
            {
                var token = await Helpers.AppStorage.GetTokenAsync();
                //Debug.Write("Token Found" + token);
                if (!string.IsNullOrEmpty(token))
                {
                    request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
                }
            }
            if (body != null)
            {
                if (body is MultipartFormDataContent multipartContent)
                {
                    request.Content = multipartContent;
                }
                else
                {
                    // Standard JSON serialization for other requests
                    var json = System.Text.Json.JsonSerializer.Serialize(body);
                    request.Content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");
                }
            }
            return await _httpClient.SendAsync(request);
        }
    }
}
