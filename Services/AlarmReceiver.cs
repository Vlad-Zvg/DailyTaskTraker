using Android.Content;
using DailyTaskTraker.Models;

namespace DailyTaskTraker.Services
{
    // Exported=false: PendingIntent с явным компонентом работает без экспорта
    [BroadcastReceiver(Enabled = true, Exported = false)]
    public class AlarmReceiver : BroadcastReceiver
    {
        public const string ExtraTaskId       = "task_id";
        public const string ExtraTaskTitle    = "task_title";
        public const string ExtraIsRecurring  = "is_recurring";
        public const string ExtraRecurrenceRule = "recurrence_rule";
        public const string ExtraDaysMask     = "days_mask";
        public const string ExtraRemindAtMs   = "remind_at_ms";

        public override void OnReceive(Context? context, Intent? intent)
        {
            if (context == null || intent == null) return;

            int taskId     = intent.GetIntExtra(ExtraTaskId, 0);
            string title   = intent.GetStringExtra(ExtraTaskTitle) ?? "Задача";
            bool recurring = intent.GetBooleanExtra(ExtraIsRecurring, false);
            long firedMs   = intent.GetLongExtra(ExtraRemindAtMs, 0);

            // Показываем уведомление
            NotificationHelper.Show(context, taskId, title);

            // Для повторяющихся — планируем следующий будильник
            if (!recurring || firedMs == 0) return;

            int ruleInt  = intent.GetIntExtra(ExtraRecurrenceRule, 0);
            int daysMask = intent.GetIntExtra(ExtraDaysMask, 0);
            var rule     = (RecurrenceRule)ruleInt;

            var firedAt = DateTimeOffset.FromUnixTimeMilliseconds(firedMs).LocalDateTime;
            var next    = ReminderScheduler.NextOccurrence(rule, daysMask, firedAt);
            if (next == null) return;

            // Строим Intent для следующего срабатывания
            var nextIntent = new Intent(context, typeof(AlarmReceiver));
            nextIntent.PutExtra(ExtraTaskId,        taskId);
            nextIntent.PutExtra(ExtraTaskTitle,      title);
            nextIntent.PutExtra(ExtraIsRecurring,    true);
            nextIntent.PutExtra(ExtraRecurrenceRule, ruleInt);
            nextIntent.PutExtra(ExtraDaysMask,       daysMask);
            nextIntent.PutExtra(ExtraRemindAtMs,
                new DateTimeOffset(next.Value.ToUniversalTime()).ToUnixTimeMilliseconds());

            ReminderScheduler.ScheduleAlarm(context, taskId, next.Value, nextIntent);
        }
    }
}
