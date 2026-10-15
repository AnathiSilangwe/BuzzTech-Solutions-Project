using BusinessBuzz.Core.DTOs;
using BusinessBuzz.Core.Entities;
using BusinessBuzz.Core.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using OpenAI;
using OpenAI.Chat;
using System.ClientModel;

namespace BusinessBuzz.Infrastructure.Services;

public class BuzzGuideService : IBuzzGuideService
{
    private readonly IConfiguration _configuration;
    private readonly ILogger<BuzzGuideService> _logger;

    public BuzzGuideService(
        IConfiguration configuration,
        ILogger<BuzzGuideService> logger)
    {
        _configuration = configuration;
        _logger = logger;
    }

    public async Task<string> GetResponseAsync(
        BuzzGuideContext context,
        List<BuzzGuideMessage> conversation,
        string question)
    {
        var apiKey = _configuration["Groq:ApiKey"];
        var model = _configuration["Groq:Model"]
                    ?? "openai/gpt-oss-20b";

        if (string.IsNullOrWhiteSpace(apiKey))
        {
            return "BuzzGuide is not configured. Please contact support.";
        }

        try
        {
            var options = new OpenAIClientOptions
            {
                Endpoint = new Uri(
                    "https://api.groq.com/openai/v1")
            };

            var client = new ChatClient(
                model: model,
                credential: new ApiKeyCredential(apiKey),
                options: options);

            var prompt = $"""
                You are BuzzGuide, the business guidance assistant
                within the Business Buzz application.

                Your purpose is to help first-time entrepreneurs
                understand and navigate their business journey.

                Give clear, friendly, practical, step-by-step advice.
                Use the business context below to personalise your
                response. Do not invent official requirements,
                approval outcomes, or funding guarantees.
                When requirements may change, direct the entrepreneur
                to the relevant official institution.

                BUSINESS PROFILE
                Business name: {context.BusinessName}
                Business type: {context.BusinessType}
                Industry: {context.Industry}
                Location: {context.BusinessLocation}
                Employs people: {context.EmploysPeople}
                Seeking funding: {context.SeeksFunding}

                PERSONAL JOURNEY
                {string.Join(
                    "\n",
                    context.Journey.Select(stage =>
                        $"- {stage.StageOrder}. {stage.StageName}; " +
                        $"Institution: {stage.InstitutionName}; " +
                        $"Status: {stage.ApplicationStatus ?? "Not Started"}"
                    )
                )}

                ENTREPRENEUR'S QUESTION
                {question}

                Give a useful answer focused on the entrepreneur's
                question and their current stage of the journey.
                """;

            ChatCompletion completion =
                await client.CompleteChatAsync(prompt);

            var answer = completion.Content.FirstOrDefault()?.Text;

            return string.IsNullOrWhiteSpace(answer)
                ? "BuzzGuide couldn't generate a response. Please try again."
                : answer;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "BuzzGuide AI request failed.");

            return "BuzzGuide is temporarily unavailable. Please try again.";
        }
    }
}