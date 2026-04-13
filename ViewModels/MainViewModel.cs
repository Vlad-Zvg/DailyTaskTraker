using DailyTaskTraker.Data;
using DailyTaskTraker.Models;

namespace DailyTaskTraker.ViewModels
{
    // Управляет списком вкладок и текущей активной вкладкой
    public class MainViewModel : BaseViewModel
    {
        private readonly TabRepository _tabRepo = new();

        private List<Tab> _tabs = new();
        private Tab? _selectedTab;

        public List<Tab> Tabs
        {
            get => _tabs;
            private set => SetProperty(ref _tabs, value);
        }

        public Tab? SelectedTab
        {
            get => _selectedTab;
            set => SetProperty(ref _selectedTab, value);
        }

        public async Task LoadTabsAsync()
        {
            IsBusy = true;
            Tabs = await _tabRepo.GetAllAsync();
            SelectedTab ??= Tabs.FirstOrDefault();
            IsBusy = false;
        }

        public async Task AddTabAsync(string name)
        {
            var tab = new Tab
            {
                Name = name,
                Type = TabType.Custom,
                IsSystem = false,
                // Вставляем перед History (SortOrder = 999)
                SortOrder = Tabs.Where(t => !t.IsSystem || t.Type != TabType.History)
                                .Select(t => t.SortOrder)
                                .DefaultIfEmpty(0)
                                .Max() + 1
            };
            await _tabRepo.InsertAsync(tab);
            await LoadTabsAsync();
        }

        public async Task<bool> DeleteTabAsync(Tab tab)
        {
            bool deleted = await _tabRepo.DeleteAsync(tab);
            if (deleted) await LoadTabsAsync();
            return deleted;
        }
    }
}
