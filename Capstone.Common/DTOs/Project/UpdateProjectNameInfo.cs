namespace Capstone.Common.DTOs.Project;

public class UpdateProjectNameInfo
{
	public Guid ProjectId { get; set; }
	public string ProjectName { get; set; }
    public string Description { get; set; }
    public DateTime EndDate { get; set; }
}