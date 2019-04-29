using Microsoft.EntityFrameworkCore;
using SteamGameChecklist.Web.Db.Models;

namespace SteamGameChecklist.Web.Db.Contexts
{
    public class SteamGameChecklistContext : DbContext
    {
        public DbSet<Game> Games { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite("Data Source=checklist.db");
        }
    }
}
