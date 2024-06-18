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
        public EditBookViewModel(BookView bookView)
        {
            using (var dbContext = new MyDbContext())
            {
                _book = dbContext.Books.FirstOrDefault(b => b.id == bookView.Id);
                Authors = new ObservableCollection<Author>(dbContext.Authors.ToList());
                Genres = new ObservableCollection<Genre>(dbContext.Genres.ToList());
                Author = Authors.FirstOrDefault(a => a.id == _book.author_id);
                Genre = Genres.FirstOrDefault(g => g.id == _book.genre_id);
                Cover = DataService.GetCover(_book.coverimage_id);
                if (Cover == null)
                    SelectFileCommandExecute();
                Title = _book.title;
                YearOfManufacture = _book.year_of_manufacture;
                ISBN = _book.isbn;
            }
            LoadAuthors();
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
        private void LoadAuthors()
{
    List<Author> authors = DataService.GetFullTable<Author>();
    Authors = new ObservableCollection<Author>(authors);
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

        private Author _selectedAuthor;
        public Author SelectedAuthor
        {
            get { return _selectedAuthor; }
            set
            {
                _selectedAuthor = value;
                OnPropertyChanged(nameof(SelectedAuthor));
            }
        }

        public List<string> GetAuthorsName()
        {
            List<Author> authors = DataService.GetFullTable<Author>();
            List<string> authorsName = new List<string>();
            foreach(Author author in authors)
            {
                authorsName.Add(DataService.GetAuthorName(author.id));
            }
            return authorsName;
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
        private void SaveExecute()
        {
            if (!ValidateInputs())
            {
                MessageBox.Show("Не все данные заполнены");
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
                   Genre != null;
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
                    CoverImage cover = dbContext.CoverImages.FirstOrDefault(g => g.id == _book.genre_id);
                    cover.cover_data = DataService.SetImageInDB(filePath);
                    dbContext.SaveChanges();
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
