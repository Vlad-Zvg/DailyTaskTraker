using DailyTaskTraker.Models;

namespace DailyTaskTraker.Data
{
    public class ReminderRepository
    {
        private readonly DatabaseHelper _db = DatabaseHelper.Instance;

        public Task<Reminder?> GetByTaskAsync(int taskId) =>
            _db.Db.Table<Reminder>().Where(r => r.TaskId == taskId).FirstOrDefaultAsync();

        public Task<List<Reminder>> GetAllAsync() =>
            _db.Db.Table<Reminder>().ToListAsync();

        public Task<int> InsertAsync(Reminder reminder) =>
            _db.Db.InsertAsync(reminder);

        public Task<int> UpdateAsync(Reminder reminder) =>
            _db.Db.UpdateAsync(reminder);

        public Task<int> DeleteAsync(Reminder reminder) =>
            _db.Db.DeleteAsync(reminder);
    }
}
