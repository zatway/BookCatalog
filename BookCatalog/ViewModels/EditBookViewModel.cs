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
        private readonly MyDbContext _dbContext = new MyDbContext();
        private Book _book;
        private BookForOutput _bookForOutput;
        private BitmapImage _cover;

        public EditBookViewModel(BookForOutput bookForOutput)
        {
            _bookForOutput = bookForOutput;
            LoadBook();
            LoadAuthors();
            LoadGenres();
        }

        public EditBookViewModel()
        {
            LoadAuthors();
            LoadGenres();
        }

        public string Action { get; set; }

        private void LoadBook()
        {
            _book = _dbContext.Books.FirstOrDefault(b => b.id == _bookForOutput.Id);
            if (_book == null)
                return;

            Title = _book.title;
            YearOfManufacture = _book.year_of_manufacture;
            ISBN = _book.isbn;
            Description = _book.description;
            _cover = DataService.GetCover(_dbContext.CoverImage.FirstOrDefault(c => c.id == _book.coverimage_id));
            OnPropertyChanged(nameof(Cover));
        }

        private void LoadAuthors()
        {
            Authors = new ObservableCollection<Author>(_dbContext.Authors.ToList());
        }

        private void LoadGenres()
        {
            Genres = new ObservableCollection<Genre>(_dbContext.Genres.ToList());
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

        private string _coverFilePath;
        public string CoverFilePath
        {
            get { return _coverFilePath; }
            set
            {
                if (_coverFilePath != value)
                {
                    _coverFilePath = value;
                    OnPropertyChanged(nameof(CoverFilePath));
                    LoadCoverImage();
                }
            }
        }

        private void LoadCoverImage()
        {
            if (!string.IsNullOrEmpty(CoverFilePath))
            {
                _cover = new BitmapImage(new Uri(CoverFilePath));
                OnPropertyChanged(nameof(Cover));
            }
        }

        public BitmapImage Cover
        {
            get { return _cover; }
            set
            {
                _cover = value;
                OnPropertyChanged(nameof(Cover));
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
                MessageBox.Show("Не все поля заполнены");
                return;
            }

            try
            {
                using (var dbContext = new MyDbContext())
                {
                    _book.title = Title;
                    _book.year_of_manufacture = YearOfManufacture.ToUniversalTime();
                    _book.isbn = ISBN;

                    _book.author_id = Author.id;
                    CoverImage coverImage = dbContext.CoverImage.FirstOrDefault(c => c.id == _book.coverimage_id);
                    byte[] coverData = DataService.SetImageInDB(CoverFilePath);
                    if (coverImage != null)
                    {
                        coverImage.cover_data = coverData;
                    }
                    else
                    {
                        coverImage.cover_data = coverData;
                        dbContext.CoverImage.Add(coverImage);
                    }
                    _book.genre_id = Genre.id;
                    _book.description = Description;

                    dbContext.Entry(_book).State = EntityState.Modified;
                    dbContext.SaveChanges();
                }
                MessageBox.Show("Изменения успешно сохранены");
            }

            catch (InvalidOperationException ex)
            {
                MessageBox.Show($"Ошибка при выполнении запроса LINQ: {ex.Message}\n{ex.InnerException?.Message}");
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
                   Genre != null;
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

        private void SelectFileCommandExecute()
        {
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                Filter = "Image files (*.png;*.jpg)|*.png;*.jpg|All files (*.*)|*.*"
            };

            if (openFileDialog.ShowDialog() == true)
            {
                CoverFilePath = openFileDialog.FileName;
            }
        }

        private ObservableCollection<Author> _authors;
        public ObservableCollection<Author> Authors
        {
            get { return _authors; }
            set
            {
                _authors = value;
                OnPropertyChanged(nameof(Authors));
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

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
