using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace CalendarApp.Models
{
	public class ApplicationContext : DbContext
	{
		public DbSet<Calendar> Dates { get; set; }

		public DbSet<Note> Notes { get; set; }

		protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
		{
			var connectionStringBuilder = new SqliteConnectionStringBuilder { DataSource = "ApplicationDb.db" };
			var connection = new SqliteConnection(connectionStringBuilder.ToString());
			optionsBuilder.UseSqlite(connection);
		}

		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			modelBuilder.Entity<Note>()
				.HasOne(p => p.Calendar)
				.WithMany(b => b.Notes)
				.OnDelete(DeleteBehavior.Cascade);
		}
	}
}
