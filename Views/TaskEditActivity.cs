using Android.App;
using Android.Widget;
using AndroidX.AppCompat.App;
using DailyTaskTraker.Models;
using DailyTaskTraker.Services;
using DailyTaskTraker.ViewModels;

namespace DailyTaskTraker.Views
{
    [Activity(Label = "Задача", Theme = "@style/AppTheme")]
    public class TaskEditActivity : AppCompatActivity
    {
        public const string ExtraTaskId = "task_id";
        public const string ExtraTabId = "tab_id";

        private readonly TaskEditViewModel _viewModel = new();
        private bool _isEditMode;
        private DateTime? _reminderDate;

        // Views
        private Google.Android.Material.TextField.TextInputEditText _editTitle = null!;
        private Spinner _spinnerCategory = null!;
        private CheckBox _checkboxRecurring = null!;
        private Android.Views.View _layoutRecurrence = null!;
        private RadioButton _radioDaily = null!, _radioWeekly = null!, _radioSpecificDays = null!;
        private Android.Views.View _layoutDays = null!;
        private CheckBox _checkMon = null!, _checkTue = null!, _checkWed = null!,
                         _checkThu = null!, _checkFri = null!, _checkSat = null!, _checkSun = null!;
        private CheckBox _checkboxReminder = null!;
        private Android.Views.View _layoutReminder = null!;
        private TextView _textReminderValue = null!;

        protected override void OnCreate(Bundle? savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.activity_task_edit);
            SupportActionBar?.SetDisplayHomeAsUpEnabled(true);

            BindViews();
            SetupListeners();

            int taskId = Intent?.GetIntExtra(ExtraTaskId, 0) ?? 0;
            int tabId = Intent?.GetIntExtra(ExtraTabId, 0) ?? 0;
            _isEditMode = taskId > 0;

            _ = InitAsync(taskId, tabId);
        }

        private void BindViews()
        {
            _editTitle = FindViewById<Google.Android.Material.TextField.TextInputEditText>(Resource.Id.editTitle)!;
            _spinnerCategory = FindViewById<Spinner>(Resource.Id.spinnerCategory)!;
            _checkboxRecurring = FindViewById<CheckBox>(Resource.Id.checkboxRecurring)!;
            _layoutRecurrence = FindViewById<Android.Views.View>(Resource.Id.layoutRecurrence)!;
            _radioDaily = FindViewById<RadioButton>(Resource.Id.radioDaily)!;
            _radioWeekly = FindViewById<RadioButton>(Resource.Id.radioWeekly)!;
            _radioSpecificDays = FindViewById<RadioButton>(Resource.Id.radioSpecificDays)!;
            _layoutDays = FindViewById<Android.Views.View>(Resource.Id.layoutDays)!;
            _checkMon = FindViewById<CheckBox>(Resource.Id.checkMon)!;
            _checkTue = FindViewById<CheckBox>(Resource.Id.checkTue)!;
            _checkWed = FindViewById<CheckBox>(Resource.Id.checkWed)!;
            _checkThu = FindViewById<CheckBox>(Resource.Id.checkThu)!;
            _checkFri = FindViewById<CheckBox>(Resource.Id.checkFri)!;
            _checkSat = FindViewById<CheckBox>(Resource.Id.checkSat)!;
            _checkSun = FindViewById<CheckBox>(Resource.Id.checkSun)!;
            _checkboxReminder = FindViewById<CheckBox>(Resource.Id.checkboxReminder)!;
            _layoutReminder = FindViewById<Android.Views.View>(Resource.Id.layoutReminder)!;
            _textReminderValue = FindViewById<TextView>(Resource.Id.textReminderValue)!;
        }

        private void SetupListeners()
        {
            _checkboxRecurring.CheckedChange += (_, e) =>
                _layoutRecurrence.Visibility = e.IsChecked
                    ? Android.Views.ViewStates.Visible
                    : Android.Views.ViewStates.Gone;

            _radioSpecificDays.CheckedChange += (_, e) =>
                _layoutDays.Visibility = e.IsChecked
                    ? Android.Views.ViewStates.Visible
                    : Android.Views.ViewStates.Gone;

            _checkboxReminder.CheckedChange += (_, e) =>
                _layoutReminder.Visibility = e.IsChecked
                    ? Android.Views.ViewStates.Visible
                    : Android.Views.ViewStates.Gone;

            FindViewById<Button>(Resource.Id.btnPickDate)!.Click += (_, _) => ShowDatePicker();
            FindViewById<Button>(Resource.Id.btnPickTime)!.Click += (_, _) => ShowTimePicker();
            FindViewById<Google.Android.Material.Button.MaterialButton>(Resource.Id.btnSave)!.Click
                += async (_, _) => await SaveAsync();
        }

        private async Task InitAsync(int taskId, int tabId)
        {
            if (_isEditMode)
                await _viewModel.InitEditAsync(taskId);
            else
                await _viewModel.InitNewAsync(tabId);

            RunOnUiThread(PopulateUi);
        }

