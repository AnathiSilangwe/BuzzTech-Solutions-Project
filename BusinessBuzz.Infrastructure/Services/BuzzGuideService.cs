using BusinessBuzz.Core.DTOs;
using BusinessBuzz.Core.Services;
using Microsoft.Extensions.Configuration;
using OpenAI.Chat;

namespace BusinessBuzz.Infrastructure.Services;

public class BuzzGuideService : IBuzzGuideService
{
    private readonly IConfiguration _configuration;

    public BuzzGuideService(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public async Task<string> GetResponseAsync(
        BuzzGuideContext context,
        string question)
    {
        var apiKey = _configuration["OpenAI:ApiKey"];
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

        return completion.Content[0].Text;
    }
}