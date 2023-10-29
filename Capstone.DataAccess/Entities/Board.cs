using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Capstone.Common.Enums;

namespace Capstone.DataAccess.Entities
{
    public class Board
    {
        [Key]
        public Guid BoardId { get; set; }
        public string Title { get; set; }
        public DateTime CreateAt { get; set; }
        public DateTime? UpdateAt { get; set; }
        public DateTime? DeleteAt { get; set; }
        [ForeignKey("Status")] 
        public Guid StatusId { get; set; }
        public Guid? ProjectId { get; set; }
        public List<Interation>? Interations { get; set; }
        [ForeignKey("StatusId")]
        public Status? Status { get; set; }
        public Project? Project { get; set; }
    }
}
