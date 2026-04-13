using DailyTaskTraker.Data;
using DailyTaskTraker.Models;

namespace DailyTaskTraker.ViewModels
{
    // Управляет списком категорий
    public class CategoryViewModel : BaseViewModel
    {
        private readonly CategoryRepository _categoryRepo = new();

        private List<Category> _categories = new();

        public List<Category> Categories
        {
            get => _categories;
            private set => SetProperty(ref _categories, value);
        }

        public async Task LoadAsync()
        {
            IsBusy = true;
            Categories = await _categoryRepo.GetAllAsync();
            IsBusy = false;
        }

        public async Task AddAsync(string name, string color)
        {
            var category = new Category
            {
                Name = name,
                Type = CategoryType.Custom,
                Color = color
            };
            await _categoryRepo.InsertAsync(category);
            await LoadAsync();
        }

        public async Task<bool> DeleteAsync(Category category)
        {
            bool deleted = await _categoryRepo.DeleteAsync(category);
            if (deleted) await LoadAsync();
            return deleted;
        }
    }
}
