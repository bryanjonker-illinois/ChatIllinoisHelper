using ChatIllinoisHelper.Data.DataHelper;
using Markdig;
using System.Text;

namespace ChatIllinoisHelper.Data.Logic {
    public class ChatbotInterface(string? url, ChatItemHelper helper) {
        private readonly string _url = url ?? "";
        private readonly ChatItemHelper _helper = helper;

        public async Task<string> SendMessage(string message, string source) {
            try {
                var item = await _helper.GetByCode(source);
                if (item != null && item.Code == source) {
                    using var client = new HttpClient();
                    var messages = new List<ChatbotTransportMessage>();
                    if (!string.IsNullOrEmpty(item.SystemPrompt)) {
                        messages.Add(new ChatbotTransportMessage { content = item.SystemPromptMassaged, role = "system" });
                    }
                    messages.Add(new ChatbotTransportMessage { content = message, role = "user" });
                    var json = System.Text.Json.JsonSerializer.Serialize(new ChatbotTransport {
                        api_key = item.ApiSecret.Trim(),
                        course_name = item.Name.Trim(),
                        messages = messages,
                        model = item.Model.Trim()
                    });
                    Console.WriteLine($"Sending message to chatbot: {json}");
                    var content = new StringContent(
                        json,
                        Encoding.UTF8,
                        "application/json"
                    );
                    var response = await client.PostAsync(_url, content);
                    var markdownResponse = await response.Content.ReadAsStringAsync();
                    var fullResponse = Markdown.ToHtml(markdownResponse, new MarkdownPipelineBuilder().UseSoftlineBreakAsHardlineBreak().UseAutoLinks().Build());
                    if (item.RemoveThinkText) {
                        fullResponse = fullResponse.RemoveThinkTags();
                    }
                    return fullResponse;
                } else {
                    return "Invalid source code.";
                }
            } catch (Exception e) {
                return $"Error sending message: {e.Message}";
            }
        }
    }
}

