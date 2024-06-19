﻿using BookCatalog.Commands;
using BookCatalog.Models;
using BookCatalog.Service;
using Microsoft.EntityFrameworkCore;
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
    public class AddAutorsViewModel : INotifyPropertyChanged
    {
        private readonly Window _window;
        public AddAutorsViewModel(Window window)
        {
            _window = window;
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
        private string _surname;
        public string Surname
        {
            get { return _surname; }
            set
            {
                if (_surname != value)
                {
                    _surname = value;
                    OnPropertyChanged(nameof(_surname));
                }
            }
        }


        private string _patronymic;
        public string Patronymic
        {
            get { return _patronymic; }
            set
            {
                if (_patronymic != value)
                {
                    _patronymic = value;
                    OnPropertyChanged(nameof(_patronymic));
                }
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
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
            if(Validation())
            {
                string fullName = $"{Name} {Patronymic} {Surname}";
                Author newAuthor = new Author()
                {
                    full_name = fullName,
                };
                using (var dbContext = new MyDbContext())
                {
                    dbContext.Authors.Add(newAuthor);
                    dbContext.SaveChanges();
                }
                _window.Close();
            }
        }

        bool Validation()
        {
            return !string.IsNullOrWhiteSpace(Name) && !string.IsNullOrWhiteSpace(Surname) && !string.IsNullOrWhiteSpace(Patronymic);

        }
    }
}