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
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;
using Npgsql;
using System.Windows.Controls;

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
        public ICommand OpenWindowCommand { get; }
        private ICommand _startSearchCommamnd;
        public ICommand StartSearchCommamnd { get; }
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
        public ObservableCollection<ComboBoxItem> FilterOptions { get; set; }


        private string _searchQuery;
        public string SearchQuery
        {
            get { return _searchQuery; }
            set
            {
                if (_searchQuery != value)
                {
                    _searchQuery = value;
                    OnPropertyChanged(nameof(SearchQuery));
                }
            }
        }

        private ObservableCollection<BookView> _booksList;
        public ObservableCollection<BookView> BooksList
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

        public BookCatalogViewModel()
        {
            OpenCardBookCommand = new RelayCommand(o => OpenCardBooks());
            AddBookCommand = new RelayCommand(o => AddBook());
            PreviousPageCommand = new RelayCommand(o => PreviousPage());
            NextPageCommand = new RelayCommand(o => NextPage());
            StartSearchCommamnd = new RelayCommand(o => SearchBooks());
            LoadBooks();
            _booksList = new ObservableCollection<BookView>();
            FilterOptions = new ObservableCollection<ComboBoxItem>
            {
            new ComboBoxItem { Content = "По названию" },
            new ComboBoxItem { Content = "По автору" },
            new ComboBoxItem { Content = "По жанру" },
            new ComboBoxItem { Content = "По году выпуска" }
            };
            _searchQuery = "";
            _pageNumber = 1;
            _pageSize = 5; 

        }

        void LoadBooks()
        {
            using (var dbContext = new MyDbContext())
            {
                var books = DataService.GetFullTable<Book>();

                var bookView = books.Select(b => new BookView
                {
                    Id = b.id,
                    Title = b.title,
                    Author = dbContext.Authors.FirstOrDefault(a => a.id == b.author_id),
                    YearOfManufacture = b.year_of_manufacture,
                    ISBN = b.isbn,
                    Genre = dbContext.Genres.FirstOrDefault(g => g.id == b.genre_id)
                }).ToList();

                BooksList = new ObservableCollection<BookView>(bookView);
            }
        }

        void SearchBooks()
        {

        }

        private void ApplyFilter()
        {
            
            using (var dbContext = new MyDbContext())
            {
                string selectedFilter = SelectedFilter?.Content.ToString();
                var books = dbContext.Books
                    .FromSqlRaw(@"
            SELECT * FROM filter_books(@selectedFilter, @pageNumber, @pageSize)",
                        new NpgsqlParameter("@selectedFilter", selectedFilter),
                        new NpgsqlParameter("@pageNumber", PageNumber),
                        new NpgsqlParameter("@pageSize", PageSize))
                    .Select(b => new BookView
                    {
                        Id = b.id,
                        Title = b.title,
                        Author = dbContext.Authors.FirstOrDefault(a => a.id == b.author_id),
                        YearOfManufacture = b.year_of_manufacture,
                        ISBN = b.isbn,
                        Genre = dbContext.Genres.FirstOrDefault(g => g.id == b.genre_id)
                    })
                    .ToList();

                BooksList = new ObservableCollection<BookView>(books);
                OnPropertyChanged(nameof(BooksList));
            }
        }

        private int _pageNumber = 1;
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
            
        void PagenatedOutput()
        {
            using (var dbContext = new MyDbContext())
            { 
                IQueryable<Book> query = dbContext.Books;
            int skip = (PageNumber - 1) * PageSize;
            query = query.Skip(skip).Take(PageSize);

            var books = query.Select(b => new BookView
            {
                Id = b.id,
                Title = b.title,
                Author = dbContext.Authors.FirstOrDefault(a => a.id == b.author_id),
                YearOfManufacture = b.year_of_manufacture,
                ISBN = b.isbn,
                Genre = dbContext.Genres.FirstOrDefault(g => g.id == b.genre_id)
            }).ToList();

                 BooksList = new ObservableCollection<BookView>(books);
                OnPropertyChanged(nameof(BooksList));
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

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
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
        }
    }
}
