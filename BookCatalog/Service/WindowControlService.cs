using BookCatalog.ViewModels;
using BookCatalog.Views;
using BookCatalog.Models;
using System.Windows;

namespace BookCatalog.Service
{
    public static class WindowControlService
    {
        /// <summary>
        /// Открытие окна карты книги
        /// </summary>
        /// <param name="SelectBook">выбранная книга</param>
        public static void OpenWindowCardBook(Book SelectBook)
        {
            if (SelectBook != null)
            {
                var viewModel = new OpenAndEditCardBookViewModel(SelectBook);
                var window = new OpenAndEditCardBookWindow()
                {
                    DataContext = viewModel,
                };
                window.DataContext = viewModel;
                window.ShowDialog();
            }
            else
            {
                MessageBox.Show("Книга для редактирования не выбрана");
            }
        }

        /// <summary>
        /// Открытие окна добавления книги
        /// </summary>
        public static void OpenWindowAddBook()
        {
            var viewModel = new AddNewBookViewModel();
            var window = new AddNewBookWindow()
            {
                DataContext = viewModel,
            };
            viewModel.CloseWindow = window.Close;
            window.ShowDialog();
        }

        /// <summary>
        /// Открытие окна добавления жанра
        /// </summary>
        public static void OpenWindowAddGenre()
        {
            var viewModel = new AddGenreViewModel();
            var window = new AddGenreWindow
            {
                DataContext = viewModel,
            };
            viewModel.CloseWindow = window.Close;
            window.ShowDialog();
        }

        /// <summary>
        /// Открытие окна добавления автора
        /// </summary>
        public static void OpenWindowAddAuthor()
        {
            var viewModel = new AddAutorsViewModel();
            var window = new AddAuthorsWindow
            {
                DataContext = viewModel,
            };
            viewModel.CloseWindow = window.Close;
            window.ShowDialog();
        }
    }
}
