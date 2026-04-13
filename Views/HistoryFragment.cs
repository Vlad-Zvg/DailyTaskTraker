using Android.Views;
using AndroidX.Fragment.App;
using Fragment = AndroidX.Fragment.App.Fragment;
using AndroidX.RecyclerView.Widget;
using DailyTaskTraker.Adapters;
using DailyTaskTraker.ViewModels;

namespace DailyTaskTraker.Views
{
    public class HistoryFragment : Fragment
    {
        private HistoryViewModel _viewModel = new();
        private HistoryAdapter _adapter = null!;

        public override View? OnCreateView(LayoutInflater inflater, ViewGroup? container, Bundle? savedInstanceState)
            => inflater.Inflate(Resource.Layout.fragment_history, container, false);

        public override void OnViewCreated(View view, Bundle? savedInstanceState)
        {
            base.OnViewCreated(view, savedInstanceState);

            var recycler = view.FindViewById<RecyclerView>(Resource.Id.recyclerHistory)!;
            recycler.SetLayoutManager(new LinearLayoutManager(Context));

            _adapter = new HistoryAdapter();
            recycler.SetAdapter(_adapter);
        }

        public override void OnResume()
        {
            base.OnResume();
            _ = RefreshAsync();
        }

        public async System.Threading.Tasks.Task RefreshAsync()
        {
            await _viewModel.LoadAsync();
            Activity?.RunOnUiThread(() => _adapter.UpdateItems(_viewModel.Logs));
        }
    }
}
