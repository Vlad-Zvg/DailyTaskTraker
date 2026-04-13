using SQLite;
using DailyTaskTraker.Models;

namespace DailyTaskTraker.Data
{
    public class DatabaseHelper
    {
        private static DatabaseHelper? _instance;
        private readonly SQLiteAsyncConnection _db;

        private DatabaseHelper(string dbPath)
        {
            _db = new SQLiteAsyncConnection(dbPath);
        }

        public static DatabaseHelper Instance
        {
            get
            {
                if (_instance == null)
                {
                    string dbPath = Path.Combine(
                        Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                        "dailytasktraker.db"
                    );
                    _instance = new DatabaseHelper(dbPath);
                }
                return _instance;
            }
        }

        public SQLiteAsyncConnection Db => _db;

        public async Task InitAsync()
        {
            await _db.CreateTableAsync<Tab>();
            await _db.CreateTableAsync<Category>();
            await _db.CreateTableAsync<TaskItem>();
            await _db.CreateTableAsync<Recurrence>();
            await _db.CreateTableAsync<Reminder>();
            await _db.CreateTableAsync<Streak>();
            await _db.CreateTableAsync<CompletionLog>();

            await SeedDefaultDataAsync();
        }

        // Заполняет системные вкладки и категории при первом запуске
        private async Task SeedDefaultDataAsync()
        {
            int tabCount = await _db.Table<Tab>().CountAsync();
            if (tabCount == 0)
            {
                await _db.InsertAllAsync(new List<Tab>
                {
                    new Tab { Name = "Daily",   Type = TabType.Daily,   SortOrder = 0, IsSystem = true },
                    new Tab { Name = "Weekly",  Type = TabType.Weekly,  SortOrder = 1, IsSystem = true },
                    new Tab { Name = "Other",   Type = TabType.Other,   SortOrder = 2, IsSystem = true },
                    new Tab { Name = "History", Type = TabType.History, SortOrder = 999, IsSystem = true },
                });
            }

            int categoryCount = await _db.Table<Category>().CountAsync();
            if (categoryCount == 0)
            {
                await _db.InsertAllAsync(new List<Category>
                {
                    new Category { Name = "Задача",   Type = CategoryType.Task,  Color = "#4A90E2" },
                    new Category { Name = "Привычка", Type = CategoryType.Habit, Color = "#7ED321" },
                });
            }
        }
    }
}
