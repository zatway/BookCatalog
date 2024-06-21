using BookCatalog.Service;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookCatalog.Migrations
{
    internal class AddFilterBooksFunction : IMigrator
    {

        private readonly string _filterBooksSql = @"
           CREATE OR REPLACE FUNCTION filter_books(
    selected_filter TEXT,
    page_number INT,
    page_size INT
)
RETURNS TABLE (
    id INT,
    title VARCHAR(70),
    author_id INT,
    year_of_manufacture DATE,
    isbn VARCHAR(30),
    genre_id INT,
    author_full_name VARCHAR(100),
    genre_name VARCHAR(50)
) AS $$
BEGIN
    RETURN QUERY
    SELECT
        b.id,
        b.title,
        b.author_id,
        b.year_of_manufacture,
        b.isbn,
        b.genre_id,
        a.full_name AS author_full_name,
        g.name AS genre_name
    FROM
        books b
        JOIN authors a ON b.author_id = a.id
        JOIN genres g ON b.genre_id = g.id
    ORDER BY
        CASE
            WHEN selected_filter = 'По названию' THEN b.title
            WHEN selected_filter = 'По автору' THEN a.full_name
            WHEN selected_filter = 'По жанру' THEN g.name
            WHEN selected_filter = 'По году выпуска' THEN b.year_of_manufacture::TEXT
        END
    OFFSET (page_number - 1) * page_size
    LIMIT page_size;
END;
$$ LANGUAGE plpgsql;";
        public void Migrate(string? targetMigration = null)
        {
            using (var dbContext = new MyDbContext())
            {
                dbContext.Database.ExecuteSqlRaw(_filterBooksSql);
            }
        }

        public async Task MigrateAsync(string? targetMigration = null, CancellationToken cancellationToken = default)
        {
            using (var dbContext = new MyDbContext())
            {
                await dbContext.Database.ExecuteSqlRawAsync(_filterBooksSql, cancellationToken);
            }
        }

        public string GenerateScript(string? fromMigration = null, string? toMigration = null, MigrationsSqlGenerationOptions options = MigrationsSqlGenerationOptions.Default)
        {
            return _filterBooksSql;
        }
    }
}
