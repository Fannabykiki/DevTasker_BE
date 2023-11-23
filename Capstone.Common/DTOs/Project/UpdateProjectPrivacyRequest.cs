namespace Capstone.Common.DTOs.Project;

public class UpdateProjectPrivacyRequest
{
    public Guid ProjectId { get; set; }
    public bool PrivacyStatus { get; set; } // false: Private , true: Public
}