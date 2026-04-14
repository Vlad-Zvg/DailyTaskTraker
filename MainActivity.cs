using Android.Content;
using Android.OS;
using AndroidX.AppCompat.App;
using AndroidX.DrawerLayout.Widget;
using DailyTaskTraker.Data;
using DailyTaskTraker.Views;
using Google.Android.Material.BottomNavigation;
using Google.Android.Material.Navigation;

namespace DailyTaskTraker
{
    [Activity(Label = "@string/app_name", MainLauncher = true, Theme = "@style/AppTheme")]
    public class MainActivity : AppCompatActivity
    {
        private const string PrefsName = "app_settings";
        private const string KeyDarkMode = "dark_mode";
        private const string KeyNavItem = "selected_nav_item";

        private DrawerLayout _drawerLayout = null!;
        private NavigationView _navigationView = null!;
        private BottomNavigationView _bottomNav = null!;

        // Кэш фрагментов
        private TaskListFragment? _homeFragment;
        private HistoryFragment? _historyFragment;
        private SettingsFragment? _settingsFragment;

        protected override void OnCreate(Bundle? savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Применяем сохранённую тему ДО SetContentView
            ApplyStoredTheme();

            SetContentView(Resource.Layout.activity_main);

            _drawerLayout = FindViewById<DrawerLayout>(Resource.Id.drawerLayout)!;
            _navigationView = FindViewById<NavigationView>(Resource.Id.navigationView)!;
            _bottomNav = FindViewById<BottomNavigationView>(Resource.Id.bottomNav)!;

            RequestNotificationPermission();
            SetupNavigation(savedInstanceState);
            _ = InitDbAsync();
        }

        protected override void OnSaveInstanceState(Bundle outState)
        {
            base.OnSaveInstanceState(outState);
            // Сохраняем выбранный элемент навигации (нужно для Recreate при смене темы)
            outState.PutInt(KeyNavItem, _bottomNav.SelectedItemId);
        }

        private void ApplyStoredTheme()
        {
            var prefs = GetSharedPreferences(PrefsName, FileCreationMode.Private);
            bool darkMode = prefs!.GetBoolean(KeyDarkMode, false);
            Delegate?.SetLocalNightMode(darkMode
                ? AppCompatDelegate.ModeNightYes
                : AppCompatDelegate.ModeNightNo);
        }

        private void RequestNotificationPermission()
        {
            if (Build.VERSION.SdkInt >= BuildVersionCodes.Tiramisu)
                RequestPermissions(new[] { Android.Manifest.Permission.PostNotifications }, 0);
        }

        private void SetupNavigation(Bundle? savedInstanceState)
        {
            int initialId = savedInstanceState?.GetInt(KeyNavItem, Resource.Id.nav_tasks)
                            ?? Resource.Id.nav_tasks;

            // Bottom Navigation
            _bottomNav.SetOnItemSelectedListener(new BottomNavListener(id =>
            {
                ShowFragmentForId(id);
                return true;
            }));

            // Navigation Drawer
            _navigationView.SetNavigationItemSelectedListener(new DrawerNavListener(id =>
            {
                ShowFragmentForId(id);
                _bottomNav.SelectedItemId = id;
                _drawerLayout.CloseDrawer(_navigationView);
                return true;
            }));

            // Восстанавливаем/устанавливаем начальный экран
            ShowFragmentForId(initialId);
            _bottomNav.SelectedItemId = initialId;
        }

        private void ShowFragmentForId(int id)
        {
            if (id == Resource.Id.nav_tasks) ShowHome();
            else if (id == Resource.Id.nav_history) ShowHistory();
            else if (id == Resource.Id.nav_settings) ShowSettings();
        }

        private async Task InitDbAsync()
        {
            await DatabaseHelper.Instance.InitAsync();
        }

        // Вызывается из TaskListFragment
        public void OpenDrawer() => _drawerLayout.OpenDrawer(_navigationView);

        private void ShowHome()
        {
            _homeFragment ??= new TaskListFragment();
            ShowFragment(_homeFragment);
        }

        private void ShowHistory()
        {
            _historyFragment ??= new HistoryFragment();
            ShowFragment(_historyFragment);
        }

        private void ShowSettings()
        {
            _settingsFragment ??= new SettingsFragment();
            ShowFragment(_settingsFragment);
        }

        private void ShowFragment(AndroidX.Fragment.App.Fragment fragment)
        {
            SupportFragmentManager.BeginTransaction()
                .Replace(Resource.Id.fragmentContainer, fragment)
                .Commit();
        }

        public override void OnBackPressed()
        {
            if (_drawerLayout.IsDrawerOpen(_navigationView))
                _drawerLayout.CloseDrawer(_navigationView);
            else
                base.OnBackPressed();
        }

        private class BottomNavListener : Java.Lang.Object,
            Google.Android.Material.Navigation.NavigationBarView.IOnItemSelectedListener
        {
            private readonly Func<int, bool> _callback;
            public BottomNavListener(Func<int, bool> callback) => _callback = callback;
            public bool OnNavigationItemSelected(Android.Views.IMenuItem item)
                => _callback(item.ItemId);
        }

        private class DrawerNavListener : Java.Lang.Object,
            Google.Android.Material.Navigation.NavigationView.IOnNavigationItemSelectedListener
        {
            private readonly Func<int, bool> _callback;
            public DrawerNavListener(Func<int, bool> callback) => _callback = callback;
            public bool OnNavigationItemSelected(Android.Views.IMenuItem item)
                => _callback(item.ItemId);
        }
    }
}
