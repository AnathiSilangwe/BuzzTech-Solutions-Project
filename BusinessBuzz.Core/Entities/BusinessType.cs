namespace BusinessBuzz.Core.Entities;

public class BusinessType
{
    public int BusinessTypeId { get; set; }
    public string TypeName { get ; set; } = string.Empty;

    //Navigation property
    public ICollection<BusinessProfile> BusinessProfiles { get; set; } = new List<BusinessProfile>();
    public List<string> GetRequirements()
    {
        return TypeName switch
        {
            "Sole Proprietorship" => new List<string>
            {
                "Identity Information",
                "Business Information"
            },
            "Private Company" => new List<string>
            {
                "Identity Information",
                "Business Information",
                "Company Registration Information"
            },
            "Partnership" => new List<string>
            {
                "Identity Information",
                "Business Information",
                "Partner Information"
            },

            _ => new List<string>()
        };
    }
}