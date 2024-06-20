using BookCatalog.Commands;
using BookCatalog.Models;
using BookCatalog.Service;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace BookCatalog.ViewModels
{
    internal class AddGenreViewModel
    {
        private readonly Window _parrentWindow;
        public AddGenreViewModel(Window parrentWindow)
        {
            _parrentWindow = parrentWindow;
        }

        private string _name;
        public string Name
        {
            get { return _name; }
            set
            {
                if (_name != value)
                {
                    _name = value;
                    OnPropertyChanged(nameof(_name));
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
        public void SaveExecute()
        {
            if (Validation())
            {
                Genre newGenre = new Genre()
                {
                    Name = Name,
                };
                using (var dbContext = new MyDbContext())
                {
                    dbContext.Genres.Add(newGenre);
                    dbContext.SaveChanges();
                }
                _parrentWindow.Close();
            }
            else
                MessageBox.Show("Поле заполнено некорректно");
        }

        bool Validation() => !string.IsNullOrWhiteSpace(Name);

        public event PropertyChangedEventHandler? PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

    }
}
