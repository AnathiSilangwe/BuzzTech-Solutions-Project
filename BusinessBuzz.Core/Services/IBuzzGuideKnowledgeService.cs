using BusinessBuzz.Core.Entities;
using BusinessBuzz.Core.Entitites;


namespace BusinessBuzz.Core.Services;
public interface IBuzzGuideKnowledgeService{

    Task<List<KnowledgeArticle>> SearchAsync(string question);
}