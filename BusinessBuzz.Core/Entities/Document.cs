namespace BusinessBuzz.Core.Entities;

public class Document
{
    public int DocumentId { get; set; }
    public int BusinessProfileId { get; set; }
    public int DocumentTypeId { get; set; }
    public string FilePath { get; set; } = string.Empty;
    public bool IsSystemGenerated { get; set; }
    public bool IsVerified { get; set; }
    public DateTime UploadedAt { get; set; }

    //Navigation property 
    public BusinessProfile BusinessProfile { get; set; } = null!;
    public DocumentType DocumentType { get; set; } = null!;
    public void UploadFile()
    {
        //Implemention will be inserted here
    }
    public void VerifyDocument()
    {
        IsVerified = true;
    }

}