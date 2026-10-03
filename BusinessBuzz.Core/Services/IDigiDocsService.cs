using BusinessBuzz.Core.Entities;

namespace BusinessBuzz.Core.Services;

public interface IDigiDocsService
{
    Task<Document?> GenerateBusinessInformationDocumentAsync(
        int businessProfileId,
        string idNumber);
}