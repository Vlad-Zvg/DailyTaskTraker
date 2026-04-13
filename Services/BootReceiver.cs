using Android.Content;
using DailyTaskTraker.Data;

namespace DailyTaskTraker.Services
{
    // Восстанавливает все будильники после перезагрузки устройства
    [BroadcastReceiver(Enabled = true, Exported = true)]
    [IntentFilter(new[] { Intent.ActionBootCompleted })]
    public class BootReceiver : BroadcastReceiver
    {
        public override void OnReceive(Context? context, Intent? intent)
        {
            if (context == null || intent?.Action != Intent.ActionBootCompleted) return;

            // GoAsync даёт дополнительное время на async-работу (по умолчанию ~10 сек)
            var pending = GoAsync();

            _ = System.Threading.Tasks.Task.Run(async () =>
            {
                try   { await RescheduleAllAsync(context); }
                finally { pending.Finish(); }
            });
        }

        private static async System.Threading.Tasks.Task RescheduleAllAsync(Context context)
        {
            await DatabaseHelper.Instance.InitAsync();

            var reminderRepo   = new ReminderRepository();
            var taskRepo       = new TaskRepository();
            var recurrenceRepo = new RecurrenceRepository();

            foreach (var reminder in await reminderRepo.GetAllAsync())
            {
                var task = await taskRepo.GetByIdAsync(reminder.TaskId);
                if (task == null) continue;

                var fireAt = reminder.RemindAt;

                // Для прошедших повторяющихся — сдвигаем на ближайшее будущее время
                if (fireAt <= DateTime.Now)
                {
                    if (!reminder.IsRecurring) continue;

                    var recurrence = await recurrenceRepo.GetByTaskAsync(reminder.TaskId);
                    if (recurrence == null) continue;

                    fireAt = AdvanceToFuture(recurrence, fireAt);
                    if (fireAt <= DateTime.Now) continue;

                    reminder.RemindAt = fireAt;
                }

                var rec = reminder.IsRecurring
                    ? await recurrenceRepo.GetByTaskAsync(reminder.TaskId)
                    : null;

                ReminderScheduler.Schedule(context, task, reminder, rec);
            }
        }

        // Прокручивает время вперёд шагами правила повторения, пока не попадём в будущее
        private static DateTime AdvanceToFuture(Models.Recurrence recurrence, DateTime from)
        {
            var current = from;
            while (current <= DateTime.Now)
            {
                var next = ReminderScheduler.NextOccurrence(recurrence.Rule, recurrence.DaysMask, current);
                if (next == null) return current; // повторений нет — вернём прошлое, вызывающий отфильтрует
                current = next.Value;
            }
            return current;
        }
    }
}
