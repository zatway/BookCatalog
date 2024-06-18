using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Input;
using BookCatalog.ViewModels;
using BookCatalog.Models;
using BookCatalog.Service;
using BookCatalog.Views;
using BookCatalog.Commands;

namespace BookCatalog.ViewModels
{
    public class MainWindowViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private object _currentViewModel;
        public object CurrentViewModel
        {
            get => _currentViewModel;
            set
            {
                _currentViewModel = value;
                OnPropertyChanged(nameof(CurrentViewModel));
            }
        }

        public ICommand ShowBookCatalogViewCommand { get; }

        public MainWindowViewModel()
        {
            ShowBookCatalogViewCommand = new RelayCommand(o => ShowBookCatalogView());
            ShowBookCatalogView();
        }

        public void ShowBookCatalogView()
        {
            
        }

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }


    }
}
