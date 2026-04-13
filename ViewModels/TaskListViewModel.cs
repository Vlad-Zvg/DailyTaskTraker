using DailyTaskTraker.Data;
using DailyTaskTraker.Models;

namespace DailyTaskTraker.ViewModels
{
    // Управляет списком задач для одной вкладки
    public class TaskListViewModel : BaseViewModel
    {
        private readonly TaskRepository _taskRepo = new();
        private readonly CategoryRepository _categoryRepo = new();
        private readonly StreakRepository _streakRepo = new();

        private List<TaskItem> _tasks = new();
        private List<Category> _categories = new();
        private int _currentTabId;

        public List<TaskItem> Tasks
        {
            get => _tasks;
            private set => SetProperty(ref _tasks, value);
        }

        public List<Category> Categories
        {
            get => _categories;
            private set => SetProperty(ref _categories, value);
        }

        public async Task LoadAsync(int tabId)
        {
            IsBusy = true;
            _currentTabId = tabId;

            _categories = await _categoryRepo.GetAllAsync();
            var tasks = await _taskRepo.GetByTabAsync(tabId);

            // Подгружаем категорию для каждой задачи
            foreach (var task in tasks)
                task.Category = _categories.FirstOrDefault(c => c.Id == task.CategoryId);

            Tasks = tasks;
            IsBusy = false;
        }

        public async Task MarkDoneAsync(TaskItem task)
        {
            await _taskRepo.MarkDoneAsync(task, _currentTabId);

            // Обновляем streak если это привычка
            if (task.Category?.Type == CategoryType.Habit)
                await _streakRepo.IncrementAsync(task.Id);

            await LoadAsync(_currentTabId);
        }

        public async Task MarkUndoneAsync(TaskItem task)
        {
            await _taskRepo.MarkUndoneAsync(task);
            await LoadAsync(_currentTabId);
        }

        public async Task DeleteAsync(TaskItem task)
        {
            await _taskRepo.DeleteAsync(task);
            await LoadAsync(_currentTabId);
        }
    }
}
