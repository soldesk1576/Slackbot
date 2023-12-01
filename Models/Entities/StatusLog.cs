using StatusNotifier.Models.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace StatusNotifier.Models.Entities
{
    public class StatusLog
    {
        [Required]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long ID { get; set; }

        [Required]
        public DateTime CreatedDateTime { get; set; } = DateTime.Now;

        [Required]
        public ECallType CallType { get; set; }

        [Required]
        public EStatus Status { get; set; }

        public string ErrorCause { get; set; }
    }
}
