using BookCatalog.Models;
using BookCatalog.Service;
using BookCatalog.Views;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using System.ComponentModel;
using System.Collections.ObjectModel;
using System.Windows.Input;
using System.Windows;
using BookCatalog.Commands;

namespace BookCatalog.ViewModels
{
    public class BookCatalogViewModel : INotifyPropertyChanged
    {
        public ICommand OpenCardBookCommand { get; }
        public ICommand AddBookCommand { get; }
        public ICommand PreviousPageCommand { get; }
        public ICommand NextPageCommand { get; }
        private ICommand _openWindowCommand;
        public ICommand OpenWindowCommand
        {
            get { return _openWindowCommand; }
        }
        public List<Book> BooksList { get; private set; }
        
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
            BooksList = DataService.GetFullTable<Book>();
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
            MessageBox.Show("PreviousPage");
        }

        void OpenCardBooks()
        {
            var viewModel = new EditBookViewModel(); // Замените на вашу ViewModel для второго окна
            var window = new EditBookWindow(); // Замените на ваше окно WPF
            window.DataContext = viewModel;
            window.ShowDialog();
        }

        void AddBook()
        {
            Console.WriteLine("AddBook");
        }
    }
}
