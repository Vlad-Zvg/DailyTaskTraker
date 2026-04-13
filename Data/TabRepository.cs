using DailyTaskTraker.Models;

namespace DailyTaskTraker.Data
{
    public class TabRepository
    {
        private readonly DatabaseHelper _db = DatabaseHelper.Instance;

        public Task<List<Tab>> GetAllAsync() =>
            _db.Db.Table<Tab>().OrderBy(t => t.SortOrder).ToListAsync();

        public Task<Tab?> GetByIdAsync(int id) =>
            _db.Db.Table<Tab>().Where(t => t.Id == id).FirstOrDefaultAsync();

        public Task<int> InsertAsync(Tab tab) =>
            _db.Db.InsertAsync(tab);

        public Task<int> UpdateAsync(Tab tab) =>
            _db.Db.UpdateAsync(tab);

        // Системные вкладки удалять нельзя
        public async Task<bool> DeleteAsync(Tab tab)
        {
            if (tab.IsSystem) return false;
            await _db.Db.DeleteAsync(tab);
            return true;
        }
    }
}
