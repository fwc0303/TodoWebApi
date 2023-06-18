using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TodoWebApi.Models
{
    public class IsoDateConverter : IsoDateTimeConverter
    {
        public IsoDateConverter()
        {
            DateTimeFormat = "yyyy-MM-dd";
        }
    }

    public class Tasks
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public string Name { get; set; }

        public string? Description { get; set; }

        [Column(TypeName = "Date")]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        [JsonConverter(typeof(IsoDateConverter))]
        public DateTime DueDate { get; set; }

        public StatusEnum Status { get; set; }

        public PriorityEnum Priority { get; set; }
        public string Email { get; set; }
    }

    public enum StatusEnum
    {
        Aborted = 0,
        NotStarted = 1,
        InProgress = 2,
        Completed = 3
    }

    public enum PriorityEnum
    {
        Escalated = 1,
        Urgent = 2,
        Normal = 3
    }
}
