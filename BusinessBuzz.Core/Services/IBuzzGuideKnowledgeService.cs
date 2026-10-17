using BusinessBuzz.Core.Entities;



namespace BusinessBuzz.Core.Services;
public interface IBuzzGuideKnowledgeService{

    Task<List<KnowledgeArticle>> SearchAsync(string question);
}