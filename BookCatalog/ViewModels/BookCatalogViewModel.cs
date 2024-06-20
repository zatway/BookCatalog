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
using Npgsql;
using System.Windows.Controls;

namespace BookCatalog.ViewModels
{
    public class BookCatalogViewModel : INotifyPropertyChanged
    {
        public BookCatalogViewModel()
        {
            DataService.CreateDBOrExistsCheck();
            _pageNumber = 1;
            PageSize = 5;
            BooksList = DataService.PagenatedOutput(PageNumber, PageSize);
        }

        private ICommand _openCardBookCommand;
        public ICommand OpenCardBookCommand
        {
            get
            {
                if (_openCardBookCommand == null)
                {
                    _openCardBookCommand = new RelayCommand(o =>
                    {
                        WindowControlService.OpenCardBooks(SelectedBook);
                        UpdateBookList();
                    });
                }
                return _openCardBookCommand;
            }
        }

        private ICommand _openWindowAddBookCommand;
        public ICommand OpenWindowAddBookCommand
        {
            get
            {
                if (_openWindowAddBookCommand == null)
                {
                    _openWindowAddBookCommand = new RelayCommand(o =>
                    {
                        WindowControlService.OpenWindowAddBook();
                        UpdateBookList();
                    });
                }
                return _openWindowAddBookCommand;
            }
        }

        private ICommand _removeBookCommand;
        public ICommand RemoveBookCommand
        {
            get
            {
                if (_removeBookCommand == null)
                {
                    _removeBookCommand = new RelayCommand(o =>
                    {
                        DataService.RemoveBookForDB(SelectedBook);
                        UpdateBookList();
                    });
                }
                return _removeBookCommand;
            }
        }

        private ICommand _previousPageCommand;
        public ICommand PreviousPageCommand
        {
            get
            {
                if (_previousPageCommand == null)
                {
                    _previousPageCommand = new RelayCommand(o => PreviousPage());
                }
                return _previousPageCommand;
            }
        }

        private ICommand _nextPageCommand;
        public ICommand NextPageCommand
        {
            get
            {
                if (_nextPageCommand == null)
                {
                    _nextPageCommand = new RelayCommand(o => NextPage());
                }
                return _nextPageCommand;
            }
        }

        private ICommand _startSearchCommand;
        public ICommand StartSearchCommand
        {
            get
            {
                if (_startSearchCommand == null)
                {
                    _startSearchCommand = new RelayCommand(o =>
                    {
                        BooksList = DataService.StartSearch(SearchQuery, PageNumber, PageSize);
                    });
                }
                return _startSearchCommand;
            }
        }

        private Book _selectedBook;
        public Book SelectedBook
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
                    BooksList = DataService.ApplyFilter(SelectedFilter, PageNumber, PageSize);
                }
            }
        }
            
        private string _searchQuery;
        public string SearchQuery
        {
            get => _searchQuery;
            set
            {
                _searchQuery = value;
                OnPropertyChanged(nameof(SearchQuery));
            }
        }

        private ObservableCollection<Book> _booksList;
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

        public readonly int PageSize;

        private int _pageNumber;
        public int PageNumber
        {
            get => _pageNumber;
            set
            {
                if (_pageNumber != value)
                {
                    _pageNumber = value;
                    OnPropertyChanged(nameof(PageNumber));
                    BooksList = DataService.PagenatedOutput(PageNumber, PageSize);
                }
            }
        }


        //private void StartSearch()
        //{
        //    if (SearchQuery != null)
        //    {
        //        using (var dbContext = new MyDbContext())
        //        {
        //            BooksList = new ObservableCollection<Book>(dbContext.Books
        //                .FromSqlRaw(@"
        //            SELECT * FROM search_books(@searchQuery, @pageNumber, @pageSize)",
        //                    new NpgsqlParameter("@searchQuery", $"%{SearchQuery}%"),
        //                    new NpgsqlParameter("@pageNumber", PageNumber), // For simplicity, start at page 1
        //                    new NpgsqlParameter("@pageSize", PageSize)) // Adjust page size as needed
        //                .Select(b => new Book
        //                {
        //                    Id = b.Id,
        //                    Title = b.Title,
        //                    Author = b.Author,
        //                    YearOfManufacture = b.YearOfManufacture,
        //                    ISBN = b.ISBN,
        //                    Genre = b.Genre
        //                }).ToList()
        //                );
        //        }
        //    }
        //    return;
        //}

        //private void ApplyFilter()
        //{
        //    if(SelectedFilter != null)
        //    {
        //        using (var dbContext = new MyDbContext())
        //        {
        //            string selectedFilter = SelectedFilter?.Content.ToString();
        //            BooksList = new ObservableCollection<Book>(
        //                dbContext.Books
        //                    .FromSqlRaw(@"
        //                    SELECT * FROM filter_books(@selectedFilter, @pageNumber, @pageSize)",
        //                        new NpgsqlParameter("@selectedFilter", selectedFilter),
        //                        new NpgsqlParameter("@pageNumber", PageNumber),
        //                        new NpgsqlParameter("@pageSize", PageSize))
        //                    .Select(b => new Book
        //                    {
        //                        Id = b.Id,
        //                        Title = b.Title,
        //                        Author = b.Author,
        //                        YearOfManufacture = b.YearOfManufacture,
        //                        ISBN = b.ISBN,
        //                        Genre = b.Genre
        //                    }).ToList()
        //            );
        //        }
        //    }
        //}

        //void PagenatedOutput()
        //{
        //    using (var dbContext = new MyDbContext())
        //    {
        //        IQueryable<Book> query = dbContext.Books;
        //        int skip = (PageNumber - 1) * PageSize;
        //        query = query.Skip(skip).Take(PageSize);

        //        BooksList = new ObservableCollection<Book>(query.Select(b => new Book
        //        {
        //            Id = b.Id,
        //            Title = b.Title,
        //            Author = b.Author,
        //            YearOfManufacture = b.YearOfManufacture,
        //            ISBN = b.ISBN,
        //            Genre = b.Genre
        //        }).ToList()
        //    );
        //    }
        //}
        
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

        //private void OpenCardBooks()
        //{
        //    //if (SelectedBook != null)
        //    //{
        //    //    var viewModel = new EditBookViewModel(SelectedBook);
        //    //    var window = new EditBookWindow();
        //    //    window.DataContext = viewModel;
        //    //    window.ShowDialog();
        //    //}
        //    //else
        //    //{
        //    //    MessageBox.Show("Книга для редактирования не выбрана");
        //    //}
        //}

        //private void AddBook()
        //{
        //    var viewModel = new EditBookViewModel();
        //    var window = new EditBookWindow();
        //    window.DataContext = viewModel;
        //    window.ShowDialog();
        //    PagenatedOutput();
        //    ApplyFilter();
        //}

        //private void RemoveBook()
        //{
        //    BookControlService.RemoveBook(_selectedBook);
        //    PagenatedOutput();
        //    ApplyFilter();
        //}
       
        private void UpdateBookList()
        {
            if (SelectedFilter?.Content?.ToString() != null)
                BooksList = DataService.ApplyFilter(SelectedFilter, PageNumber, PageSize);
            else
                BooksList = DataService.PagenatedOutput(PageNumber, PageSize);
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
