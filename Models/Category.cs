using SQLite;

namespace DailyTaskTraker.Models
{
    public enum CategoryType { Task, Habit, Custom }

    [Table("Categories")]
    public class Category
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        [NotNull]
        public string Name { get; set; } = string.Empty;

        [NotNull]
        public CategoryType Type { get; set; }

        [NotNull]
        public string Color { get; set; } = "#FFFFFF";
    }
}
