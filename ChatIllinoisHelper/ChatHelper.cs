using ChatIllinoisHelper.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;
using System.Web;

namespace ChatIllinoisHelper;

public class ChatHelper(ChatbotInterface chatbotInterface, ILogger<ChatHelper> logger) {
    ILogger<ChatHelper> logger = logger;
    ChatbotInterface chatbotInterface = chatbotInterface;

    [Function("IllinoisChat")]
    public async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Anonymous, "get", "post")] HttpRequestData req) {
        logger.LogInformation("IllinoisChat triggered.");
        var question = HttpUtility.ParseQueryString(req.Url.Query)["q"]?.ToString() ?? "";
        if (string.IsNullOrWhiteSpace(question)) {
            return new BadRequestObjectResult("Please provide a question using the 'q' query parameter.");
        }
        chatbotInterface.SystemPrompt = "You are a chatbot for the University of Illinois Urbana-Champaign, dedicated to providing accurate, timely, and friendly assistance exclusively using approved knowledge sources. Your target audience is the University of Illinois internal staff and faculty.";
        return new OkObjectResult(await chatbotInterface.SendMessage(question));
    }
}