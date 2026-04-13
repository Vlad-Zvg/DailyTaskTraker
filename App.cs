using Android.App;
using Android.OS;
using Android.Runtime;
using DailyTaskTraker.Services;

namespace DailyTaskTraker
{
    [Application]
    public class App : Application
    {
        public App(IntPtr javaReference, JniHandleOwnership transfer)
            : base(javaReference, transfer) { }

        public override void OnCreate()
        {
            base.OnCreate();

            // Создаём канал уведомлений (Android 8.0+)
            NotificationHelper.CreateChannel(this);
        }
    }
}
