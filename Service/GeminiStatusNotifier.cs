using System.Text;

namespace StatusNotifier.Service
{
    public class GeminiStatusNotifier
    {
        private readonly string _slackWebhookUrl = "https://hooks.slack.com/services/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";

        public async Task SendSlackNotificationAsync(string message)
        {
            using (var httpClient = new HttpClient())
            {
                var content = new StringContent($"{{\"text\":\"{message}\"}}", Encoding.UTF8, "application/json");
                await httpClient.PostAsync(_slackWebhookUrl, content);
            }
        }
    }
}
