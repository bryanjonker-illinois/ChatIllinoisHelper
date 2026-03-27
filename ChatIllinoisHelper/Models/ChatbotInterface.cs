using Markdig;
using System.Text;

namespace ChatIllinoisHelper.Models {
    public class ChatbotInterface(string? url, string? chatbotName, string? apiKey) {
        private readonly string _url = url ?? "";
        private readonly string _apiKey = apiKey ?? "";
        private readonly string _chatbotName = chatbotName ?? "";

        public string Model { get; set; } = "";
        public string SystemPrompt { get; set; } = "";

        public async Task<string> SendMessage(string message) {
            try {
                using var client = new HttpClient();
                var messages = new List<ChatbotTransportMessage>();
                if (!string.IsNullOrEmpty(SystemPrompt)) {
                    messages.Add(new ChatbotTransportMessage { content = SystemPrompt, role = "system" });
                }
                messages.Add(new ChatbotTransportMessage { content = message, role = "user" });
                var json = System.Text.Json.JsonSerializer.Serialize(new ChatbotTransport {
                    api_key = _apiKey,
                    course_name = _chatbotName,
                    messages = messages,
                    model = Model
                });
                var content = new StringContent(
                    json,
                    Encoding.UTF8,
                    "application/json"
                );
                var response = await client.PostAsync(_url, content);
                var markdownResponse = await response.Content.ReadAsStringAsync();
                return Markdown.ToHtml(markdownResponse, new MarkdownPipelineBuilder().UseSoftlineBreakAsHardlineBreak().UseAutoLinks().Build());
            } catch (Exception e) {
                return $"Error sending message: {e.Message}";
            }
        }
    }
}

