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
    private readonly IBuzzGuideKnowledgeService _knowledgeService;

    public BuzzGuideService(
        IConfiguration configuration,
        ILogger<BuzzGuideService> logger,
        IBuzzGuideKnowledgeService knowledgeService)
    {
        _configuration = configuration;
        _logger = logger;
        _knowledgeService = knowledgeService;
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
            // Retrieve relevant knowledge articles.
            List<KnowledgeArticle> articles = new();

            try
            {
                articles = await _knowledgeService.SearchAsync(question);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(
                    ex,
                    "Could not retrieve BuzzGuide knowledge articles.");
            }

            var knowledgeText = articles.Count == 0
                ? "No matching verified knowledge articles were found."
                : string.Join(
                    "\n\n",
                    articles.Select(article =>
                        $"""
                        TITLE: {article.Title}
                        TOPIC: {article.Topic}
                        SUMMARY: {article.Summary}
                        CONTENT: {article.Content}
                        SOURCE: {article.SourceName}
                        SOURCE URL: {article.SourceUrl}
                        LAST VERIFIED: {article.LastVerifiedAt:yyyy-MM-dd}
                        STEPS:
                        {string.Join(
                            "\n",
                            article.Steps
                                .OrderBy(step => step.StepNumber)
                                .Select(step =>
                                    $"{step.StepNumber}. {step.Instruction}"))}
                        """));

            // Include recent conversation history, avoiding duplication
            // if the current question is already the last saved message.
            var history = conversation.ToList();

            if (history.Count > 0 &&
                string.Equals(
                    history[^1].Message,
                    question,
                    StringComparison.OrdinalIgnoreCase))
            {
                history.RemoveAt(history.Count - 1);
            }

            var historyText = history.Count == 0
                ? "No previous conversation."
                : string.Join(
                    "\n",
                    history.TakeLast(10).Select(message =>
                        $"{message.Role}: {message.Message}"));

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

                Help first-time entrepreneurs understand and navigate
                their business journey.

                RESPONSE RULES
                - Answer the exact question first.
                - Be concise, practical and friendly.
                - Use numbered steps for procedures.
                - Use the supplied business profile and journey
                  records for questions about personal progress.
                - Treat application statuses in the supplied journey
                  as the source of truth.
                - Use the knowledge articles for general business
                  guidance when relevant.
                - Do not invent official requirements, fees, deadlines,
                  approval outcomes or funding guarantees.
                - Do not claim that an article is verified unless
                  its supplied source and verification date support it.
                - If no relevant knowledge article is available,
                  say that you do not have verified information for
                  that question and direct the user to an appropriate
                  official institution when possible.
                - Include the supplied official source URL when it
                  supports your answer.
                - Do not claim to have searched the live internet.
                - If information conflicts or appears incomplete,
                  explain the limitation instead of guessing.

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
                        $"Status: {stage.ApplicationStatus ?? "Not Started"}"))
                }

                KNOWLEDGE BASE
                {knowledgeText}

                RECENT CONVERSATION
                {historyText}

                CURRENT QUESTION
                {question}

                Give a focused answer based on the available evidence.
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
