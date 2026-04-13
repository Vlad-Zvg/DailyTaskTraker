using Android.Views;
using Android.Widget;
using AndroidX.RecyclerView.Widget;
using DailyTaskTraker.Models;

namespace DailyTaskTraker.Adapters
{
    public class HistoryAdapter : RecyclerView.Adapter
    {
        private List<CompletionLog> _items = new();

        public void UpdateItems(List<CompletionLog> items)
        {
            _items = items;
            NotifyDataSetChanged();
        }

        public override int ItemCount => _items.Count;

        public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
        {
            var view = LayoutInflater.From(parent.Context)!
                .Inflate(Resource.Layout.item_history, parent, false)!;
            return new HistoryViewHolder(view);
        }

        public override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
        {
            var vh = (HistoryViewHolder)holder;
            var log = _items[position];

            vh.TextTaskTitle.Text = log.Task?.Title ?? $"Задача #{log.TaskId}";
            vh.TextCompletedAt.Text = log.CompletedAt.ToString("dd.MM.yyyy HH:mm");
        }

        private class HistoryViewHolder : RecyclerView.ViewHolder
        {
            public TextView TextTaskTitle { get; }
            public TextView TextCompletedAt { get; }

            public HistoryViewHolder(View view) : base(view)
            {
                TextTaskTitle = view.FindViewById<TextView>(Resource.Id.textTaskTitle)!;
                TextCompletedAt = view.FindViewById<TextView>(Resource.Id.textCompletedAt)!;
            }
        }
    }
}
