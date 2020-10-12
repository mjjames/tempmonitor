using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using TempMonitorUI.Models;

namespace TempMonitorUI.Data
{
	public class TempMonitorContext : DbContext
	{
		private readonly string _connectionString;

		public TempMonitorContext(IConfiguration configuration) => _connectionString = configuration.GetConnectionString("Sqlite");
		public DbSet<AtmosphericCondition> AtmosphericConditions { get; set; }
		protected override void OnConfiguring(DbContextOptionsBuilder options)
				=> options.UseSqlite(_connectionString);
	}
}
