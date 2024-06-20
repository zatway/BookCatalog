using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
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

        public EditBookViewModel(Book book)
        {
            _book = book;
            LoadBook();
            LoadAuthors();
            LoadGenres();
        }

        public EditBookViewModel()
        {
            LoadAuthors();
            LoadGenres();
        }

        private ICommand _selectFileCommand;
        public ICommand SelectFileCommand
        {
            get
            {
                if (_selectFileCommand == null)
                {
                    _selectFileCommand = new RelayCommand(param => SelectFileCommandExecute());
                }
                return _selectFileCommand;
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

        private ObservableCollection<Genre> _genres;
        public ObservableCollection<Genre> Genres
        {
            get => _genres;
            set
            {
                if (_genres != value)
                {
                    _genres = value;
                    OnPropertyChanged(nameof(Genres));
                }
            }
        }

        private ObservableCollection<Author> _authors;
        public ObservableCollection<Author> Authors
        {
            get { return _authors; }
            set
            {
                if (value != _authors)
                {
                    _authors = DataService.GetFullTable<Author>();
                    OnPropertyChanged(nameof(Authors));
                }
            }
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

        private CoverImage _coverImage;
        public CoverImage CoverImage
        {
            get => _coverImage;
            set
            {
                if (_coverImage != value)
                {
                    _coverImage = value;
                    OnPropertyChanged(nameof(CoverImage));
                }
            }
        }

        private BitmapImage _coverImageInBytes;
        public BitmapImage CoverImageBitmap
        {
            get => _coverImageInBytes;
            set
            {
                if (_coverImageInBytes == null || _coverImageInBytes != value)
                {
                    _coverImageInBytes = DataService.GetCover(_coverImage.ImageData);
                    OnPropertyChanged(nameof(CoverImageBitmap));
                }
            }
        }

        private string _description;
        public string Description
        {
            get { return _description; }
            set
            {
                if (_description != value)
                {
                    _description = value;
                    OnPropertyChanged(nameof(Description));
                }
            }
        }

        private void LoadBook()
        {
            Title = _book.Title;
            YearOfManufacture = _book.YearOfManufacture;
            ISBN = _book.ISBN;
            Description = _book.Description;
            Author = _book.Author;
            Genre = _book.Genre;
            CoverImage = _book.CoverImage;
        }

        private void LoadGenres()
        {
            Genres = DataService.GetFullTable<Genre>();
        }

        private void LoadAuthors()
        {
            Authors = DataService.GetFullTable<Author>();
        }

        private void SaveExecute()
        {
            if (!ValidateInputs())
            {
                MessageBox.Show("Не все поля заполнены");
                return;
            }

            try
            {
                using (var dbContext = new MyDbContext())
                {
                    _book.Title = Title;
                    _book.YearOfManufacture = YearOfManufacture.ToUniversalTime();
                    _book.ISBN = ISBN;
                    _book.Author = Author;
                    _book.CoverImage = CoverImage;
                    dbContext.Entry(_book).State = EntityState.Modified;
                    dbContext.SaveChanges();
                }
                MessageBox.Show("Изменения успешно сохранены");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Произошла ошибка при сохранении данных: {ex.Message}");
            }
        }

        private bool ValidateInputs()
        {
            return !string.IsNullOrWhiteSpace(Title) &&
                   YearOfManufacture != default(DateTime) &&
                   !string.IsNullOrWhiteSpace(ISBN) &&
                   Author != null &&
                   Genre != null && CoverImageBitmap != null;
        }

        private void SelectFileCommandExecute()
        {
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                Filter = "Image files (*.png;*.jpg)|*.png;*.jpg|All files (*.*)|*.*"
            };

            if (openFileDialog.ShowDialog() == true)
            {
                string selectedImagePath = openFileDialog.FileName;
                CoverImage.ImageData = File.ReadAllBytes(selectedImagePath);
                CoverImageBitmap = new BitmapImage(new Uri(selectedImagePath)); 
            }
        }
        private bool _isAuthorNotInListChecked;
        public bool IsAuthorNotInListChecked
        {
            get => _isAuthorNotInListChecked;
            set
            {
                if (_isAuthorNotInListChecked != value)
                {
                    _isAuthorNotInListChecked = value;
                    OnPropertyChanged(nameof(IsAuthorNotInListChecked));
                    if (_isAuthorNotInListChecked)
                    {
                        AuthorCheckBoxCommand.Execute(null); 
                    }
                }
            }
        }

        private ICommand _authorCheckBoxCommand;
        public ICommand AuthorCheckBoxCommand
        {
            get
            {
                if (_authorCheckBoxCommand == null)
                {
                    _authorCheckBoxCommand = new RelayCommand(param => ExecuteAuthorCheckBoxCommand());
                }
                return _authorCheckBoxCommand;
            }
        }

        private void ExecuteAuthorCheckBoxCommand()
        {
            if (IsAuthorNotInListChecked)
            {
                WindowControlService.OpenWindowAddAuthor(Application.Current.MainWindow);
                LoadAuthors();
            }
            IsAuthorNotInListChecked = false;
        }

        private bool _isGenreNotInListChecked;
        public bool IsGenreNotInListChecked
        {
            get => _isGenreNotInListChecked;
            set
            {
                if (_isGenreNotInListChecked != value)
                {
                    _isGenreNotInListChecked = value;
                    OnPropertyChanged(nameof(IsGenreNotInListChecked));
                    if (_isGenreNotInListChecked)
                    {
                        GenreCheckBoxCommand.Execute(null);
                    }
                }
            }
        }

        private ICommand _genreCheckBoxCommand;
        public ICommand GenreCheckBoxCommand
        {
            get
            {
                if (_genreCheckBoxCommand == null)
                {
                    _genreCheckBoxCommand = new RelayCommand(param => ExecuteGenreCheckBoxCommand());
                }
                return _authorCheckBoxCommand;
            }
        }

        private void ExecuteGenreCheckBoxCommand()
        {
            if (IsGenreNotInListChecked)
            {
                WindowControlService.OpenWindowAddGenre(Application.Current.MainWindow);
                LoadGenres();
            }
            IsGenreNotInListChecked = false;
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
