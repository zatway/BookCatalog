using BookCatalog.Models;
using BookCatalog.Service;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using BookCatalogWPF;
using System.ComponentModel;
using System.Collections.ObjectModel;
using System.Windows.Input;
using System.Windows;

namespace BookCatalog.ViewModels
{
    public class BookCatalogViewModel : INotifyPropertyChanged
    {
        private readonly MainWindowViewModel _mainViewModel;
        public ICommand OpenCardBookCommand { get; }
        public ICommand AddBookCommand { get; }
        public ICommand PreviousPageCommand { get; }
        public ICommand NextPageCommand { get; }
        public List<Books> BooksList
        {
            get => BooksList;
            set
            {
                
            }
        }

        public BookCatalogViewModel()
        {
            OpenCardBookCommand = new RelayCommand(o => OpenCardBooks());
            AddBookCommand = new RelayCommand(o => this.AddBook());
            PreviousPageCommand = new RelayCommand(o => PreviousPage());
            NextPageCommand = new RelayCommand(o => NextPage());
            LoadBooks();
        }

        void LoadBooks()
        {
            BooksList = GetDataInDB.GetFullTable<Books>();
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public string Title { get; set; }
        private int _authorId { get; set; }
        public int Author
        {
            get => _authorId;
            set => 
                _authorId = value;
        }

        public DateTime YearOfManufacture { get; set; }
        public string ISBN { get; set; }
        public int GenreId { get; set; }

        void NextPage()
        {
            MessageBox.Show("NextPage");
        }

        void PreviousPage()
        {
            Console.WriteLine("PreviousPage");
        }

        void OpenCardBooks()
        {
            MainWindowViewModel main = new MainWindowViewModel();

        }

        void AddBook()
        {
            Console.WriteLine("AddBook");
        }
    }
}
