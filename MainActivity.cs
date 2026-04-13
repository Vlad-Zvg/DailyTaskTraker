using DailyTaskTraker.Data;

namespace DailyTaskTraker
{
    [Activity(Label = "@string/app_name", MainLauncher = true)]
    public class MainActivity : Activity
    {
        protected override void OnCreate(Bundle? savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.activity_main);

            _ = InitDatabaseAsync();
        }

        private async Task InitDatabaseAsync()
        {
            await DatabaseHelper.Instance.InitAsync();
        }
    }
}
