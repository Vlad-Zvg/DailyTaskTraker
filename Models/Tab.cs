using SQLite;

namespace DailyTaskTraker.Models
{
    public enum TabType { Daily, Weekly, Other, Custom, History }

    [Table("Tabs")]
    public class Tab
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        [NotNull]
        public string Name { get; set; } = string.Empty;

        [NotNull]
        public TabType Type { get; set; }

        [NotNull]
        public int SortOrder { get; set; }

        [NotNull]
        public bool IsSystem { get; set; }
    }
}
