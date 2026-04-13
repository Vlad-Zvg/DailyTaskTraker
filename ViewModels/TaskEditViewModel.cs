using DailyTaskTraker.Data;
using DailyTaskTraker.Models;

namespace DailyTaskTraker.ViewModels
{
    // Управляет созданием и редактированием задачи
    public class TaskEditViewModel : BaseViewModel
    {
        private readonly TaskRepository _taskRepo = new();
        private readonly CategoryRepository _categoryRepo = new();
        private readonly RecurrenceRepository _recurrenceRepo = new();
        private readonly ReminderRepository _reminderRepo = new();
        private readonly StreakRepository _streakRepo = new();

        private TaskItem _task = new();
        private List<Category> _categories = new();
        private Recurrence? _recurrence;
        private Reminder? _reminder;
        private bool _isEditMode;

        public TaskItem Task
        {
            get => _task;
            set => SetProperty(ref _task, value);
        }

        public List<Category> Categories
        {
            get => _categories;
            private set => SetProperty(ref _categories, value);
        }

        public Recurrence? Recurrence
        {
            get => _recurrence;
            set => SetProperty(ref _recurrence, value);
        }

        public Reminder? Reminder
        {
            get => _reminder;
            set => SetProperty(ref _reminder, value);
        }

        // Загрузка для создания новой задачи
        public async Task InitNewAsync(int tabId)
        {
            _isEditMode = false;
            Task = new TaskItem { TabId = tabId, CreatedAt = DateTime.Now };
            Categories = await _categoryRepo.GetAllAsync();
            Task.CategoryId = Categories.FirstOrDefault()?.Id ?? 0;
            Recurrence = null;
            Reminder = null;
        }

        // Загрузка для редактирования существующей задачи
        public async Task InitEditAsync(int taskId)
        {
            _isEditMode = true;
            Categories = await _categoryRepo.GetAllAsync();
            Task = await _taskRepo.GetByIdAsync(taskId) ?? new TaskItem();
            Recurrence = await _recurrenceRepo.GetByTaskAsync(taskId);
            Reminder = await _reminderRepo.GetByTaskAsync(taskId);
        }

        public async Task<bool> SaveAsync()
        {
            if (string.IsNullOrWhiteSpace(Task.Title)) return false;

            if (_isEditMode)
            {
                await _taskRepo.UpdateAsync(Task);
            }
            else
            {
                await _taskRepo.InsertAsync(Task);

                // Создаём streak для привычки
                var category = Categories.FirstOrDefault(c => c.Id == Task.CategoryId);
                if (category?.Type == CategoryType.Habit)
                {
                    await _streakRepo.InsertAsync(new Streak { TaskId = Task.Id });
                }
            }

            // Сохраняем повтор
            if (Recurrence != null)
            {
                Recurrence.TaskId = Task.Id;
                if (Recurrence.Id == 0)
                    await _recurrenceRepo.InsertAsync(Recurrence);
                else
                    await _recurrenceRepo.UpdateAsync(Recurrence);
            }

            // Сохраняем напоминание
            if (Reminder != null)
            {
                Reminder.TaskId = Task.Id;
                if (Reminder.Id == 0)
                    await _reminderRepo.InsertAsync(Reminder);
                else
                    await _reminderRepo.UpdateAsync(Reminder);
            }

            return true;
        }
    }
}
