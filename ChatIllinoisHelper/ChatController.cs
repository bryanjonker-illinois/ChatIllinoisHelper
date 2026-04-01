using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using ChatIllinoisHelper.Models;

namespace ChatIllinoisHelper;

public class ChatController(ILogger<ChatController> logger, ChatbotInterface chatbot) {
    private readonly ILogger<ChatController> _logger = logger;
    private readonly ChatbotInterface _chatbot = chatbot;

    [Function("Chat")]
    public async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Anonymous, "get", "post")] HttpRequest req) {
        _logger.LogInformation("Chat function processed a request.");

        string? query = req.Query["q"];
        string? source = req.Query["source"];

        if (string.IsNullOrEmpty(query)) {
            return new BadRequestObjectResult("Query parameter 'q' is required.");
        }

        if (string.IsNullOrEmpty(source)) {
            return new BadRequestObjectResult("Query parameter 'source' is required.");
        }

        try {
            _logger.LogInformation("Processing query: {Query} from source: {Source}", query, source);
            
            var htmlResponse = await _chatbot.SendMessage(query);
            
            return new ContentResult {
                Content = htmlResponse,
                ContentType = "text/html",
                StatusCode = 200
            };
        } catch (Exception ex) {
            _logger.LogError(ex, "Error processing chat request");
            return new ObjectResult($"<html><body><h1>Error</h1><p>{ex.Message}</p></body></html>") {
                ContentType = "text/html",
                StatusCode = 500
            };
        }
    }
}
