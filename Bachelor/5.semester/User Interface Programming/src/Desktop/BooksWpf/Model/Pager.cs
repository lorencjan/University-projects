using System.Collections.ObjectModel;
using BooksWpf.ViewModels;

namespace BooksWpf.Model
{
    public class Pager : ViewModelBase
    {
        private int _page = 1;
        public int Page
        {
            get => _page;
            set => SetProperty(ref _page, value);
        }

        private ObservableCollection<int> _options = new ObservableCollection<int>{ 1 };
        public ObservableCollection<int> Options
        {
            get => _options;
            set => SetProperty(ref _options, value);
        }

        public int PageSize => 10;
    }
}
