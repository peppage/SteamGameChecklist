using Microsoft.EntityFrameworkCore;
using SteamGameChecklist.DB.Models;

namespace SteamGameChecklist.DB.Contexts
{
    public class ChecklistContext : DbContext
    {
        public DbSet<Game> Games { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite("Data Source=checklist.db");
        }
    }
}
