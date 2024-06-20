using BookCatalog.ViewModels;
using BookCatalog.Views;
using BookCatalog.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace BookCatalog.Service
{
    public static class WindowControlService
    {
        public static void OpenCardBooks(Book SelectedBook)
        {
            if (SelectedBook != null)
            {
                var viewModel = new EditBookViewModel(SelectedBook);
                var window = new EditBookWindow();
                window.DataContext = viewModel;
                window.ShowDialog();
            }
            else
            {
                MessageBox.Show("Книга для редактирования не выбрана");
            }
        }

        public static void OpenWindowAddBook()
        {
            var viewModel = new EditBookViewModel();
            var window = new EditBookWindow();
            window.DataContext = viewModel;
            window.ShowDialog();
        }

        public static void OpenWindowAddGenre(Window windowSource)
        {
            var viewModel = new AddAutorsViewModel(windowSource);
            var window = new AddAuthorsWindow
            {
                DataContext = viewModel,
                Owner = windowSource
            };
            window.ShowDialog();
        }

        public static void OpenWindowAddAuthor(Window windowSource)
        {
            var viewModel = new AddAutorsViewModel(windowSource);
            var window = new AddAuthorsWindow
            {
                DataContext = viewModel,
                Owner = windowSource
            };
            window.ShowDialog();
        }
    }
}
