using Android.Graphics;
using Android.Views;
using Android.Widget;
using AndroidX.RecyclerView.Widget;
using DailyTaskTraker.Models;

namespace DailyTaskTraker.Adapters
{
    public class TaskAdapter : RecyclerView.Adapter
    {
        private List<TaskItem> _items = new();
        private readonly Func<TaskItem, Task> _onToggle;
        private readonly Func<TaskItem, Task> _onDelete;
        private readonly Action<TaskItem> _onEdit;

        public TaskAdapter(
            Func<TaskItem, Task> onToggle,
            Func<TaskItem, Task> onDelete,
            Action<TaskItem> onEdit)
        {
            _onToggle = onToggle;
            _onDelete = onDelete;
            _onEdit = onEdit;
        }

        public void UpdateItems(List<TaskItem> items)
        {
            _items = items;
            NotifyDataSetChanged();
        }

        public override int ItemCount => _items.Count;

        public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
        {
            var view = LayoutInflater.From(parent.Context)!
                .Inflate(Resource.Layout.item_task, parent, false)!;
            return new TaskViewHolder(view);
        }

        public override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
        {
            var vh = (TaskViewHolder)holder;
            var task = _items[position];

            vh.TextTitle.Text = task.Title;
            vh.TextCategory.Text = task.Category?.Name ?? string.Empty;
            vh.TextCategory.Visibility = string.IsNullOrEmpty(task.Category?.Name)
                ? ViewStates.Gone
                : ViewStates.Visible;

            // Зачёркивание и прозрачность для выполненных задач
            vh.TextTitle.PaintFlags = task.IsDone
                ? vh.TextTitle.PaintFlags | PaintFlags.StrikeThruText
                : vh.TextTitle.PaintFlags & ~PaintFlags.StrikeThruText;
            vh.TextTitle.Alpha = task.IsDone ? 0.5f : 1.0f;

            // Отключаем слушатель перед программным изменением
            vh.Checkbox.SetOnCheckedChangeListener(null);
            vh.Checkbox.Checked = task.IsDone;
            vh.Checkbox.SetOnCheckedChangeListener(new CheckedChangeListener(task, _onToggle));

            vh.ItemView.SetOnClickListener(new ClickListener(() => _onEdit(task)));
            vh.BtnDelete.SetOnClickListener(new ClickListener(() => _ = _onDelete(task)));
        }

        private class TaskViewHolder : RecyclerView.ViewHolder
        {
            public CheckBox Checkbox { get; }
            public TextView TextTitle { get; }
            public TextView TextCategory { get; }
            public ImageButton BtnDelete { get; }

            public TaskViewHolder(View view) : base(view)
            {
                Checkbox = view.FindViewById<CheckBox>(Resource.Id.checkboxDone)!;
                TextTitle = view.FindViewById<TextView>(Resource.Id.textTitle)!;
                TextCategory = view.FindViewById<TextView>(Resource.Id.textCategory)!;
                BtnDelete = view.FindViewById<ImageButton>(Resource.Id.btnDelete)!;
            }
        }

        private class CheckedChangeListener : Java.Lang.Object, CompoundButton.IOnCheckedChangeListener
        {
            private readonly TaskItem _task;
            private readonly Func<TaskItem, Task> _onToggle;

            public CheckedChangeListener(TaskItem task, Func<TaskItem, Task> onToggle)
            {
                _task = task;
                _onToggle = onToggle;
            }

            public void OnCheckedChanged(CompoundButton? buttonView, bool isChecked)
                => _ = _onToggle(_task);
        }

        private class ClickListener : Java.Lang.Object, View.IOnClickListener
        {
            private readonly Action _action;
            public ClickListener(Action action) => _action = action;
            public void OnClick(View? v) => _action();
        }
    }
}
