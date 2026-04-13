using SQLite;

namespace DailyTaskTraker.Models
{
    public enum RecurrenceRule { Daily, Weekly, SpecificDays }

    [Table("Recurrences")]
    public class Recurrence
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        [Indexed, NotNull]
        public int TaskId { get; set; }

        [NotNull]
        public RecurrenceRule Rule { get; set; }

        // Битовая маска дней: пн=1, вт=2, ср=4, чт=8, пт=16, сб=32, вс=64
        public int DaysMask { get; set; }

        public bool IsActiveOnDay(DayOfWeek day)
        {
            int bit = 1 << ((int)day + 6) % 7;
            return (DaysMask & bit) != 0;
        }
    }
}
