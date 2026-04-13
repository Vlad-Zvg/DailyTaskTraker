using SQLite;

namespace DailyTaskTraker.Models
{
    [Table("Streaks")]
    public class Streak
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        [Indexed, NotNull]
        public int TaskId { get; set; }

        [NotNull]
        public int CurrentStreak { get; set; }

        [NotNull]
        public int LongestStreak { get; set; }

        public DateTime? LastDoneDate { get; set; }
    }
}
