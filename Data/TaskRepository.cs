using DailyTaskTraker.Models;

namespace DailyTaskTraker.Data
{
    public class TaskRepository
    {
        private readonly DatabaseHelper _db = DatabaseHelper.Instance;

        public Task<List<TaskItem>> GetByTabAsync(int tabId) =>
            _db.Db.Table<TaskItem>().Where(t => t.TabId == tabId).ToListAsync();

        public Task<TaskItem?> GetByIdAsync(int id) =>
            _db.Db.Table<TaskItem>().Where(t => t.Id == id).FirstOrDefaultAsync();

        public Task<int> InsertAsync(TaskItem task) =>
            _db.Db.InsertAsync(task);

        public Task<int> UpdateAsync(TaskItem task) =>
            _db.Db.UpdateAsync(task);

        public Task<int> DeleteAsync(TaskItem task) =>
            _db.Db.DeleteAsync(task);

        public async Task MarkDoneAsync(TaskItem task, int tabId)
        {
            task.IsDone = true;
            await _db.Db.UpdateAsync(task);

            await _db.Db.InsertAsync(new CompletionLog
            {
                TaskId = task.Id,
                TabId = tabId,
                CompletedAt = DateTime.Now
            });
        }

        public async Task MarkUndoneAsync(TaskItem task)
        {
            task.IsDone = false;
            await _db.Db.UpdateAsync(task);
        }
    }
}
