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
        private BookView _selectedBook;
        public BookView SelectedBook
        {
            get { return _selectedBook; }
            set
            {
                if (_selectedBook != value)
                {
                    _selectedBook = value;
                    OnPropertyChanged(nameof(SelectedBook));
                }
            }
        }

        public ICommand OpenCardBookCommand { get; }
        public ICommand AddBookCommand { get; }
        public ICommand PreviousPageCommand { get; }
        public ICommand NextPageCommand { get; }
        private ICommand _openWindowCommand;
        public ICommand OpenWindowCommand
        {
            get { return _openWindowCommand; }
        }
        public ObservableCollection<BookView> BooksList { get; private set; }

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
            using (var dbContext = new MyDbContext())
            {
                var books = DataService.GetFullTable<Book>();

                var bookViewModels = books.Select(b => new BookView
                {
                    Id = b.id,
                    Title = b.title,
                    AuthorName = DataService.GetAuthorName(b.author_id),
                    YearOfManufacture = b.year_of_manufacture,
                    ISBN = b.isbn,
                    GenreName = DataService.GetGenreName(b.author_id),
                }).ToList();

                BooksList = new ObservableCollection<BookView>(bookViewModels);
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

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
            if (SelectedBook != null)
            {
                var viewModel = new EditBookViewModel(SelectedBook); 
                var window = new EditBookWindow(); 
                window.DataContext = viewModel;
                window.ShowDialog();
            }
        }

        void AddBook()
        {
            Console.WriteLine("AddBook");
        }
    }
}




