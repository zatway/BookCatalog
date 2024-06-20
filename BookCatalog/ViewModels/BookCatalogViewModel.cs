using BookCatalog.Models;
using BookCatalog.Service;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using BookCatalog.Commands;
using BookCatalog.Views;
using Npgsql;
using System.Windows.Controls;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage;
using static System.Reflection.Metadata.BlobBuilder;

namespace BookCatalog.ViewModels
{
    public class BookCatalogViewModel : INotifyPropertyChanged
    {
        private BookForOutput _selectedBook;
        public ICommand OpenCardBookCommand { get; }
        public ICommand AddBookCommand { get; }
        public ICommand RemoveBookCommand { get; }
        public ICommand PreviousPageCommand { get; }
        public ICommand NextPageCommand { get; }
        public ICommand StartSearchCommamnd { get; }
        private string _searchQuery;
        private int _pageNumber = 1;
        private ObservableCollection<Book> _booksList;
        public BookCatalogViewModel()
        {
            CreateDBOrExistsCheck();
            OpenCardBookCommand = new RelayCommand(o => OpenCardBooks());
            AddBookCommand = new RelayCommand(o => AddBook());
            RemoveBookCommand = new RelayCommand(o => RemoveBook());
            PreviousPageCommand = new RelayCommand(o => PreviousPage());
            NextPageCommand = new RelayCommand(o => NextPage());
            StartSearchCommamnd = new RelayCommand(o => StartSearch());
            _booksList = new ObservableCollection<Book>();
            LoadBooks();
            _pageNumber = 1;
            _pageSize = 5;
            PagenatedOutput();
        }
        public BookForOutput SelectedBook
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
        private ComboBoxItem _selectedFilter;
        public ComboBoxItem SelectedFilter
        {
            get => _selectedFilter;
            set
            {
                if (_selectedFilter != value)
                {
                    _selectedFilter = value;
                    OnPropertyChanged(nameof(SelectedFilter));
                    ApplyFilter();
                }
            }
        }
        public string SearchQuery
        {
            get => _searchQuery;
            set
            {
                _searchQuery = value;
                OnPropertyChanged(nameof(SearchQuery));
            }
        }
        public ObservableCollection<Book> BooksList
        {
            get { return _booksList; }
            set
            {
                if (_booksList != value)
                {
                    _booksList = value;
                    OnPropertyChanged(nameof(BooksList));
                }
            }
        }
        public int PageNumber
        {
            get => _pageNumber;
            set
            {
                if (_pageNumber != value)
                {
                    _pageNumber = value;
                    OnPropertyChanged(nameof(PageNumber));
                    PagenatedOutput();
                }
            }
        }
        private int _pageSize;
        public int PageSize
        {
            get => _pageSize;
            set
            {
                if (_pageSize != value)
                {
                    _pageSize = value;
                    OnPropertyChanged(nameof(PageSize));
                    PagenatedOutput();
                }
            }
        }

        void CreateDBOrExistsCheck()
        {
           using (var dbContext = new MyDbContext())
            {
                if (dbContext.Database.GetService<IRelationalDatabaseCreator>().Exists())//проверка на существование бд
                {
                    dbContext.Database.Migrate();
                }
                else
                {
                    dbContext.Database.EnsureCreated();//создание бд
                }
                
            }
        }

        void LoadBooks()
        {
            using (var dbContext = new MyDbContext())
            {
                var books = DataService.GetFullTable<Book>();

                BooksList = new ObservableCollection<Book>(books);
            }
        }

        private void StartSearch()
        {
            if (SearchQuery != null)
            {
                using (var dbContext = new MyDbContext())
                {
                    var books = dbContext.Books
                        .FromSqlRaw(@"
                    SELECT * FROM search_books(@searchQuery, @pageNumber, @pageSize)",
                            new NpgsqlParameter("@searchQuery", $"%{SearchQuery}%"),
                            new NpgsqlParameter("@pageNumber", PageNumber), // For simplicity, start at page 1
                            new NpgsqlParameter("@pageSize", PageSize)) // Adjust page size as needed
                        .Select(b => new Book
                        {
                            Id = b.Id,
                            Title = b.Title,
                            Author = b.Author,
                            YearOfManufacture = b.YearOfManufacture,
                            ISBN = b.ISBN,
                            Genre = b.Genre
                        })
                        .ToList();

                    BooksList = new ObservableCollection<Book>(books);
                    OnPropertyChanged(nameof(BooksList));
                }
            }
            return;
        }

        private void ApplyFilter()
        {
            if(SelectedFilter != null)
            {
                using (var dbContext = new MyDbContext())
                {
                    string selectedFilter = SelectedFilter?.Content.ToString();
                    List<Book> books = dbContext.Books
                        .FromSqlRaw(@"
                            SELECT * FROM filter_books(@selectedFilter, @pageNumber, @pageSize)",
                            new NpgsqlParameter("@selectedFilter", selectedFilter),
                            new NpgsqlParameter("@pageNumber", PageNumber),
                            new NpgsqlParameter("@pageSize", PageSize))
                        .Select(b => new Book
                        {
                            Id = b.Id,
                            Title = b.Title,
                            Author = b.Author,
                            YearOfManufacture = b.YearOfManufacture,
                            ISBN = b.ISBN,
                            Genre = b.Genre
                        }).ToList();
                    BooksList = new ObservableCollection<Book>(books);
                }
           
                OnPropertyChanged(nameof(BooksList));
            }
        }

        void PagenatedOutput()
        {
            using (var dbContext = new MyDbContext())
            { 
                IQueryable<Book> query = dbContext.Books;
            int skip = (PageNumber - 1) * PageSize;
            query = query.Skip(skip).Take(PageSize);

            var books = query.Select(b => new BookForOutput
            {
                Id = b.Id,
                Title = b.Title,
                Author = b.Author,
                YearOfManufacture = b.YearOfManufacture,
                ISBN = b.ISBN,
                Genre = b.Genre
            }).ToList();

                 BooksList = new ObservableCollection<B>(books);
                OnPropertyChanged(nameof(BooksList));
            }
        }
        
        private void NextPage()
        {
            PageNumber++;
        }

        private void PreviousPage()
        {
            if (PageNumber > 1)
            {
                PageNumber--;
            }
        }
        private void OpenCardBooks()
        {
            if (SelectedBook != null)
            {
                var viewModel = new EditBookViewModel(SelectedBook);
                var window = new EditBookWindow();
                window.DataContext = viewModel;
                window.ShowDialog();
            }
            else
            {
                MessageBox.Show("Книга для редактирования не выбрана");
            }
        }
        private void AddBook()
        {
            var viewModel = new EditBookViewModel();
            var window = new EditBookWindow();
            window.DataContext = viewModel;
            window.ShowDialog();
            PagenatedOutput();
            ApplyFilter();
        }

        private void RemoveBook()
        {
            using (var dbContext = new MyDbContext())
            {
                Book book = dbContext.Books.FirstOrDefault(b => b.id == _selectedBook.Id);
                dbContext.Books.Remove(book);
                dbContext.SaveChanges();
            }
            PagenatedOutput();
            ApplyFilter();

        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
