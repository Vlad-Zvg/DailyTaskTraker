using Android.App;
using Android.Content;
using Android.OS;
using DailyTaskTraker.Models;

namespace DailyTaskTraker.Services
{
    public static class ReminderScheduler
    {
        /// <summary>
        /// Планирует будильник для задачи. Если reminder == null — отменяет существующий.
        /// </summary>
        public static void Schedule(Context context, TaskItem task, Reminder reminder, Recurrence? recurrence)
        {
            var intent = BuildIntent(context, task.Id, task.Title, reminder, recurrence);
            ScheduleAlarm(context, task.Id, reminder.RemindAt, intent);
        }

        /// <summary>
        /// Отменяет будильник для задачи.
        /// </summary>
        public static void Cancel(Context context, int taskId)
        {
            var alarmManager = GetAlarmManager(context);
            var intent = new Intent(context, typeof(AlarmReceiver));
            var pending = GetPendingIntent(context, taskId, intent);
            if (pending != null)
                alarmManager.Cancel(pending);
        }

        /// <summary>
        /// Планирует конкретный Intent на заданное время. Используется также из BootReceiver и AlarmReceiver.
        /// </summary>
        public static void ScheduleAlarm(Context context, int requestCode, DateTime fireAt, Intent intent)
        {
            var alarmManager = GetAlarmManager(context);
            var pending = GetPendingIntent(context, requestCode, intent);
            if (pending == null) return;

            long triggerMs = new DateTimeOffset(fireAt.ToUniversalTime()).ToUnixTimeMilliseconds();

            if (Build.VERSION.SdkInt >= BuildVersionCodes.S && !alarmManager.CanScheduleExactAlarms())
            {
                // Пользователь не выдал разрешение на точные будильники — используем неточный
                alarmManager.Set(AlarmType.RtcWakeup, triggerMs, pending);
            }
            else if (Build.VERSION.SdkInt >= BuildVersionCodes.M)
            {
                alarmManager.SetExactAndAllowWhileIdle(AlarmType.RtcWakeup, triggerMs, pending);
            }
            else
            {
                alarmManager.SetExact(AlarmType.RtcWakeup, triggerMs, pending);
            }
        }

        /// <summary>
        /// Вычисляет следующее время срабатывания для повторяющегося напоминания.
        /// </summary>
        public static DateTime? NextOccurrence(RecurrenceRule rule, int daysMask, DateTime previous)
        {
            return rule switch
            {
                RecurrenceRule.Daily => previous.AddDays(1),
                RecurrenceRule.Weekly => previous.AddDays(7),
                RecurrenceRule.SpecificDays => FindNextSpecificDay(daysMask, previous),
                _ => null
            };
        }

        // ──────────────────────────────────────────────────────────
        // Private helpers
        // ──────────────────────────────────────────────────────────

        private static Intent BuildIntent(Context context, int taskId, string title, Reminder reminder, Recurrence? recurrence)
        {
            var intent = new Intent(context, typeof(AlarmReceiver));
            intent.PutExtra(AlarmReceiver.ExtraTaskId, taskId);
            intent.PutExtra(AlarmReceiver.ExtraTaskTitle, title);
            intent.PutExtra(AlarmReceiver.ExtraIsRecurring, reminder.IsRecurring);
            intent.PutExtra(AlarmReceiver.ExtraRemindAtMs,
                new DateTimeOffset(reminder.RemindAt.ToUniversalTime()).ToUnixTimeMilliseconds());

            if (reminder.IsRecurring && recurrence != null)
            {
                intent.PutExtra(AlarmReceiver.ExtraRecurrenceRule, (int)recurrence.Rule);
                intent.PutExtra(AlarmReceiver.ExtraDaysMask, recurrence.DaysMask);
            }

            return intent;
        }

        private static AlarmManager GetAlarmManager(Context context)
            => (AlarmManager)context.GetSystemService(Context.AlarmService)!;

        private static PendingIntent? GetPendingIntent(Context context, int requestCode, Intent intent)
        {
            var flags = PendingIntentFlags.UpdateCurrent;
            if (Build.VERSION.SdkInt >= BuildVersionCodes.M)
                flags |= PendingIntentFlags.Immutable;

            return PendingIntent.GetBroadcast(context, requestCode, intent, flags);
        }

        private static DateTime? FindNextSpecificDay(int daysMask, DateTime from)
        {
            if (daysMask == 0) return null;

            for (int i = 1; i <= 7; i++)
            {
                var candidate = from.AddDays(i);
                if ((daysMask & DayToBit(candidate.DayOfWeek)) != 0)
                    return new DateTime(candidate.Year, candidate.Month, candidate.Day,
                        from.Hour, from.Minute, 0);
            }
            return null;
        }

        private static int DayToBit(DayOfWeek day) => day switch
        {
            DayOfWeek.Monday    => 1,
            DayOfWeek.Tuesday   => 2,
            DayOfWeek.Wednesday => 4,
            DayOfWeek.Thursday  => 8,
            DayOfWeek.Friday    => 16,
            DayOfWeek.Saturday  => 32,
            DayOfWeek.Sunday    => 64,
            _                   => 0
        };
    }
}
