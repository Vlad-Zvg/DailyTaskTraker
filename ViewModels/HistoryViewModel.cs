using DailyTaskTraker.Data;
using DailyTaskTraker.Models;

namespace DailyTaskTraker.ViewModels
{
    // Управляет вкладкой History — логом выполненных задач
    public class HistoryViewModel : BaseViewModel
    {
        private readonly CompletionLogRepository _logRepo = new();
        private readonly TaskRepository _taskRepo = new();

        private List<CompletionLog> _logs = new();

        public List<CompletionLog> Logs
        {
            get => _logs;
            private set => SetProperty(ref _logs, value);
        }

        public async Task LoadAsync()
        {
            IsBusy = true;
            var logs = await _logRepo.GetAllAsync();

            // Подгружаем задачу для каждой записи лога
            foreach (var log in logs)
                log.Task = await _taskRepo.GetByIdAsync(log.TaskId);

            Logs = logs;
            IsBusy = false;
        }
    }
}
