using Capstone.Common.Enums;

namespace Capstone.Common.DTOs.Project;

public class CreateProjectRequest
{
   public string ProjectName {get;set;}

   public DateTime StartDate { get; set; }
   public DateTime EndDate { get; set; }
   public Guid CreateBy{ get; set; }
   public Guid BoardId { get; set; }
   public DateTime CreateAt { get; set; }
   public StatusEnum ProjectStatus { get; set; }
   public bool PrivacyStatus { get; set; }
}