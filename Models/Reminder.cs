using SQLite;

namespace DailyTaskTraker.Models
{
    [Table("Reminders")]
    public class Reminder
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        [Indexed, NotNull]
        public int TaskId { get; set; }

        [NotNull]
        public DateTime RemindAt { get; set; }

        [NotNull]
        public bool IsRecurring { get; set; }
    }
}
