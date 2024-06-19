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
using BookCatalog.Views;
using Microsoft.EntityFrameworkCore;
using Microsoft.Win32;

namespace BookCatalog.ViewModels
{
    public class EditBookViewModel : INotifyPropertyChanged
    {
        private Book _book;
        private readonly MyDbContext _dbContext = new MyDbContext();

        public EditBookViewModel(BookView bookView)
        {
            LoadBook(bookView.Id);
            LoadAuthors();
            LoadGenres();
            LoadCover();
        }

        public EditBookViewModel()
        {
            LoadAuthors();
            LoadGenres();
        }

        public string Action { get; set; }

        private void LoadBook(int bookId)
        {
            _book = _dbContext.Books.FirstOrDefault(b => b.id == bookId);
            if (_book == null)
            {
                return;
            }

            Title = _book.title;
            YearOfManufacture = _book.year_of_manufacture;
            ISBN = _book.isbn;
            Description = _book.description;
        }

        private void LoadAuthors()
        {
            Authors = new ObservableCollection<Author>(_dbContext.Authors.ToList());
        }

        private void LoadGenres() 
        { 
            Genres = new ObservableCollection<Genre>(_dbContext.Genres.ToList()); 
        }

        private void LoadCover()
        {
            if(Cover != null)
            {
                Cover = DataService.GetCover((int)_book.coverimage_id);
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

        private int _coverimageId;
        public int CoverimageId
        {
            get { return _coverimageId; }
            set
            {
                if (_coverimageId != value)
                {
                    _coverimageId = value;
                    OnPropertyChanged(nameof(CoverimageId));
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
                string filePath = openFileDialog.FileName;
                var bitmap = new BitmapImage(new Uri(filePath));
                Cover = bitmap;

                byte[] coverData = DataService.SetImageInDB(filePath);

                using (var dbContext = new MyDbContext())
                {
                    CoverImage cover = dbContext.CoverImage.FirstOrDefault(g => g.id == _book.coverimage_id);

                    if (cover == null)
                    {
                        CoverImage coverImage = new CoverImage
                        {
                            cover_data = coverData
                        };
                        dbContext.CoverImage.Add(coverImage);
                        dbContext.SaveChanges();
                        _book.coverimage_id = coverImage.id;
                    }
                    else
                    {
                        cover.cover_data = coverData;
                        dbContext.CoverImage.Update(cover);
                    }

                    dbContext.SaveChanges();
                }
            }
        }

        private ICommand _authorCheckBoxCommand;
        private ICommand _genreCheckBoxCommand;

        public ICommand AuthorCheckBoxCommand
        {
            get
            {
                if (_authorCheckBoxCommand == null)
                {
                    _authorCheckBoxCommand = new RelayCommand(param => CheckBoxCommandExecute(param, "Author"));
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
                    _genreCheckBoxCommand = new RelayCommand(param => CheckBoxCommandExecute(param, "Genre"));
                }
                return _genreCheckBoxCommand;
            }
        }

        private void CheckBoxCommandExecute(object parameter, string type)
        {
            if (type == "Author")
            {
                var window = new AddAuthorsWindow();
                var viewModel = new AddAutorsViewModel(window);
                window.DataContext = viewModel;
                window.ShowDialog();
            }
            else if (type == "Genre")
            {
                var window = new AddGenreWindow();
                var viewModel = new AddGenreViewModel(window);
                window.DataContext = viewModel;
                window.ShowDialog();
            }
        }
        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}