using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Input;
using BookCatalog.ViewModels;
using BookCatalog.Models;
using BookCatalog.Service;
using BookCatalog.Views;

namespace BookCatalog.ViewModels
{
    public class MainWindowViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        private object _currentViewModel;
        public object CurrentViewModel
        {
            get;
            set;
        }
        

        public ICommand ShowBookCatalogViewCommand { get; }
        public ICommand ShowAddBookViewCommand { get; }
        public ICommand ShowEditBookViewCommand { get; }

        public List<Book> Books { get; set; }
        public RelayCommand OpenBookCatalogWindowCommand { get; }
        public MainWindowViewModel()
        {
            Books = GetDataInDB.LoadBooks();
            OpenBookCatalogWindowCommand = new RelayCommand(AddBookView);
        }

        private void AddBookView(object parameter)
        {
            AddBookWindow secondWindow = new AddBookWindow();
            secondWindow.DataContext = new AddBookViewModels(); // Привязка ViewModel
            secondWindow.Show(); // Открыть новое окно
        }


        public void EditBookView()
        {
           // CurrentViewModel = new EditBookViewModels(this);
        }

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        
    }
}
