using DailyTaskTraker.Models;

namespace DailyTaskTraker.Data
{
    public class RecurrenceRepository
    {
        private readonly DatabaseHelper _db = DatabaseHelper.Instance;

        public Task<Recurrence?> GetByTaskAsync(int taskId) =>
            _db.Db.Table<Recurrence>().Where(r => r.TaskId == taskId).FirstOrDefaultAsync();

        public Task<int> InsertAsync(Recurrence recurrence) =>
            _db.Db.InsertAsync(recurrence);

        public Task<int> UpdateAsync(Recurrence recurrence) =>
            _db.Db.UpdateAsync(recurrence);

        public Task<int> DeleteAsync(Recurrence recurrence) =>
            _db.Db.DeleteAsync(recurrence);
    }
}
