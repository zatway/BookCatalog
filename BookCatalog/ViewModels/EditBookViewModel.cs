using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using BookCatalog.Commands;

namespace BookCatalog.ViewModels
{
    public class EditBookViewModel : INotifyPropertyChanged
    {
        private ICommand _checkBoxCommand;

        public ICommand CheckBoxCommand
        {
            get
            {
                if (_checkBoxCommand == null)
                {
                    _checkBoxCommand = new RelayCommand(CheckBoxCommandExecute, CanCheckBoxCommandExecute);
                }
                return _checkBoxCommand;
            }
        }

        private void CheckBoxCommandExecute(object parameter)
        {
            // Выполнение нужных действий

            // Снятие флажка
            IsCheckBoxChecked = false; // предполагается, что у вас есть свойство для IsChecked в ViewModel
        }

        private bool CanCheckBoxCommandExecute(object parameter)
        {
            return true; 
        }

        // Свойство для IsChecked CheckBox
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

        // Реализация INotifyPropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
