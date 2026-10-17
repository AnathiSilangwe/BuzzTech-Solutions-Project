using BusinessBuzz.Core.Entities;
using BusinessBuzz.Core.Services;
using BusinessBuzz.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace BusinessBuzz.Infrastructure.Services;

public class BuzzGuideKnowledgeService : IBuzzGuideKnowledgeService
{
    private readonly BusinessBuzzDbContext _context;

    public BuzzGuideKnowledgeService(BusinessBuzzDbContext context)
    {
        _context = context;
    }

    public async Task<List<KnowledgeArticle>> SearchAsync(string question)
    {
        var terms = question
            .ToLowerInvariant()
            .Split(' ', StringSplitOptions.RemoveEmptyEntries)
            .Where(word => word.Length >= 3)
            .Distinct()
            .ToList();

        if (terms.Count == 0)
        {
            return new List<KnowledgeArticle>();
        }

        var articles = await _context.KnowledgeArticles
            .AsNoTracking()
            .Include(article => article.Steps)
            .Where(article => article.IsActive)
            .ToListAsync();

        return articles
            .Select(article => new
            {
                Article = article,
                Score = terms.Count(term =>
                    article.Title.Contains(
                        term, StringComparison.OrdinalIgnoreCase) ||
                    article.Topic.Contains(
                        term, StringComparison.OrdinalIgnoreCase) ||
                    article.Keywords.Contains(
                        term, StringComparison.OrdinalIgnoreCase) ||
                    article.Summary.Contains(
                        term, StringComparison.OrdinalIgnoreCase) ||
                    article.Content.Contains(
                        term, StringComparison.OrdinalIgnoreCase))
            })
            .Where(result => result.Score > 0)
            .OrderByDescending(result => result.Score)
            .Take(3)
            .Select(result =>
            {
                result.Article.Steps = result.Article.Steps
                    .OrderBy(step => step.StepNumber)
                    .ToList();

                return result.Article;
            })
            .ToList();
    }
}