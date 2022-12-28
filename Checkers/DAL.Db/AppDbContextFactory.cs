using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace DAL.Db;

public class AppDbContextFactory: IDesignTimeDbContextFactory<AppDbContext>
{
    public AppDbContext CreateDbContext(string[] args)
    {
        // var dataSource = Environment.GetEnvironmentVariable("DATABASE_URL");
        var dataSource =
            "/Users/aleksandrsmirnov/git/icd0008-2022f/Checkers/ConsoleApp/bin/Debug/net7.0/db/checkers.db";
        
        var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>();
        optionsBuilder.UseSqlite($"Data Source={dataSource}");

        return new AppDbContext(optionsBuilder.Options);
    }
}