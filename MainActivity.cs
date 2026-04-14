using Android.OS;
using AndroidX.AppCompat.App;
using AndroidX.ViewPager2.Widget;
using DailyTaskTraker.Adapters;
using DailyTaskTraker.Data;
using DailyTaskTraker.Models;
using DailyTaskTraker.ViewModels;
using Google.Android.Material.AppBar;
using Google.Android.Material.Tabs;

namespace DailyTaskTraker
{
    [Activity(Label = "@string/app_name", MainLauncher = true, Theme = "@style/AppTheme")]
    public class MainActivity : AppCompatActivity
    {
        private readonly MainViewModel _viewModel = new();
        private MaterialToolbar _toolbar = null!;
        private TabLayout _tabLayout = null!;
        private ViewPager2 _viewPager = null!;

        protected override void OnCreate(Bundle? savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.activity_main);

            _toolbar = FindViewById<MaterialToolbar>(Resource.Id.toolbar)!;
            SetSupportActionBar(_toolbar);

            _tabLayout = FindViewById<TabLayout>(Resource.Id.tabLayout)!;
            _viewPager = FindViewById<ViewPager2>(Resource.Id.viewPager)!;

            RequestNotificationPermission();
            _ = InitAsync();
        }

        private void RequestNotificationPermission()
        {
            // Android 13+ требует явного разрешения на уведомления
            if (Build.VERSION.SdkInt >= BuildVersionCodes.Tiramisu)
                RequestPermissions(new[] { Android.Manifest.Permission.PostNotifications }, 0);
        }

        private async Task InitAsync()
        {
            await DatabaseHelper.Instance.InitAsync();
            await _viewModel.LoadTabsAsync();

            RunOnUiThread(() =>
            {
                var adapter = new TabsPagerAdapter(this, _viewModel.Tabs);
                _viewPager.Adapter = adapter;

                new TabLayoutMediator(_tabLayout, _viewPager,
                    new TabConfigStrategy(_viewModel.Tabs)).Attach();
            });
        }

        private class TabConfigStrategy : Java.Lang.Object, TabLayoutMediator.ITabConfigurationStrategy
        {
            private readonly List<Tab> _tabs;

            public TabConfigStrategy(List<Tab> tabs) => _tabs = tabs;

            public void OnConfigureTab(TabLayout.Tab tab, int position)
                => tab.SetText(_tabs[position].Name);
        }
    }
}
