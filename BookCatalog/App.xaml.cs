using BookCatalog.Service;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage;
using System.Configuration;
using System.Data;
using System.Windows;
using BookCatalog.Migrations;
using Microsoft.EntityFrameworkCore.Migrations;
using BookCatalog.Views;

namespace BookCatalog
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            // Вызов метода проверки и применения миграций
            CreateDBOrExistsCheck();
        }

        public static void CreateDBOrExistsCheck()
        {
            using (var dbContext = new MyDbContext())
            {
                if (!dbContext.Database.GetService<IRelationalDatabaseCreator>().Exists()) // проверка на существование бд
                    dbContext.Database.EnsureCreated(); // создание бд
                ApplyMigrations(dbContext);
            }
        }

        private static void ApplyMigrations(MyDbContext dbContext)
        {
            try
            {
               
                List<IMigrator> migratorList = new List<IMigrator> { new AddFilterBooksFunction() };
                foreach(var  migrator in migratorList)
                {
                    migrator.Migrate();
                }
                // Применяем все ожидающие миграции
                dbContext.Database.GetMigrations();
            }
            catch (System.Exception ex)
            {
                MessageBox.Show($"Ошибка применения миграций: {ex.Message}");
            }
        }
    }

}
