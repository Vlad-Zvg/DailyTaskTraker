using DailyTaskTraker.Models;

namespace DailyTaskTraker.Data
{
    public class CategoryRepository
    {
        private readonly DatabaseHelper _db = DatabaseHelper.Instance;

        public Task<List<Category>> GetAllAsync() =>
            _db.Db.Table<Category>().ToListAsync();

        public Task<Category?> GetByIdAsync(int id) =>
            _db.Db.Table<Category>().Where(c => c.Id == id).FirstOrDefaultAsync();

        public Task<int> InsertAsync(Category category) =>
            _db.Db.InsertAsync(category);

        public Task<int> UpdateAsync(Category category) =>
            _db.Db.UpdateAsync(category);

        // Встроенные категории (Task, Habit) защищены от удаления
        public async Task<bool> DeleteAsync(Category category)
        {
            if (category.Type != CategoryType.Custom) return false;
            await _db.Db.DeleteAsync(category);
            return true;
        }
    }
}
