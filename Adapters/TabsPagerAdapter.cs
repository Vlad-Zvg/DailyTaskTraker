using AndroidX.Fragment.App;
using AndroidX.ViewPager2.Adapter;
using Fragment = AndroidX.Fragment.App.Fragment;
using DailyTaskTraker.Models;
using DailyTaskTraker.Views;

namespace DailyTaskTraker.Adapters
{
    public class TabsPagerAdapter : FragmentStateAdapter
    {
        private readonly List<Tab> _tabs;

        public TabsPagerAdapter(FragmentActivity activity, List<Tab> tabs)
            : base(activity)
        {
            _tabs = tabs;
        }

        public override int ItemCount => _tabs.Count;

        public override Fragment CreateFragment(int position)
        {
            var tab = _tabs[position];

            if (tab.Type == TabType.History)
                return new HistoryFragment();

            return TaskListFragment.NewInstance(tab.Id);
        }
    }
}
