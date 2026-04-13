using DailyTaskTraker.Models;

namespace DailyTaskTraker.Data
{
    public class StreakRepository
    {
        private readonly DatabaseHelper _db = DatabaseHelper.Instance;

        public Task<Streak?> GetByTaskAsync(int taskId) =>
            _db.Db.Table<Streak>().Where(s => s.TaskId == taskId).FirstOrDefaultAsync();

        public Task<int> InsertAsync(Streak streak) =>
            _db.Db.InsertAsync(streak);

        public async Task IncrementAsync(int taskId)
        {
            var streak = await GetByTaskAsync(taskId);
            if (streak == null) return;

            streak.CurrentStreak++;
            if (streak.CurrentStreak > streak.LongestStreak)
                streak.LongestStreak = streak.CurrentStreak;

            streak.LastDoneDate = DateTime.Today;
            await _db.Db.UpdateAsync(streak);
        }

        public async Task ResetAsync(int taskId)
        {
            var streak = await GetByTaskAsync(taskId);
            if (streak == null) return;

            streak.CurrentStreak = 0;
            streak.LastDoneDate = null;
            await _db.Db.UpdateAsync(streak);
        }
    }
}
