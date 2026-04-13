using Android.Content;
using Android.Views;
using AndroidX.Fragment.App;
using Fragment = AndroidX.Fragment.App.Fragment;
using AndroidX.RecyclerView.Widget;
using DailyTaskTraker.Adapters;
using DailyTaskTraker.ViewModels;

namespace DailyTaskTraker.Views
{
    public class TaskListFragment : Fragment
    {
        private const string ArgTabId = "tab_id";

        private int _tabId;
        private TaskListViewModel _viewModel = new();
        private TaskAdapter _adapter = null!;

        public static TaskListFragment NewInstance(int tabId)
        {
            var fragment = new TaskListFragment();
            var args = new Bundle();
            args.PutInt(ArgTabId, tabId);
            fragment.Arguments = args;
            return fragment;
        }

        public override void OnCreate(Bundle? savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            _tabId = Arguments?.GetInt(ArgTabId) ?? 0;
        }

        public override View? OnCreateView(LayoutInflater inflater, ViewGroup? container, Bundle? savedInstanceState)
            => inflater.Inflate(Resource.Layout.fragment_task_list, container, false);

        public override void OnViewCreated(View view, Bundle? savedInstanceState)
        {
            base.OnViewCreated(view, savedInstanceState);

            var recycler = view.FindViewById<RecyclerView>(Resource.Id.recyclerTasks)!;
            recycler.SetLayoutManager(new LinearLayoutManager(Context));

            _adapter = new TaskAdapter(
                onToggle: async task =>
                {
                    if (task.IsDone) await _viewModel.MarkUndoneAsync(task);
                    else await _viewModel.MarkDoneAsync(task);
                    RefreshList();
                },
                onDelete: async task =>
                {
                    await _viewModel.DeleteAsync(task);
                    RefreshList();
                },
                onEdit: task =>
                {
                    var intent = new Intent(Context, typeof(TaskEditActivity));
                    intent.PutExtra(TaskEditActivity.ExtraTaskId, task.Id);
                    StartActivity(intent);
                }
            );
            recycler.SetAdapter(_adapter);

            view.FindViewById(Resource.Id.fabAddTask)!.Click += (_, _) =>
            {
                var intent = new Intent(Context, typeof(TaskEditActivity));
                intent.PutExtra(TaskEditActivity.ExtraTabId, _tabId);
                StartActivity(intent);
            };
        }

        public override void OnResume()
        {
            base.OnResume();
            _ = RefreshAsync();
        }

        public async System.Threading.Tasks.Task RefreshAsync()
        {
            await _viewModel.LoadAsync(_tabId);
            RefreshList();
        }

        private void RefreshList()
            => Activity?.RunOnUiThread(() => _adapter.UpdateItems(_viewModel.Tasks));
    }
}
