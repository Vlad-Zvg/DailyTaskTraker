using Android.Content;
using Android.Views;
using Android.Widget;
using AndroidX.AppCompat.App;
using AndroidX.Fragment.App;
using Fragment = AndroidX.Fragment.App.Fragment;
using Google.Android.Material.SwitchMaterial;

namespace DailyTaskTraker.Views
{
    public class SettingsFragment : Fragment
    {
        private const string PrefsName = "app_settings";
        private const string KeyDarkMode = "dark_mode";

        private SwitchMaterial _switchDarkTheme = null!;
        private SwitchMaterial _switchNotifications = null!;

        public override View? OnCreateView(LayoutInflater inflater, ViewGroup? container, Bundle? savedInstanceState)
            => inflater.Inflate(Resource.Layout.fragment_settings, container, false);

        public override void OnViewCreated(View view, Bundle? savedInstanceState)
        {
            base.OnViewCreated(view, savedInstanceState);

            _switchDarkTheme = view.FindViewById<SwitchMaterial>(Resource.Id.switchDarkTheme)!;
            _switchNotifications = view.FindViewById<SwitchMaterial>(Resource.Id.switchNotifications)!;

            // Показываем текущее состояние темы
            var prefs = Activity?.GetSharedPreferences(PrefsName, FileCreationMode.Private);
            _switchDarkTheme.Checked = prefs?.GetBoolean(KeyDarkMode, false) ?? false;

            // Весь ряд кликабелен
            view.FindViewById(Resource.Id.itemTheme)!.Click += (_, _) =>
                _switchDarkTheme.Checked = !_switchDarkTheme.Checked;

            view.FindViewById(Resource.Id.itemNotifications)!.Click += (_, _) =>
                _switchNotifications.Checked = !_switchNotifications.Checked;

            // Переключение темы: сохраняем и перезапускаем Activity
            _switchDarkTheme.CheckedChange += (_, e) =>
            {
                var editor = Activity?.GetSharedPreferences(PrefsName, FileCreationMode.Private)?.Edit();
                editor?.PutBoolean(KeyDarkMode, e.IsChecked);
                editor?.Apply();

                // Применяем режим через делегат активности
                (Activity as AppCompatActivity)?.Delegate?.SetLocalNightMode(
                    e.IsChecked ? AppCompatDelegate.ModeNightYes : AppCompatDelegate.ModeNightNo);

                Activity?.Recreate();
            };
        }
    }
}
