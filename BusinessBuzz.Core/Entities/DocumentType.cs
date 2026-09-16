using System.Reflection.Metadata;

namespace BusinessBuzz.Core.Entities;

public class DocumentType
{
    public int DocumentTypeId { get; set; }
    public string TypeName { get; set; } = string.Empty;
    public bool IsRequired { get; set; }

    //Navigation property
    public ICollection<Document> Documents { get; set; } = new List<Document>();
    
}