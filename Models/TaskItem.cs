using SQLite;

namespace DailyTaskTraker.Models
{
    [Table("Tasks")]
    public class TaskItem
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        [NotNull]
        public string Title { get; set; } = string.Empty;

        [Indexed, NotNull]
        public int TabId { get; set; }

        [Indexed, NotNull]
        public int CategoryId { get; set; }

        [NotNull]
        public bool IsDone { get; set; }

        [NotNull]
        public bool IsRecurring { get; set; }

        [NotNull]
        public DateTime CreatedAt { get; set; }

        // Навигационные свойства — не хранятся в БД
        [Ignore] public Category? Category { get; set; }
        [Ignore] public Recurrence? Recurrence { get; set; }
        [Ignore] public Reminder? Reminder { get; set; }
        [Ignore] public Streak? Streak { get; set; }
    }
}
