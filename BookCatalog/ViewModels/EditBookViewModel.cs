using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using BookCatalog.Commands;
using BookCatalog.Models;
using BookCatalog.Service;
using Microsoft.EntityFrameworkCore;
using Microsoft.Win32;

namespace BookCatalog.ViewModels
{
    public class EditBookViewModel : INotifyPropertyChanged
    {
        private Book _book;
        public string Action;
        public EditBookViewModel(Book book)
        {
            Action = "Редактировать";
            _book = book;
            using (var dbContext = new MyDbContext())
            {
                Authors = new ObservableCollection<Author>(dbContext.Authors.ToList());
                Genres = new ObservableCollection<Genre>(dbContext.Genres.ToList());
                Author = Authors.FirstOrDefault(a => a.Id == book.author_id);
                Genre = Genres.FirstOrDefault(g => g.Id == book.genre_id);
                Cover = DataService.GetCover(book.coverimage_id);
            }
            Title = book.title;
            YearOfManufacture = book.year_of_manufacture;
            ISBN = book.isbn;
        }

        public EditBookViewModel()
        {
            Action = "Добавить";
        }

        private Author _author;
        public Author Author
        {
            get { return _author; }
            set
            {
                if (_author != value)
                {
                    _author = value;
                    OnPropertyChanged(nameof(Author));
                }
            }
        }

        private Genre _genre;
        public Genre Genre
        {
            get { return _genre; }
            set
            {
                if (_genre != value)
                {
                    _genre = value;
                    OnPropertyChanged(nameof(Genre));
                }
            }
        }
        private BitmapImage _cover;
        public BitmapImage Cover
        {
            get { return _cover; }
            set
            {
                if (_cover != value)
                {
                    _cover = value;
                    OnPropertyChanged(nameof(Cover));
                }
            }
        }
        private ObservableCollection<Author> _authors;
        public ObservableCollection<Author> Authors
        {
            get { return _authors; }
            set
            {
                if (_authors != value)
                {
                    _authors = value;
                    OnPropertyChanged(nameof(Authors));
                }
            }
        }

        private ObservableCollection<Genre> _genres;
        public ObservableCollection<Genre> Genres
        {
            get { return _genres; }
            set
            {
                if (_genres != value)
                {
                    _genres = value;
                    OnPropertyChanged(nameof(Genres));
                }
            }
        }

        private string _title;
        public string Title
        {
            get { return _title; }
            set
            {
                if (_title != value)
                {
                    _title = value;
                    OnPropertyChanged(nameof(Title));
                }
            }
        }
        private string _isbn;
        public string ISBN
        {
            get { return _isbn; }
            set
            {
                if (_isbn != value)
                {
                    _isbn = value;
                    OnPropertyChanged(nameof(ISBN));
                }
            }
        }

        public int CoverimageId { get;  set; }
        public string Description { get; set; }

        private DateTime _yearOfManufacture;
        public DateTime YearOfManufacture
        {
            get { return _yearOfManufacture; }
            set
            {
                if (_yearOfManufacture != value)
                {
                    _yearOfManufacture = value;
                    OnPropertyChanged(nameof(YearOfManufacture));
                }
            }
        }

        private ICommand _saveCommand;
        public ICommand SaveCommand
        {
            get
            {
                if (_saveCommand == null)
                {
                    _saveCommand = new RelayCommand(param => SaveExecute());
                }
                return _saveCommand;
            }
        }


        private void SaveExecute()
        {
            if (!ValidateInputs())
            {
                MessageBox.Show("Не все данные заполлнены");
            }

            using (var dbContext = new MyDbContext())
            {
                _book.title = Title;
                _book.year_of_manufacture = YearOfManufacture;
                _book.isbn = ISBN;
                _book.author_id = Author.Id;
                _book.genre_id = Genre.Id;
                dbContext.Entry(_book).State = EntityState.Modified;
                dbContext.SaveChanges();
            }
        }

        private bool ValidateInputs()
        {
            if (string.IsNullOrWhiteSpace(Title))
                return false;
            if (YearOfManufacture == default)
                return false;
            if (string.IsNullOrWhiteSpace(ISBN))
                return false;
            if (Author == null)
                return false;
            if (Genre == null)
                return false;

            return true;
        }

        private void SelectFileCommandExecute()
        {
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                Filter = "Image files (*.png;*.jpg)|*.png;*.jpg|All files (*.*)|*.*"
            };
            if (openFileDialog.ShowDialog() == true)
            {
                string filePath = openFileDialog.FileName;
                var bitmap = new BitmapImage(new Uri(filePath));
                Cover = bitmap;
                using (var dbContext = new MyDbContext())
                {
                    CoverImage cover = dbContext.CoverImages.FirstOrDefault(g => g.Id == _book.genre_id);
                    cover.CoverData = DataService.SetImageInDB(filePath);
                }
            }
        }

        // Для флажков
        private ICommand _authorCheckBoxCommand;
        private ICommand _genreCheckBoxCommand;

        public ICommand AuthorCheckBoxCommand
        {
            get
            {
                if (_authorCheckBoxCommand == null)
                {
                    _authorCheckBoxCommand = new RelayCommand(param => CheckBoxCommandExecute(param, "Author"), CanCheckBoxCommandExecute);
                }
                return _authorCheckBoxCommand;
            }
        }

        public ICommand GenreCheckBoxCommand
        {
            get
            {
                if (_genreCheckBoxCommand == null)
                {
                    _genreCheckBoxCommand = new RelayCommand(param => CheckBoxCommandExecute(param, "Genre"), CanCheckBoxCommandExecute);
                }
                return _genreCheckBoxCommand;
            }
        }

        private void CheckBoxCommandExecute(object parameter, string type)
        {
            // Выполнение нужных действий в зависимости от типа (Author или Genre)
            if (type == "Author")
            {
                // Действия для Author
            }
            else if (type == "Genre")
            {
                // Действия для Genre
            }

            IsCheckBoxChecked = false; // Предполагается, что у вас есть свойство для IsChecked в ViewModel
        }

        private bool CanCheckBoxCommandExecute(object parameter) => true;

        private bool _isCheckBoxChecked;
        public bool IsCheckBoxChecked
        {
            get { return _isCheckBoxChecked; }
            set
            {
                if (_isCheckBoxChecked != value)
                {
                    _isCheckBoxChecked = value;
                    OnPropertyChanged(nameof(IsCheckBoxChecked));
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
