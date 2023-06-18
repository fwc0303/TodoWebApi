using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace TodoWebApi.Dto
{
    public class TasksDto
    {
        public string Name { get; set; }
        public string? Description { get; set; }
        [Column(TypeName = "Date")]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime DueDate { get; set; }
        public ushort Status { get; set; }
        public ushort Priority { get; set; }
    }
}
