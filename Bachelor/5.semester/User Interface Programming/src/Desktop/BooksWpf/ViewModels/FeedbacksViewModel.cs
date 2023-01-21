using BooksService.Client;
using BooksWpf.Commands;
using BooksWpf.DAL.Services;
using BooksWpf.Model;
using BooksWpf.Resources.Img;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace BooksWpf.ViewModels
{
     public class FeedbacksViewModel : ViewModelBase
     {
        private readonly UsersService _feedbackService;

        private ObservableCollection<Feedback> _feedbacks;
        public ObservableCollection<Feedback> Feedbacks
        {
            get => _feedbacks;
            set => SetProperty(ref _feedbacks, value);
        }

        #region Commands
        public IAsyncCommand LoadFeedbacksCommand { get; set; }
        public ICommand DeleteFeedbackCommand { get; set; }
        #endregion

        #region Visibilities
        public Visibility _listVisible;
        public Visibility ListVisible
        {
            get { return _listVisible; }
            set { SetProperty(ref _listVisible, value); }
        }
        #endregion

        #region Paging
        private Pager _pager;
        public Pager Pager
        {
            get => _pager;
            set => SetProperty(ref _pager, value);
        }

        public byte[] PageLeftIcon { get; set; } = Images.ChevronLeft___green;
        public byte[] PageRightIcon { get; set; } = Images.ChevronRight___green;

        public AsyncCommand PageLeftCommand { get; set; }
        public AsyncCommand PageRightCommand { get; set; }
        #endregion

        public byte[] DeleteIcon { get; } = Images.TrashBin;

        public FeedbacksViewModel(UsersService feedbackService)
        {
            _feedbackService = feedbackService;
            Pager = new Pager();
            LoadFeedbacksCommand = new AsyncCommand(async () => await LoadFeedbacks());
            PageLeftCommand = new AsyncCommand(async () => await MovePage(true), () => Pager.Page != Pager.Options.First());
            PageRightCommand = new AsyncCommand(async () => await MovePage(false), () => Pager.Page != Pager.Options.Last());
            DeleteFeedbackCommand = new AsyncCommand<int>(async id => await DeleteFeedback(id));
            ListVisible = Visibility.Visible;
        }

        public async Task LoadFeedbacks()
        {
            var feedbacks = await _feedbackService.GetFeedbacks();
            Pager.Options = new ObservableCollection<int>(Enumerable.Range(1, feedbacks.Count / 10 + 1).ToList());
            if (Pager.Options.Last() < Pager.Page)
            {
                Pager.Page = Pager.Options.Last();
            }
            PageLeftCommand.RaiseCanExecuteChanged();
            PageRightCommand.RaiseCanExecuteChanged();
            var paged = feedbacks.Skip(Pager.PageSize * (Pager.Page - 1)).Take(Pager.PageSize).ToList();

            Feedbacks = new ObservableCollection<Feedback>(paged);
        }

        public async Task MovePage(bool left)
        {
            Pager.Page += left ? -1 : 1;
            await LoadFeedbacks();
        }

        public async Task DeleteFeedback(int id)
        {
            await _feedbackService.DeleteFeedback(id);
            Feedbacks = await _feedbackService.GetFeedbacks();
        }
    }
}
