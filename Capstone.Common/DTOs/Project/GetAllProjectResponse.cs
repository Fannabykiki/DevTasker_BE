using Capstone.Common.DTOs.Paging;

namespace Capstone.Common.DTOs.Project
{
	public class GetAllProjectResponse
    {
        public PagedResponse<GetAllProjectViewModel> pagination { get; set; }
    }
}
