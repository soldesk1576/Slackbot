namespace StatusNotifier.Service
{
    public class StatusChecker
    {
        private readonly string _apiUrl = "https://status.gemini.com/api/v2/status.json";
        private readonly HttpClient httpClient;

        public StatusChecker(HttpClient httpClient)
        {
            this.httpClient = httpClient;
        }

        public async Task<string> GetGeminiStatusAsync()
        {
            try
            {
                HttpResponseMessage response = await httpClient.GetAsync(_apiUrl);
                response.EnsureSuccessStatusCode();
                string responseBody = await response.Content.ReadAsStringAsync();
                return responseBody;
            }
            catch (HttpRequestException e)
            {
                Console.WriteLine($"Error: {e.Message}");
                throw;
            }
        }
    }
}
