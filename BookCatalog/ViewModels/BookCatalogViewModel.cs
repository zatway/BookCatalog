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

        /// <summary>
        /// конструктор класса BookCatalogViewModel
        /// </summary>
        public BookCatalogViewModel()
        {
            StartSearchCommand = new RelayCommand(o =>
            {
                BooksList = DataService.SearchAndFilter(SearchQuery, SelectedFilter, PageNumber, PageSize);
            });
            OpenCardBookCommand = new RelayCommand(o =>
            {
                WindowControlService.OpenWindowCardBook(SelectedBook);
                UpdateBookList();
            });

            OpenWindowAddBookCommand = new RelayCommand(o =>
            {
                WindowControlService.OpenWindowAddBook();
                UpdateBookList();
            });

            RemoveBookCommand = new RelayCommand(o =>
            {
                if (SelectedBook != null)
                    DataService.RemoveBookForDB(SelectedBook);
                else
                    MessageBox.Show("Книга не выбрана");
                UpdateBookList();
            });

            PreviousPageCommand = new RelayCommand(o =>
            {
                PreviousPage();
            });

            NextPageCommand = new RelayCommand(o =>
            {
                NextPage();
            });

            _pageNumber = 1;
            PageSize = 5;
            UpdateBookList();
        }

        /// <summary>
        /// Команда для открытия карточки книги
        /// </summary>
        public ICommand OpenCardBookCommand { get; }

        /// <summary>
        /// Команда для открытия окна добавления книги
        /// </summary>
        public ICommand OpenWindowAddBookCommand { get; }

        /// <summary>
        /// Команда для удаления книги
        /// </summary>

        public ICommand RemoveBookCommand { get; }

        /// <summary>
        /// Команда для переключения на предыдущую страницу
        /// </summary>

        public ICommand PreviousPageCommand { get; }

        /// <summary>
        /// Команда для переключения на следующую страницу
        /// </summary>

        public ICommand NextPageCommand { get; }

        /// <summary>
        /// Команда для поиска книг в таблице по ее названию
        /// </summary>

        public ICommand StartSearchCommand { get; }

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
                    if (value.Content.ToString() == "Убрать фильтры")
                        _selectedFilter = null;
                    else
                        _selectedFilter = value;
                                        
                    BooksList = DataService.SearchAndFilter(SearchQuery, SelectedFilter, PageNumber, PageSize);
                    OnPropertyChanged(nameof(SelectedFilter));
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
                    BooksList = DataService.SearchAndFilter(SearchQuery, SelectedFilter, PageNumber, PageSize);
                    OnPropertyChanged(nameof(PageNumber));
                    
                }
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
        

        /// <summary>
        /// Обновление списка книг, после какого либо действия
        /// </summary>
        private void UpdateBookList()
        {
            BooksList = DataService.SearchAndFilter(SearchQuery, SelectedFilter, PageNumber, PageSize);
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
