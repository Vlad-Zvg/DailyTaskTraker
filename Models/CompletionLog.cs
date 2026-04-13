using SQLite;

namespace DailyTaskTraker.Models
{
    [Table("CompletionLog")]
    public class CompletionLog
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        [Indexed, NotNull]
        public int TaskId { get; set; }

        [Indexed, NotNull]
        public int TabId { get; set; }

        [NotNull]
        public DateTime CompletedAt { get; set; }

        // Навигационное свойство — не хранится в БД
        [Ignore] public TaskItem? Task { get; set; }
    }
}