        private void PopulateUi()
        {
            var names = _viewModel.Categories.Select(c => c.Name).ToArray();
            var adapter = new ArrayAdapter<string>(this, Android.Resource.Layout.SimpleSpinnerItem, names);
            adapter.SetDropDownViewResource(Android.Resource.Layout.SimpleSpinnerDropDownItem);
            _spinnerCategory.Adapter = adapter;

            if (!_isEditMode) return;

            _editTitle.Text = _viewModel.Task.Title;

            var idx = _viewModel.Categories.FindIndex(c => c.Id == _viewModel.Task.CategoryId);
            if (idx >= 0) _spinnerCategory.SetSelection(idx);

            if (_viewModel.Recurrence != null)
            {
                _checkboxRecurring.Checked = true;
                _layoutRecurrence.Visibility = Android.Views.ViewStates.Visible;

                switch (_viewModel.Recurrence.Rule)
                {
                    case RecurrenceRule.Daily:   _radioDaily.Checked = true; break;
                    case RecurrenceRule.Weekly:  _radioWeekly.Checked = true; break;
                    case RecurrenceRule.SpecificDays:
                        _radioSpecificDays.Checked = true;
                        _layoutDays.Visibility = Android.Views.ViewStates.Visible;
                        int mask = _viewModel.Recurrence.DaysMask;
                        _checkMon.Checked = (mask & 1)  != 0;
                        _checkTue.Checked = (mask & 2)  != 0;
                        _checkWed.Checked = (mask & 4)  != 0;
                        _checkThu.Checked = (mask & 8)  != 0;
                        _checkFri.Checked = (mask & 16) != 0;
                        _checkSat.Checked = (mask & 32) != 0;
                        _checkSun.Checked = (mask & 64) != 0;
                        break;
                }
            }

            if (_viewModel.Reminder != null)
            {
                _checkboxReminder.Checked = true;
                _layoutReminder.Visibility = Android.Views.ViewStates.Visible;
                _reminderDate = _viewModel.Reminder.RemindAt;
                UpdateReminderText();
            }
        }

        private async Task SaveAsync()
        {
            _viewModel.Task.Title = _editTitle.Text?.Trim() ?? string.Empty;

            int catIdx = _spinnerCategory.SelectedItemPosition;
            if (catIdx >= 0 && catIdx < _viewModel.Categories.Count)
                _viewModel.Task.CategoryId = _viewModel.Categories[catIdx].Id;

            if (_checkboxRecurring.Checked)
            {
                _viewModel.Recurrence ??= new Recurrence();
                _viewModel.Task.IsRecurring = true;

                if (_radioDaily.Checked)
                    _viewModel.Recurrence.Rule = RecurrenceRule.Daily;
                else if (_radioWeekly.Checked)
                    _viewModel.Recurrence.Rule = RecurrenceRule.Weekly;
                else if (_radioSpecificDays.Checked)
                {
                    _viewModel.Recurrence.Rule = RecurrenceRule.SpecificDays;
                    _viewModel.Recurrence.DaysMask =
                        (_checkMon.Checked ? 1  : 0) |
                        (_checkTue.Checked ? 2  : 0) |
                        (_checkWed.Checked ? 4  : 0) |
                        (_checkThu.Checked ? 8  : 0) |
                        (_checkFri.Checked ? 16 : 0) |
                        (_checkSat.Checked ? 32 : 0) |
                        (_checkSun.Checked ? 64 : 0);
                }
            }
            else
            {
                _viewModel.Recurrence = null;
                _viewModel.Task.IsRecurring = false;
            }

            if (_checkboxReminder.Checked && _reminderDate.HasValue)
            {
                _viewModel.Reminder ??= new Reminder();
                _viewModel.Reminder.RemindAt = _reminderDate.Value;
                _viewModel.Reminder.IsRecurring = _viewModel.Task.IsRecurring;
            }
            else
            {
                _viewModel.Reminder = null;
            }

            bool saved = await _viewModel.SaveAsync();
            if (saved)
            {
                // Планируем или отменяем будильник после сохранения задачи
                if (_viewModel.Reminder != null)
                    ReminderScheduler.Schedule(this, _viewModel.Task, _viewModel.Reminder, _viewModel.Recurrence);
                else
                    ReminderScheduler.Cancel(this, _viewModel.Task.Id);

                Finish();
            }
            else
            {
                Toast.MakeText(this, "Введите название задачи", ToastLength.Short)?.Show();
            }
        }

        private void ShowDatePicker()
        {
            var now = _reminderDate ?? DateTime.Now;
            new DatePickerDialog(this, (_, e) =>
            {
                _reminderDate = new DateTime(
                    e.Year, e.Month + 1, e.DayOfMonth,
                    _reminderDate?.Hour ?? 9,
                    _reminderDate?.Minute ?? 0, 0);
                UpdateReminderText();
            }, now.Year, now.Month - 1, now.Day).Show();
        }

        private void ShowTimePicker()
        {
            var now = _reminderDate ?? DateTime.Now;
            new TimePickerDialog(this, (_, e) =>
            {
                _reminderDate = new DateTime(
                    _reminderDate?.Year  ?? now.Year,
                    _reminderDate?.Month ?? now.Month,
                    _reminderDate?.Day   ?? now.Day,
                    e.HourOfDay, e.Minute, 0);
                UpdateReminderText();
            }, now.Hour, now.Minute, true).Show();
        }

        private void UpdateReminderText()
        {
            if (!_reminderDate.HasValue) return;
            _textReminderValue.Text = _reminderDate.Value.ToString("dd.MM.yyyy HH:mm");
            _textReminderValue.Visibility = Android.Views.ViewStates.Visible;
        }

        public override bool OnSupportNavigateUp()
        {
            Finish();
            return true;
        }
    }
}
