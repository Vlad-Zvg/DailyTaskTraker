using Android.App;
using Android.OS;
using AndroidX.Core.App;

namespace DailyTaskTraker.Services
{
    public static class NotificationHelper
    {
        public const string ChannelId = "task_reminders";
        private const string ChannelName = "Напоминания о задачах";

        /// <summary>
        /// Создаёт канал уведомлений (нужен для Android 8.0+).
        /// Вызывается из Application.OnCreate().
        /// </summary>
        public static void CreateChannel(Android.Content.Context context)
        {
            if (Build.VERSION.SdkInt < BuildVersionCodes.O) return;

            var channel = new NotificationChannel(ChannelId, ChannelName, NotificationImportance.High)
            {
                Description = "Напоминания для задач и привычек"
            };
            channel.EnableVibration(true);

            var manager = (NotificationManager)context.GetSystemService(Android.Content.Context.NotificationService)!;
            manager.CreateNotificationChannel(channel);
        }

        /// <summary>
        /// Показывает уведомление-напоминание.
        /// </summary>
        public static void Show(Android.Content.Context context, int taskId, string taskTitle)
        {
            var builder = new NotificationCompat.Builder(context, ChannelId)
                .SetSmallIcon(Android.Resource.Drawable.IcDialogInfo)
                .SetContentTitle("Напоминание")
                .SetContentText(taskTitle)
                .SetAutoCancel(true)
                .SetPriority(NotificationCompat.PriorityHigh);

            NotificationManagerCompat.From(context).Notify(taskId, builder.Build());
        }
    }
}
