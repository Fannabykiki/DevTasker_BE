﻿using Capstone.Common.Enums;

namespace Capstone.Common.DTOs.Project;

public class CreateProjectRequest
{
   public string ProjectName {get;set;}
   public string Description {get;set;}
   public DateTime StartDate { get; set; }
   public DateTime EndDate { get; set; }
   public bool PrivacyStatus { get; set; }
}