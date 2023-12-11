using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

var ctx = new AppDbContext();

var date = new DateIndex(20231211);


ctx.Database.ExecuteSql($"""UPDATE Entity SET "Date"={date};"""); // THROWS


public readonly record struct DateIndex
{
    public readonly int Value;

    public DateIndex(int value)
    {
        Value = value;
    }
}

public sealed class Entity
{
    public int Id { get; private set; }
    public DateIndex Date { get; set; }
}

public sealed class AppDbContext : DbContext
{
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlite("DataSource=:memory:;");
    }

    protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
    {
        configurationBuilder.Properties<DateIndex>(_ =>
            {
                _.HaveConversion<DateIndexToIntConverter>();
            });
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Entity>();
    }

    private sealed class DateIndexToIntConverter : ValueConverter<DateIndex, int>
    {
        public DateIndexToIntConverter()
            : base(model => model.Value, dbValue => new DateIndex(dbValue), convertsNulls: false)
        {
        }
    }
}