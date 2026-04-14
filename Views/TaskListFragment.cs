using Android.Content;
using Android.Graphics;
using Android.Views;
using Android.Widget;
using AndroidX.Fragment.App;
using Fragment = AndroidX.Fragment.App.Fragment;
using AndroidX.RecyclerView.Widget;
using DailyTaskTraker.Adapters;
using DailyTaskTraker.ViewModels;
using Google.Android.Material.Chip;
using System.Globalization;

namespace DailyTaskTraker.Views
{
    public class TaskListFragment : Fragment
    {
        private readonly MainViewModel _mainViewModel = new();
        private readonly TaskListViewModel _taskViewModel = new();
        private TaskAdapter _adapter = null!;
        private int _selectedTabId = -1;

        // Views
        private TextView _textGreeting = null!;
        private TextView _textDate = null!;
        private TextView _textProgressCount = null!;
        private ProgressBar _progressBar = null!;
        private ChipGroup _chipGroup = null!;
        private RecyclerView _recycler = null!;

        public override View? OnCreateView(LayoutInflater inflater, ViewGroup? container, Bundle? savedInstanceState)
            => inflater.Inflate(Resource.Layout.fragment_task_list, container, false);

        public override void OnViewCreated(View view, Bundle? savedInstanceState)
        {
            base.OnViewCreated(view, savedInstanceState);

            _textGreeting = view.FindViewById<TextView>(Resource.Id.textGreeting)!;
            _textDate = view.FindViewById<TextView>(Resource.Id.textDate)!;
            _textProgressCount = view.FindViewById<TextView>(Resource.Id.textProgressCount)!;
            _progressBar = view.FindViewById<ProgressBar>(Resource.Id.progressBar)!;
            _chipGroup = view.FindViewById<ChipGroup>(Resource.Id.chipGroupTabs)!;
            _recycler = view.FindViewById<RecyclerView>(Resource.Id.recyclerTasks)!;
            _recycler.SetLayoutManager(new LinearLayoutManager(Context));

            // Открытие бокового меню
            view.FindViewById(Resource.Id.btnMenu)!.Click += (_, _) =>
                (Activity as MainActivity)?.OpenDrawer();

            // Адаптер списка задач
            _adapter = new TaskAdapter(
                onToggle: async task =>
                {
                    if (task.IsDone) await _taskViewModel.MarkUndoneAsync(task);
                    else await _taskViewModel.MarkDoneAsync(task);
                    RefreshList();
                },
                onDelete: async task =>
                {
                    await _taskViewModel.DeleteAsync(task);
                    RefreshList();
                },
                onEdit: task =>
                {
                    var intent = new Intent(Context, typeof(TaskEditActivity));
                    intent.PutExtra(TaskEditActivity.ExtraTaskId, task.Id);
                    StartActivity(intent);
                }
            );
            _recycler.SetAdapter(_adapter);

            // FAB — добавить задачу
            view.FindViewById(Resource.Id.fabAddTask)!.Click += (_, _) =>
            {
                if (_selectedTabId < 0) return;
                var intent = new Intent(Context, typeof(TaskEditActivity));
                intent.PutExtra(TaskEditActivity.ExtraTabId, _selectedTabId);
                StartActivity(intent);
            };

            SetupGreeting();
        }

        private void SetupGreeting()
        {
            var hour = DateTime.Now.Hour;
            _textGreeting.Text = hour < 12 ? "Доброе утро!" :
                                 hour < 18 ? "Добрый день!" :
                                             "Добрый вечер!";

            var culture = new CultureInfo("ru-RU");
            _textDate.Text = DateTime.Now.ToString("dddd, d MMMM", culture);
        }

        public override void OnResume()
        {
            base.OnResume();
            SetupGreeting();
            _ = RefreshAsync();
        }

        private async System.Threading.Tasks.Task RefreshAsync()
        {
            await _mainViewModel.LoadTabsAsync();

            // Только не-History вкладки для фильтра
            var taskTabs = _mainViewModel.Tabs
                .Where(t => t.Type != Models.TabType.History)
                .ToList();

            // Если текущая вкладка ещё не выбрана — выбираем первую
            if (_selectedTabId < 0 && taskTabs.Count > 0)
                _selectedTabId = taskTabs[0].Id;

            Activity?.RunOnUiThread(() => RebuildChips(taskTabs));

            await LoadTasksAsync();
        }

        private void RebuildChips(List<Models.Tab> tabs)
        {
            _chipGroup.RemoveAllViews();

            foreach (var tab in tabs)
            {
                var chip = new Chip(Context);
                chip.Text = tab.Name;
                chip.Checkable = true;
                chip.Checked = tab.Id == _selectedTabId;

                int capturedTabId = tab.Id;
                chip.Click += (_, _) =>
                {
                    if (_selectedTabId == capturedTabId) return;
                    _selectedTabId = capturedTabId;
                    _ = LoadTasksAsync();
                };

                _chipGroup.AddView(chip);
            }
        }

        private async System.Threading.Tasks.Task LoadTasksAsync()
        {
            if (_selectedTabId < 0) return;
            await _taskViewModel.LoadAsync(_selectedTabId);
            RefreshList();
        }

        private void RefreshList()
        {
            Activity?.RunOnUiThread(() =>
            {
                _adapter.UpdateItems(_taskViewModel.Tasks);
                UpdateProgress();
            });
        }

        private void UpdateProgress()
        {
            var total = _taskViewModel.Tasks.Count;
            var done = _taskViewModel.Tasks.Count(t => t.IsDone);
            _textProgressCount.Text = $"{done} из {total}";
            _progressBar.Progress = total > 0 ? (int)(done * 100.0 / total) : 0;
        }
    }
}
