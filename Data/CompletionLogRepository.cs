using DailyTaskTraker.Models;

namespace DailyTaskTraker.Data
{
    public class CompletionLogRepository
    {
        private readonly DatabaseHelper _db = DatabaseHelper.Instance;

        public Task<List<CompletionLog>> GetAllAsync() =>
            _db.Db.Table<CompletionLog>().OrderByDescending(l => l.CompletedAt).ToListAsync();

        public Task<List<CompletionLog>> GetByTaskAsync(int taskId) =>
            _db.Db.Table<CompletionLog>().Where(l => l.TaskId == taskId).OrderByDescending(l => l.CompletedAt).ToListAsync();

        public Task<int> InsertAsync(CompletionLog log) =>
            _db.Db.InsertAsync(log);
    }
}
