using BusinessBuzz.Core.DTOs;
using BusinessBuzz.Core.Services;
using Microsoft.Extensions.Configuration;
using BusinessBuzz.Core.Entities;
using OpenAI.Chat;

namespace BusinessBuzz.Infrastructure.Services;

public class BuzzGuideService : IBuzzGuideService
{
    private readonly IConfiguration _configuration;

    public BuzzGuideService(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public Task<string> GetResponseAsync( BuzzGuideContext context, List<BuzzGuideMessage> conversation, string question)
    {
        /* var apiKey = _configuration["OpenAI:ApiKey"];
        var model = _configuration["OpenAI:Model"];

        if (string.IsNullOrWhiteSpace(apiKey))
        {
            return "BuzzGuide is not configured with an AI API key.";
        }

        if (string.IsNullOrWhiteSpace(model))
        {
            return "BuzzGuide AI model is not configured.";
        }

        var client = new ChatClient(
            model: model,
            apiKey: apiKey);

        var prompt = $"""
            You are BuzzGuide, an assistant within the Business Buzz
            application.

            Your role is to guide first-time entrepreneurs through
            their business journey.

            Use the entrepreneur's provided business context when
            answering the question.

            Business:
            Name: {context.BusinessName}
            Type: {context.BusinessType}
            Industry: {context.Industry}
            Location: {context.BusinessLocation}
            Employs People: {context.EmploysPeople}
            Seeks Funding: {context.SeeksFunding}

            Journey:
            {string.Join(
                "\n",
                context.Journey.Select(stage =>
                    $"- {stage.StageOrder}. {stage.StageName} " +
                    $"({stage.InstitutionName}) - " +
                    $"Status: {stage.ApplicationStatus ?? "Not Started"}"
                )
            )}

            Entrepreneur's question:
            {question}
            """;

        ChatCompletion completion =
            await client.CompleteChatAsync(prompt);

        return completion.Content[0].Text; */

         try
        {
            var previousMessages = conversation.Count;

            var response =
                $"Development response: I received your question " +
                $"\"{question}\". " +
                $"Your business is {context.BusinessName}, " +
                $"a {context.BusinessType} in the {context.Industry} industry. " +
                $"Your personalised journey currently contains " +
                $"{context.Journey.Count} stages. " +
                $"I can also see {previousMessages} previous conversation messages.";

            return Task.FromResult(response);
        }
        catch (Exception)
        {
            return Task.FromResult(
                "BuzzGuide is temporarily unavailable. " +
                "Please try again later.");
        }
    }
}