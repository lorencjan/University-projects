using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using MovieDatabase.App.Resources.Img;
using MovieDatabase.App.Commands;
using MovieDatabase.BL.Model;
using MovieDatabase.BL.Repositories;

namespace MovieDatabase.App.ViewModels
{
    public class PeopleViewModel : ViewModelBase
    {
        #region Attributes
        private bool _edit;
        private readonly PersonRepository _personRepository;
        public NewPersonViewModel NewPersonViewModel { get; set; }
        private ObservableCollection<PersonListDto> _listPeople;
        private PersonDto _detailPerson;
        private Visibility _listVisible, _detailVisible, _playedInVisible, _directedVisible, _newPersonVisible, _readPersonVisible, _editPersonVisible;
        private byte[] _editIcon;
        public byte[] NewIcon{set;get;}
        public byte[] EditIcon
        {
            get { return _editIcon; }
            set { SetProperty(ref _editIcon, value); }
        }
        public byte[] DeleteIcon{ get;set;}
        public byte[] ResetIcon{ get;set;}
        public byte[] AddImage{ get;set;}
        public Visibility ListVisible
        {
            get { return _listVisible; }
            set { SetProperty(ref _listVisible, value); }
        }
        public Visibility DetailVisible
        {
            get { return _detailVisible; }
            set { SetProperty(ref _detailVisible, value); }
        }
        public Visibility PlayedInVisible
        {
            get { return _playedInVisible; }
            set { SetProperty(ref _playedInVisible, value); }
        }
        public Visibility DirectedVisible
        {
            get { return _directedVisible; }
            set { SetProperty(ref _directedVisible, value); }
        }
        public Visibility NewPersonVisible
        {
            get { return _newPersonVisible; }
            set { SetProperty(ref _newPersonVisible, value); }
        }
        public Visibility ReadPersonVisible
        {
            get { return _readPersonVisible; }
            set { SetProperty(ref _readPersonVisible, value); }
        }
        public Visibility EditPersonVisible
        {
            get { return _editPersonVisible; }
            set { SetProperty(ref _editPersonVisible, value); }
        }

        public ObservableCollection<PersonListDto> ListPeople
        {
            get { return _listPeople; }
            set { SetProperty(ref _listPeople, value); }
        }
        public PersonDto DetailPerson
        {
            get { return _detailPerson; }
            set { SetProperty(ref _detailPerson, value); }
        }
        public ICommand SelectPersonCommand { get; set; }
        public ICommand NewPersonCommand { get; set; }
        public ICommand DeletePersonCommand { get; set; }
        public ICommand ToggleEditPersonCommand { get; set; }
        public ICommand Save { get; set; }
        #endregion

        public PeopleViewModel(PersonRepository personRepository, NewPersonViewModel newPersonViewModel)
        {
            ShowList();
            _personRepository = personRepository;
            NewPersonViewModel = newPersonViewModel;
            NewIcon = Images.New;
            EditIcon = Images.Edit;
            DeleteIcon = Images.Delete;
            PlayedInVisible = Visibility.Visible;
            DirectedVisible = Visibility.Visible;
            SelectPersonCommand = new RelayCommand<Guid>(SelectPerson);
            NewPersonCommand = new RelayCommand(NewPerson);
            DeletePersonCommand = new RelayCommand(DeletePerson);
            ToggleEditPersonCommand = new RelayCommand(ToggleEditPerson);
            Save = new RelayCommand<Guid>(SaveNewOrEdit);
            _edit = false;
        }
        
        private void CollapseAll()
        {
            ListVisible = Visibility.Collapsed;
            DetailVisible = Visibility.Collapsed;
            NewPersonVisible = Visibility.Collapsed;
            EditPersonVisible = Visibility.Collapsed;
        }

        private void ShowList()
        {
            CollapseAll();
            ListVisible = Visibility.Visible;
        }

        private void ShowDetail()
        {
            CollapseAll();
            ReadPersonVisible = Visibility.Visible;
            DetailVisible = Visibility.Visible;
            EditIcon = Images.Edit;
        }

        private void NewPerson()
        {
            CollapseAll();
            NewPersonVisible = Visibility.Visible;
            NewPersonViewModel.InitializeValues(new PersonDto());
            NewPersonViewModel.NewVisible = Visibility.Visible;
        }

        private void ToggleEditPerson()
        {
            ReadPersonVisible = EditPersonVisible == Visibility.Visible ? Visibility.Visible : Visibility.Collapsed;
            EditPersonVisible = EditPersonVisible == Visibility.Visible ? Visibility.Collapsed : Visibility.Visible;
            NewPersonViewModel.InitializeValues(DetailPerson, "Original Photo");
            _edit=true;
            NewPersonViewModel.NewVisible = Visibility.Collapsed;
            EditIcon = EditIcon.SequenceEqual(Images.Edit) ? Images.Back : Images.Edit;
        }

        public void DeletePerson()
        {
            _personRepository.Delete(_detailPerson.Id);
            LoadPeople();
        }

        public void SaveNewOrEdit(Guid id)
        {
            bool success = NewPersonViewModel.Save();
            if(success)
            {
                if(_edit)
                { 
                    SelectPerson(id);
                }
                else
                {
                    LoadPeople();
                }
                NewPersonViewModel.InitializeValues(new PersonDto());
                _edit = false;
                EditIcon = Images.Edit;
            }
        }

        public void SelectPerson(Guid Id)
        {
            try
            {
                DetailPerson = _personRepository.GetById(Id);
                PlayedInVisible = !DetailPerson.MoviesPlayedIn.Any() ? Visibility.Collapsed : Visibility.Visible;
                DirectedVisible = !DetailPerson.MoviesDirected.Any() ? Visibility.Collapsed : Visibility.Visible;
                ShowDetail();
            }
            catch
            {
                _messageBoxService.Show("Error occured while fetching person details from the database!", "Database error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        public void LoadPeople()
        {
            try
            {
                var people = _personRepository.GetAllAsList();
                ListPeople = new ObservableCollection<PersonListDto>(people.OrderBy(x => x.DisplayName));
                _edit = false;
                ShowList();
            }
            catch
            {
                _messageBoxService.Show("Failed to load people from the database!", "Database error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
